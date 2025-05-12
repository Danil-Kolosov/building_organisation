using Microsoft.AspNetCore.Identity;

namespace ConstructionOrganisation.Models
{
    public class User : IdentityUser // Наследуемся от IdentityUser
    {
        // Удалите дублирующие свойства, так как они уже есть в IdentityUser
        public string Role { get; set; } // "admin", "minadmin", "readonly"
    }
}
