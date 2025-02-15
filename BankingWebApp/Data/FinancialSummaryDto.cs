namespace BankingWebApp.Data
{
    public class FinancialSummaryDto
    {
        public decimal TotalDeposits { get; set; }
        public decimal TotalWithdrawals { get; set; }
        public decimal NetBalance { get; set; }
    }
}
