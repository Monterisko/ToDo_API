using Microsoft.EntityFrameworkCore;
using ToDo_API.Models;

namespace ToDo_API.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<ToDo> Todos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDo>()
                .ToTable("ToDos");
        }

      
    }
}
