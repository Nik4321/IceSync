using IceSync.Options;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace IceSync.Services.Clients
{
    public class AuthorizationHeaderHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UniversalLoaderOptions _options;
        private readonly ILogger<AuthorizationHeaderHandler> _logger;
        private static string _token;

        public AuthorizationHeaderHandler(
            IHttpClientFactory httpClientFactory,
            IOptions<UniversalLoaderOptions> options,
            ILogger<AuthorizationHeaderHandler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            var token = await GetTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_token) || !CheckTokenIsValid(_token))
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();

                    var requestBody = new
                    {
                        apiCompanyId = _options.CompanyId,
                        apiUserId = _options.UserId,
                        apiUserSecret = _options.UserSecret
                    };

                    var jsonRequest = JsonSerializer.Serialize(requestBody);
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"{_options.BaseUrl}/authenticate", content, cancellationToken);

                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    _token = jsonResponse;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to retrieve JWT token.");
                    throw;
                }
            }

            return _token;
        }

        private bool CheckTokenIsValid(string token)
        {
            var tokenTicks = GetTokenExpirationTime(token);
            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;

            var now = DateTime.Now.ToUniversalTime();

            var valid = tokenDate >= now;

            return valid;
        }

        private long GetTokenExpirationTime(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
            var ticks = long.Parse(tokenExp);
            return ticks;
        }
    }
}
