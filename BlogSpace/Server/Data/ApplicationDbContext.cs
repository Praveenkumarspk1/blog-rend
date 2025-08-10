using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlogSpace.Shared.Models;

namespace BlogSpace.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Post entity
            builder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Summary).HasMaxLength(500);
                entity.Property(e => e.Visibility).IsRequired();
                entity.Property(e => e.TagsString).HasColumnName("Tags");
                entity.HasOne(e => e.Author)
                      .WithMany(e => e.Posts)
                      .HasForeignKey(e => e.AuthorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Follow entity
            builder.Entity<Follow>(entity =>
            {
                entity.HasKey(e => new { e.FollowerId, e.FollowingId });
                entity.HasOne(e => e.Follower)
                      .WithMany(e => e.Following)
                      .HasForeignKey(e => e.FollowerId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Following)
                      .WithMany(e => e.FollowedBy)
                      .HasForeignKey(e => e.FollowingId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Notification entity
            builder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Message).HasMaxLength(500);
                entity.HasOne(e => e.User)
                      .WithMany(e => e.Notifications)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.RelatedUser)
                      .WithMany()
                      .HasForeignKey(e => e.RelatedUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
} 