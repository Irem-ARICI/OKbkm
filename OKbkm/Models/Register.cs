using Microsoft.AspNetCore.Identity;

namespace OKbkm.Models
{
    public class Register
    {
        public int id { get; set; }
        public string TC { get; set; }
        public string NameUsername { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; } 
        public string Address { get; set; }
    }
}
