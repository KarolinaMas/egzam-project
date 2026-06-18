using ExamProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExamProject.Data
{
    public class ExamProjectDbContext : DbContext
    {
        public ExamProjectDbContext(DbContextOptions<ExamProjectDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(o =>
            {
                o.ToTable("user");
                o.HasKey(e => e.Id);
                o.HasIndex(e => e.UserName).IsUnique();
                o.HasIndex(e => e.Email).IsUnique();
                o.Property(e => e.PasswordHash).IsRequired();
                o.Property(e => e.Role).IsRequired();
            });

             modelBuilder.Entity<TaskItem>(o =>
            {
                o.ToTable("task");
                o.HasKey(e => e.Id);
                o.Property(e => e.Title).IsRequired().HasMaxLength(100);
                o.Property(e => e.Description).HasMaxLength(2000);
                o.Property(e => e.IsComplete).HasDefaultValue(false);
                o.Property(e => e.CreatedAt).ValueGeneratedOnAdd();
                o.HasOne(t => t.User)
                    .WithMany(u => u.Tasks)
                    .HasForeignKey(t => t.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
