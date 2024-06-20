using IceSync.Models;
using IceSync.Services;
using IceSync.Services.Clients;

public class UniversalLoaderService : IUniversalLoaderService
{
    private readonly IUniversalLoaderClient _universalLoaderClient;

    public UniversalLoaderService(IUniversalLoaderClient universalLoaderClient)
    {
        _universalLoaderClient = universalLoaderClient;
    }

    public async Task<IEnumerable<WorkflowModel>> GetWorkflowsAsync(CancellationToken cancellationToken = default)
    {
        return await _universalLoaderClient.GetWorkflowsAsync(cancellationToken);
    }

    public async Task<bool> RunWorkflowAsync(string workflowId, CancellationToken cancellationToken = default)
    {
        return await _universalLoaderClient.RunWorkflowAsync(workflowId, cancellationToken);
    }
}