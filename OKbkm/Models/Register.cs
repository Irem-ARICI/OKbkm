using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKbkm.Models
{
    public class Register
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "TC kimlik numarası zorunludur.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "TC kimlik numarası 11 haneli olmalıdır.")]
        public string TC { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        public string NameUsername { get; set; }

        [Required(ErrorMessage = "Mail adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [RegularExpression(@"(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{6,}", ErrorMessage = "Şifre büyük harf, küçük harf ve rakam içermelidir.")]
        public string Password { get; set; }

        [NotMapped]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;
    }
}
