using Microsoft.EntityFrameworkCore;
using SEP3_Tier3.Models;

namespace SEP3_T3.Persistance
{
    public class ShapeAppDbContext : DbContext
    {
      //  public DbSet<Address> Addresses { get; set; }
      public DbSet<User> Users { get; set; }
      public DbSet<Administrator> Administrators { get; set; }
      public DbSet<Post> Posts { get; set; }
      public DbSet<Message> Messages { get; set; }
      public DbSet<PostAction> PostActions { get; set; }
      public DbSet<Friendship> Friendships { get; set; }
      public DbSet<UserAction> UserActions { get; set; }
      public DbSet<PageRating> PageRatings { get; set; }
      public DbSet<TrainingExercise> TrainingExercises { get; set; }
      public DbSet<DietMeal> DietMeals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;port=3606;database=sep3;user=root;password=29312112");
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PostAction>()
                .HasKey(pa => new
                {
                    pa.PostId,
                    pa.UserId
                });
            modelBuilder.Entity<PostAction>()
                .HasOne(pa => pa.Post).WithMany();
            modelBuilder.Entity<PostAction>()
                .HasOne(pa => pa.User).WithMany();
            
            modelBuilder.Entity<Friendship>()
                .HasKey(f => new
                {
                    f.FirstUserId,
                    f.SecondUserId
                });
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.FirstUser).WithMany();
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.SecondUser).WithMany();
            
            modelBuilder.Entity<UserAction>()
                .HasKey(ua => new
                {
                    ua.SenderId,
                    ua.ReceiverId
                });
            modelBuilder.Entity<UserAction>()
                .HasOne(ua => ua.Sender).WithMany();
            modelBuilder.Entity<UserAction>()
                .HasOne(ua => ua.Receiver).WithMany();
            
            modelBuilder.Entity<Message>()
                .HasKey(message => new
                {
                    message.Id
                });
            modelBuilder.Entity<Message>()
                .HasOne(message => message.Sender).WithMany();
            modelBuilder.Entity<Message>()
                .HasOne(message => message.Receiver).WithMany();
            
            modelBuilder.Entity<PageRating>()
                .HasKey(pr => new
                {
                    pr.PageId,
                    pr.UserId
                });
            modelBuilder.Entity<PageRating>()
                .HasOne(pr => pr.Page).WithMany();
            modelBuilder.Entity<PageRating>()
                .HasOne(pr => pr.User).WithMany();

            modelBuilder.Entity<TrainingExercise>()
                .HasKey(te => new
                {
                    te.TrainingId,
                    te.ExerciseId
                });
            modelBuilder.Entity<TrainingExercise>()
                .HasOne(te => te.Training).WithMany();
            modelBuilder.Entity<TrainingExercise>()
                .HasOne(te => te.Exercise).WithMany();

            modelBuilder.Entity<DietMeal>()
                .HasKey(dm => new
                {
                    dm.DietId,
                    dm.MealId
                });
            modelBuilder.Entity<DietMeal>()
                .HasOne(dm => dm.Diet).WithMany();
            modelBuilder.Entity<DietMeal>()
                .HasOne(dm => dm.Meal).WithMany();


            // modelBuilder.Entity<Administrator>(entity =>
            // {
            //     entity.HasKey(e => e.Id);
            //     entity.Property(e => e.Email).IsRequired();
            //     entity.Property(e => e.Password).IsRequired();
            // });
            //
            // modelBuilder.Entity<Address>(entity =>
            // {
            //     entity.HasKey(e => e.Id);
            //     entity.Property(e => e.Street).IsRequired();
            //     entity.Property(e => e.Number).IsRequired();
            //
            // });
            //
            // modelBuilder.Entity<RegularUser>(entity =>
            // {
            //     entity.HasKey(e => e.Id);
            //     entity.Property(e => e.Email).IsRequired();
            //     entity.Property(e => e.Password).IsRequired();
            //     entity.Property(e => e.Name).IsRequired();
            //     entity.Property(e => e.Description).IsRequired();
            //     entity.Property(e => e.City).IsRequired();
            //     entity.Property<string>("AvatarPath").IsRequired();
            // });
            //
            // modelBuilder.Entity<Post>(entity =>
            // {
            //     entity.HasKey(e => e.Id);
            //     entity.Property(e => e.Content);
            //     // entity.Property(e => e.Likes);
            //     // entity.HasOne(u => u.Owner)
            //     //     .WithMany(p => p.Posts);
            // });
        }  
    }
}