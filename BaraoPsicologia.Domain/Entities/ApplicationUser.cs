using Microsoft.AspNetCore.Identity;

namespace BaraoPsicologia.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public required string Name { get; set; }
        public required string Profile { get; set; }
        public bool ReceiveEmails { get; set; }
    }
}
