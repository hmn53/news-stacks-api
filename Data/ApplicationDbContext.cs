using Microsoft.EntityFrameworkCore;
using NewsStacksAPI.Models;

namespace NewsStacksAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Writer> Writers { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Account> Accounts { get; set; }

    }
}
