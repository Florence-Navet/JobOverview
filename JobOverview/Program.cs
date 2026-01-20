

using JobOverview.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace JobOverview
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string? connect = builder.Configuration.GetConnectionString("JobOverviewConnect");

            // Add services to the container.
      

         builder.Services.AddDbContext<ContexteJobOverview>(opt 
            => opt.UseSqlServer(connect));

         builder.Services.AddScoped<IServiceLogiciels, ServiceLogiciels>();

         builder.Services.AddScoped<IServiceEquipes, ServiceEquipes>();

            builder.Services.AddControllers();
         // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

         builder.Services.AddEndpointsApiExplorer();
         builder.Services.AddSwaggerGen();

         var app = builder.Build();

         // Configure the HTTP request pipeline.
         if (app.Environment.IsDevelopment())
         {
            app.UseSwagger();
            app.UseSwaggerUI();
         }

         app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
