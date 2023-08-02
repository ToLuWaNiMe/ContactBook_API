
using ContactBookApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContactBookApi
{
    public class ContactApiContext : IdentityDbContext<AppUser>
    {
        public ContactApiContext(DbContextOptions<ContactApiContext> options) : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }



    }
}
