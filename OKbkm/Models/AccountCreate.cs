using System.ComponentModel.DataAnnotations;

namespace OKbkm.Models
{
    public class AccountCreate
    {
        public int id { get; set; }
        public string TC { get; set; }
        public int AccountNo { get; set; }

 
        public string CardType { get; set; } 
    }
}
