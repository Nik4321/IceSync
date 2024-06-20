using IceSync.Models;

namespace IceSync.Services
{
    public interface IUniversalLoaderService
    {
        Task<IEnumerable<WorkflowModel>> GetWorkflowsAsync(CancellationToken cancellationToken = default);

        Task<bool> RunWorkflowAsync(string workflowId, CancellationToken cancellationToken = default);
    }
}
