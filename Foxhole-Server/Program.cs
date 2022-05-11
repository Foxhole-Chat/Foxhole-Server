using Foxhole_Server;
using Foxhole_Server.Models;

Config_Builder.Read_Config_File("Config.toml");

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);



builder.WebHost.UseUrls(Config.Host_URL);

Startup.ConfigureServices(builder.Services);

WebApplication app = builder.Build();

Startup.Configure(app, app.Environment);

PostgreSQL.Setup_Database();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/",
	defaults: new { controller = "Home" });

app.Run();