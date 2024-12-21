using Microsoft.EntityFrameworkCore;
using ToDo_API.Models;

namespace ToDo_API.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<ToDo> Todos { get; set; }

        // The OnModelCreating method is used to configure the model using modelBuilder
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // // Configures the ToDo entity to correspond to the “ToDos” table in the database
            modelBuilder.Entity<ToDo>()
                .ToTable("ToDos");
        }

      
    }
}
