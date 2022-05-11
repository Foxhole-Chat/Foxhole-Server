using Microsoft.EntityFrameworkCore;

namespace Foxhole_Server.Models
{
	public class Burrow_Context : DbContext
	{
		public DbSet<Burrow>? Burrows { get; set; }
		public DbSet<Room>? Rooms { get; set; }
		public DbSet<Category>? Categories { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options_builder)
			=> options_builder.UseNpgsql("Host=" + Config.PostgreSQL.Server_Address + ';' +
				"Database=Foxhole;" +
				"Username=" + Config.PostgreSQL.API_Provider_Username + ';' +
				"Password=" + Config.PostgreSQL.API_Provider_Password + ';');

		protected override void OnModelCreating(ModelBuilder model_builder)
		{
			model_builder.HasDefaultSchema("Server");
		}

		public Burrow_Context(DbContextOptions<Burrow_Context> options) : base(options) { }
	}
}
