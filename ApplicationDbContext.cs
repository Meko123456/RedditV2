using Microsoft.EntityFrameworkCore;
using Reddit.Models;

namespace Reddit
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Community> Communities { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):
            base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>()
                          .HasOne(c => c.Post)
                          .WithMany(p => p.Comments)
                          .HasForeignKey(c => c.PostId)
                          .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>(p =>
            {
                p.HasMany(p => p.Comments)
                 .WithOne(c => c.Post)
                 .HasForeignKey(c => c.PostId)
                 .OnDelete(DeleteBehavior.Cascade);

                p.HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);
            });
            
            modelBuilder.Entity<Community>(entity =>
            {
                entity.HasOne(c => c.Owner)
                      .WithMany(u => u.OwnedCommunities)
                      .HasForeignKey(e => e.OwnerId)
                      .OnDelete(DeleteBehavior.SetNull); // on delete


                entity.HasMany(c => c.Subscribers)
                      .WithMany(u => u.SubscribedCommunities);
            });


            // Comment entity configuration
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Content)
                    .IsRequired();

                // One-to-many relationship between User and Comment (Author)
                entity.HasOne(c => c.Author)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevents cascade delete of User deleting comments
            });

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                // Initialize collections
                entity.HasMany(u => u.Posts)
                    .WithOne(p => p.Author)
                    .HasForeignKey(p => p.AuthorId);

                entity.HasMany(u => u.Comments)
                    .WithOne(c => c.Author)
                    .HasForeignKey(c => c.AuthorId);
            });


            base.OnModelCreating(modelBuilder);
        }

    }
}
