using BankingWebApp.Data;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BankingWebApp.Services
{
    public class BankingService : IBankingService
    {
        private readonly HttpClient _httpClient;
        private readonly ProtectedSessionStorage _sessionStorage;

        public BankingService(IHttpClientFactory httpClientFactory, ProtectedSessionStorage sessionStorage)
        {
            _httpClient = httpClientFactory.CreateClient("BankingAPI"); //Get registered HttpClient
            _sessionStorage = sessionStorage;
        }

        public async Task<decimal> GetBalanceAsync()
        {
            return await _httpClient.GetFromJsonAsync<decimal>("api/banking/balance");
        }

        public async Task<FinancialSummaryDto> GetFinancialSummaryAsync(int accountId)
        {

            string token = string.Empty;
            try
            {
                var tokenResult = await _sessionStorage.GetAsync<string>("authToken");
                if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
                {
                    token = tokenResult.Value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving token: " + ex.Message);
            }

            // If we got a token, manually attach it to the HttpClient.
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine("Token manually set for the request.");
            }
            else
            {
                Console.WriteLine("No token found in session storage.");
            }

            var response = await _httpClient.GetAsync($"api/transactions/summary/{accountId}");

            // Check if the response is 401 Unauthorized.
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Log a debug message indicating a token issue.
                Console.WriteLine("DEBUG: Received 401 Unauthorized. This may be due to a missing or invalid token.");

                // Optionally, you can throw an exception to propagate the error.
                throw new UnauthorizedAccessException("401 Unauthorized: Token issue suspected.");
            }

            // If the response is not successful, you can log and throw or handle it accordingly.
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"DEBUG: Received status code {response.StatusCode} from the API.");
                response.EnsureSuccessStatusCode();
            }

            // Deserialize and return the FinancialSummaryDto.
            var summary = await response.Content.ReadFromJsonAsync<FinancialSummaryDto>();
            return summary;
        }

        public async Task<List<TransactionDto>> GetRecentTransactionsAsync(int accountId)
        {
            return await _httpClient.GetFromJsonAsync<List<TransactionDto>>($"api/transactions/recent/{accountId}");
        }


        public async Task<bool> TransferFundsAsync(string fromAccount, string toAccount, decimal amount)
        {
            var transfer = new { FromAccountId = fromAccount, ToAccountId = toAccount, Amount = amount };
            var response = await _httpClient.PostAsJsonAsync("api/transactions/transfer", transfer);
            return response.IsSuccessStatusCode;
        }
    }

}
