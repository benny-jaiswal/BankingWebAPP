namespace BankingWebApp.Data
{
    public class TransactionDto
    {
        public int TransactionId { get; set; } // Unique ID for the transaction
        public string AccountNumber { get; set; } // Affected account
        public decimal Amount { get; set; } // Transaction amount
        public string TransactionType { get; set; } // e.g., "Deposit", "Withdrawal", "Transfer"
        public DateTime TransactionDate { get; set; } // Date of transaction
        public string Description { get; set; } // Optional details
    }

}
