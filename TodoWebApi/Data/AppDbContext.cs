using Microsoft.EntityFrameworkCore;
using TodoWebApi.Models;
namespace TodoWebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoItem>()
                .HasKey(t => t.Id);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
