namespace OKbkm.Models
{
    public class TransactionEvent
    {
        public string AccountNo { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Type { get; set; } // Deposit, Withdraw, Transfer-Sent, Transfer-Received
        public DateTime Timestamp { get; set; }
    }
}
