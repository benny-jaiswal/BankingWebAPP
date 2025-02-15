using BankingWebApp.Data;
using BankingWebApp.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BankingWebApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ProtectedSessionStorage _sessionStorage;

        public AuthService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authStateProvider, ProtectedSessionStorage sessionStorage)
        {
            _httpClientFactory = httpClientFactory;
            _authStateProvider = authStateProvider;
            _sessionStorage = sessionStorage;
        }

        public async Task<bool> Login(string username, string password)
        {
            var httpClient = _httpClientFactory.CreateClient("BankingAPI");

            var loginRequest = new { userName = username, passwordHash = password };
            var response = await httpClient.PostAsJsonAsync("api/user/login", loginRequest);

            if (!response.IsSuccessStatusCode)
                return false;

            var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
            if (userDto == null || string.IsNullOrEmpty(userDto.Token))
                return false;
            await _sessionStorage.SetAsync("authToken", userDto.Token); //  Store JWT token in SessionStorage
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(userDto.Token);

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userDto.Token);

            return true;
        }

        public async Task Logout()
        {
            await _sessionStorage.DeleteAsync("authToken"); //  Remove Token from SessionStorage
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
        }

        public async Task<string> GetToken()
        {
            var tokenResult = await _sessionStorage.GetAsync<string>("authToken");
            return tokenResult.Success ? tokenResult.Value : null;
        }
        public async Task<bool> IsAuthenticated()
        {
            var token = await GetToken();
            return !string.IsNullOrEmpty(token);
        }
    }
}
