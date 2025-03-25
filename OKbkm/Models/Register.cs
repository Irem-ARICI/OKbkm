using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OKbkm.Models
{
    public class Register
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "TC kimlik numarası zorunludur.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "TC kimlik numarası 11 haneli rakamlardan oluşmalıdır.")]
        public string TC { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string NameUsername { get; set; }

        [Required(ErrorMessage = "Mail adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Şifre en az 4 karakter olmalıdır.")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).+$", ErrorMessage = "Şifre en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.")]
        public string Password { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string PhoneNumber { get; set; } 
        public string Address { get; set; }
    }
}
