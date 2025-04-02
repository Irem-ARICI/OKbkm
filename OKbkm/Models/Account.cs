using System.ComponentModel.DataAnnotations;

namespace OKbkm.Models
{
    public class Account
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string TC { get; set; }
        [Required]
        public int AccountNo { get; set; }  // random oluşaca

        [Required]
        public string CardType { get; set; }  // Seçilecek

        [Required]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; } = 0.00m; // default
    }
}
