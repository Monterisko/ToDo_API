using Microsoft.EntityFrameworkCore;
using ToDo_API.Data;

namespace ToDo_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Connect to Database

            builder.Services.AddDbContext<DataContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnectionString");
                options.UseNpgsql(connectionString);
            });

            // Add services to the container.
            builder.Services.AddControllers();
            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            using (var context = scope.ServiceProvider.GetService<DataContext>())
            {
                if (context != null)
                {
                    context.Database.EnsureCreated();
                }
            }

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
     
    }
}
