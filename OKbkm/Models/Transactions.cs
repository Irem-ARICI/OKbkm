using System.ComponentModel.DataAnnotations;

namespace OKbkm.Models
{
    public class Transactions
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hesap numarası zorunludur.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Hesap numarası 9 haneli olmalıdır.")]
        public int AccountNo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Toplam tutar negatif olamaz.")]
        public decimal Total { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Yatırılan para negatif olamaz.")]
        public decimal Deposit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Çekilen para negatif olamaz.")]
        public decimal Withdrawal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Transfer tutarı negatif olamaz.")]
        public decimal Transfer { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Bakiye negatif olamaz.")]
        public decimal Balance { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "İşlem türü zorunludur.")]
        public string TransactionType { get; set; } // Para Yatırma, Çekme, Transfer
        public int? ReceiverAccountNo { get; set; }
    }
}
