using System.ComponentModel.DataAnnotations;

namespace OKbkm.Models
{
    public class AccountCreate
    {
        [Key]
        public int id { get; set; }

        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "TC kimlik numarası 11 haneli olmalıdır.")]
        public string TC { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "12 haneli hesap numarası olmalıdır.")]
        public string AccountNo { get; set; }

        [Required]
        public string CardType { get; set; } // Visa / MasterCard gibi

        [Required]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; } = 0.00m;

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    }
}
