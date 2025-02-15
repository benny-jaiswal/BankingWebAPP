using BankingWebApp.Data;

namespace BankingWebApp.Services
{
    public interface IBankingService
    {
        Task<decimal> GetBalanceAsync();
        Task<FinancialSummaryDto> GetFinancialSummaryAsync(int accountId);
        Task<List<TransactionDto>> GetRecentTransactionsAsync(int accountId);
        Task<bool> TransferFundsAsync(string fromAccount, string toAccount, decimal amount);
    }
}
