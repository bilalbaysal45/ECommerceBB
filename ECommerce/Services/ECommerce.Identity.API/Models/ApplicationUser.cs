using Microsoft.AspNetCore.Identity;

namespace ECommerce.Identity.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
