using System.ComponentModel.DataAnnotations;

namespace OKbkm.Models
{
    public class Login
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "TC kimlik numarası gereklidir.")]
        public string TC { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        public string Password { get; set; }

        public DateTime LoginDate { get; set; } = DateTime.UtcNow;
    }
}
