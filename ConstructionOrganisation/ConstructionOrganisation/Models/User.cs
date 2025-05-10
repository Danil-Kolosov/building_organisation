using Microsoft.AspNetCore.Identity;

namespace ConstructionOrganisation.Models
{
    public class User : IdentityUser // Наследуемся от IdentityUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "admin", "minadmin", "readonly"
    }
}
