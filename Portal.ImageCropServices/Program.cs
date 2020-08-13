using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Portal.ImageCropServices.Data;
using System;

namespace Portal.ImageCropServices
{
   public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddScoped<IScopedProcessingService, ScopedProccesingService>();
                    services.AddDbContext<ImageDbContext>(options =>
                options.UseSqlServer("Data Source=.;Initial Catalog=Nova_ImageDb;Integrated Security=True"));
                });
    }
}
