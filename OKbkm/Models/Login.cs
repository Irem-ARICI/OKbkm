namespace OKbkm.Models
{
    public class Login
    {
        public int id { get; set; }
        //[Required(ErrorMessage = "TC Kimlik Numarası gereklidir.")]
        public string TC { get; set; }
        //   [Required(ErrorMessage = "Şifre gereklidir.")]
        public string Password { get; set; }
        public DateTime LoginDate { get; set; } // 🆕 Giriş zamanı
    }
}
