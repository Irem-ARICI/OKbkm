using Microsoft.AspNetCore.Identity;

namespace OKbkm.Models
{
    public class Register
    {
        public int id { get; set; } // [Key]    yazarsın bi üsrtlerine :))
        public string TC { get; set; }  // [Required]
        public string NameUsername { get; set; }    // [Required]
        public string Mail { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; } 
        public string Address { get; set; }
    }
}
