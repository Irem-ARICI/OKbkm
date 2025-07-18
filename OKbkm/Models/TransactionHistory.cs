using System.ComponentModel.DataAnnotations;

namespace OKbkm.Models
{
    public class TransactionHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Hesap numarası 12 haneli olmalıdır.")]
        public string AccountNo { get; set; }

        [Required]
        public decimal TransactionAmount { get; set; }

        [Required]
        public decimal BalanceAfter { get; set; }

        [Required]
        public string TransactionType { get; set; }
        public string? Description { get; set; } //  Açıklama/mesaj desteği

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}
