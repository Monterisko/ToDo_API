using Microsoft.EntityFrameworkCore;
using ToDo_API.Models;

namespace ToDo_API.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {

        public DbSet<ToDo> ToDos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDo>()
                .ToTable("ToDos");
        }

      
    }
}
