using Microsoft.EntityFrameworkCore;
using ShopOnline.Models;

namespace ShopOnline.DataBaseContext
{
    public class DBaseContext : DbContext
    {
        public DbSet<PrincipalStruct> PrincipalStructs { get; set; }
        public DbSet<User> Users { get; set; }

        public DBaseContext(DbContextOptions<DBaseContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PrincipalStructConfiguration());
            modelBuilder.ApplyConfiguration(new SubstructConfiguration());
            modelBuilder.ApplyConfiguration(new OpinionesConfiguration());
        }
    }
}
