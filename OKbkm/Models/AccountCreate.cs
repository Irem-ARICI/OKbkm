using System.ComponentModel.DataAnnotations;

namespace OKbkm.Models
{
    public class AccountCreate
    {
        public int id { get; set; }
        public string TC { get; set; }
        public int AccountNo { get; set; }
        public string CardType { get; set; }
        public decimal Balance { get; set; } =0.00m; // varsayılan 0.00 TL
    }
}
