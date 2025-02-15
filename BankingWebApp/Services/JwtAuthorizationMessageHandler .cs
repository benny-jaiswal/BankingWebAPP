using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;

namespace BankingWebApp.Services
{
    public class JwtAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ProtectedSessionStorage _sessionStorage;

        public JwtAuthorizationMessageHandler(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var tokenResult = await _sessionStorage.GetAsync<string>("authToken"); // Get JWT token from session storage
                if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value); // Attach token to request
                }
            }
            catch (InvalidOperationException)
            {
                // 🔹 Handle issue if session storage is not ready
            }
            catch
            {
                // Optionally catch other exceptions and proceed without a token.
            }


            return await base.SendAsync(request, cancellationToken);
        }
    }
}
