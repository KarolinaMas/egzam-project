using ExamProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExamProject.Data
{
    public class ExamProjectDbContext : DbContext
    {
        public ExamProjectDbContext(DbContextOptions<ExamProjectDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

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
        }
    }
}
