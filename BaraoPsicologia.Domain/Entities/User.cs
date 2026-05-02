using Microsoft.AspNetCore.Identity;

namespace BaraoPsicologia.Domain.Entities
{
    public class User : IdentityUser
    {
        public required string Name { get; set; }
        public required string Profile { get; set; }
    }
}
