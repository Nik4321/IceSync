using IceSync.Models;

namespace IceSync.Services.Clients
{
    public interface IUniversalLoaderClient
    {
        Task<IEnumerable<WorkflowModel>> GetWorkflowsAsync(CancellationToken cancellationToken = default);

        Task<bool> RunWorkflowAsync(string workflowId, CancellationToken cancellationToken = default);
    }
}
