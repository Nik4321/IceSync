using IceSync.Data;
using IceSync.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IceSync.Services.Sync
{
    public class WorkflowSyncService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<WorkflowSyncService> _logger;

        public WorkflowSyncService(IServiceScopeFactory serviceScopeFactory, ILogger<WorkflowSyncService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(30));
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var universalLoaderService = scope.ServiceProvider.GetRequiredService<IUniversalLoaderService>();
                    var dataContext = scope.ServiceProvider.GetRequiredService<IceSyncDbContext>();

                    try
                    {
                        var workflowsFromApi = await universalLoaderService.GetWorkflowsAsync(cancellationToken);
                        var workflowIdsFromApi = workflowsFromApi.Select(x => x.Id);

                        var query = dataContext.Workflows.AsQueryable();

                        // Update existing workflows
                        var existingWorkflows = query.Where(w => workflowIdsFromApi.Contains(w.WorkflowId));
                        foreach (var workflow in workflowsFromApi)
                        {
                            var dbWorkflow = await existingWorkflows.FirstOrDefaultAsync(db => db.WorkflowId == workflow.Id);
                            if (dbWorkflow != null)
                            {
                                dbWorkflow.WorkflowName = workflow.Name;
                                dbWorkflow.IsActive = workflow.IsActive;
                                dbWorkflow.MultiExecBehavior = workflow.MultiExecBehavior;
                                dataContext.Workflows.Update(dbWorkflow);
                            }
                        }

                        // Add workflows
                        var newWorkflows = workflowsFromApi.Where(w => !query.Any(db => db.WorkflowId == w.Id));
                        await dataContext.Workflows.AddRangeAsync(newWorkflows.Select(w => new Workflow
                        {
                            WorkflowId = w.Id,
                            WorkflowName = w.Name,
                            IsActive = w.IsActive,
                            MultiExecBehavior = w.MultiExecBehavior
                        }));

                        // Remove workflows
                        var removeWorkflows = query.Where(w => !workflowIdsFromApi.Contains(w.WorkflowId));

                        dataContext.Workflows.RemoveRange(removeWorkflows);

                        await dataContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred during workflow synchronization.");
                    }
                }
            }
        }
    }

}