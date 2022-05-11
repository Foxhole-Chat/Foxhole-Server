using Foxhole_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Foxhole_Server
{
	public class Startup
	{
		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers().AddNewtonsoftJson();

			services.AddDbContext<Burrow_Context>(optionsbuilder =>
				optionsbuilder.UseNpgsql());
		}


		public static void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
		{
			environment.ApplicationName = "Foxhole Server";

			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
				Path.Combine(Directory.GetCurrentDirectory())),
				RequestPath = "/wwwroot"
			});

			app.Use(async (context, next) =>
			{
				await next();

				if (context.Response.StatusCode == 404)
				{
					context.Request.Path = "/Exception/404";
					await next();
				}
			});

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();
		}
	}
}
