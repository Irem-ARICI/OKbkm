using System.ComponentModel.DataAnnotations;

namespace OKbkm.Models
{
    public class TransactionEvent
    {
        [Key]
        public int Id { get; set; }
        public string AccountNo { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Type { get; set; } // Deposit, Withdraw, Transfer-Sent, Transfer-Received
        public DateTime Timestamp { get; set; }
    }
}
