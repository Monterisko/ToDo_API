using Microsoft.EntityFrameworkCore;
using ToDo_API.Data;
using ToDo_API.Interfaces;
using ToDo_API.Repository;

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
            builder.Services.AddScoped<IToDoRepository, ToDoRepository>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
     
    }
}
