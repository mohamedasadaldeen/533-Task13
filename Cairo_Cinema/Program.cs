using Cairo_Cinema.Data;
using Microsoft.EntityFrameworkCore;

namespace Cairo_Cinema
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //Data
            var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string"
            + "'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            //Movies:

            // Life time
            // AddTransient, AddScoped, AddSingleton
            builder.Services.AddScoped<IRepository<Movie>, Repository<Movie>>();
            builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
            builder.Services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
            builder.Services.AddScoped<IRepository<Actor>, Repository<Actor>>();
            builder.Services.AddScoped<IMovieSubImageRepository, MovieSubImageRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Admin}/{controller=Movie}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
