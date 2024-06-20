using IceSync.Models;
using System.Text.Json;

namespace IceSync.Services.Clients
{
    public class UniversalLoaderClient : IUniversalLoaderClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UniversalLoaderService> _logger;

        public UniversalLoaderClient(HttpClient httpClient, ILogger<UniversalLoaderService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<WorkflowModel>> GetWorkflowsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("/workflows", cancellationToken);

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
                var workflows = JsonSerializer.Deserialize<IEnumerable<WorkflowModel>>(jsonResponse, GetJsonSerializerOptions());

                return workflows;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve workflows.");
                throw;
            }
        }

        public async Task<bool> RunWorkflowAsync(string workflowId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsync($"/workflows/{workflowId}/run", null, cancellationToken);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to run workflow with ID {workflowId}.");
                return false;
            }
        }

        private static JsonSerializerOptions GetJsonSerializerOptions() {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            return options;
        }
    }
}
