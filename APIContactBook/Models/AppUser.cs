using Microsoft.AspNetCore.Identity;

namespace ContactBookApi.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
