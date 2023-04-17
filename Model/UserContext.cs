using Microsoft.EntityFrameworkCore;

namespace ApiAton.Model
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=User;TrustServerCertificate=True;Integrated Security=True;Pooling=True");
        }
    }
}
