using backend.src.Task;
using backend.src.ApplicationUser;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.src.Notification;

namespace backend.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<NotificationEntity> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TaskEntity configurations
            modelBuilder.Entity<TaskEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.TaskStatus).HasConversion<string>();

                // Many-to-many: Task <-> Users (Assignees)
                entity.HasMany(e => e.Assignees)
                      .WithMany(u => u.Tasks)
                      .UsingEntity("TaskAssignees");

                // One-to-many: Task -> User (CreatedBy) - EF Core otomatik foreign key oluşturacak
                entity.HasOne(e => e.CreatedBy)
                      .WithMany()
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // AppUser configurations
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UserRole).HasConversion<string>();
            });

            // NotificationEntity configurations
            modelBuilder.Entity<NotificationEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
                entity.Property(e => e.IsRead).HasDefaultValue(false);

                entity.HasOne(e => e.Recipient)
                      .WithMany()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.RelatedTask)
                      .WithMany()
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}