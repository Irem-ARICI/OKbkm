namespace OKbkm.Models
{
    public class TransactionHistory
    {
        public int id { get; set; }
        public int AccountNo { get; set; }
        public decimal Balance { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
