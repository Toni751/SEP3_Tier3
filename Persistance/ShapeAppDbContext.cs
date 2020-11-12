using Microsoft.EntityFrameworkCore;
using SEP3_T3.Models;

namespace SEP3_T3.Persistance
{
    public class ShapeAppDbContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        
        public DbSet<RegularUser> Users { get; set; }
        
        public DbSet<Administrator> Administrators { get; set; }
        
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;port=3606;database=sep3;user=root;password=29312112");
        }
//idk
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Password).IsRequired();
            });
            
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Street).IsRequired();
                entity.Property(e => e.Number).IsRequired();

            });

            modelBuilder.Entity<RegularUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.AccountType).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.City).IsRequired();
                entity.Property<string>("AvatarPath").IsRequired();
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content);
                // entity.Property(e => e.Likes);
                // entity.HasOne(u => u.Owner)
                //     .WithMany(p => p.Posts);
            });
        }  
    }
}