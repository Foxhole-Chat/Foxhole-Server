using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;
using System.Reflection.Emit;

namespace Foxhole_Server.Models
{
	public class Burrow_Context : DbContext
	{
		public DbSet<Burrow> Burrows { get; set; }
		public DbSet<Room> Rooms { get; set; }
		public DbSet<Category> Categories { get; set; }

		public DbSet<Channel> Channels { get; set; }

		public DbSet<Message> Messages { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder options_builder)
			=> options_builder.UseNpgsql("Host=" + Config.PostgreSQL.Server_Address + ';' +
				"Database=Foxhole;" +
				"Username=" + Config.PostgreSQL.API_Provider_Username + ';' +
				"Password=" + Config.PostgreSQL.API_Provider_Password + ';');

		protected override void OnModelCreating(ModelBuilder model_builder)
		{
			model_builder.HasDefaultSchema("Server");

			IEnumerable<PropertyInfo> burrow_context_DbSets = typeof(Burrow_Context).GetProperties().Where(property => property.PropertyType.Name[..5] == "DbSet");

			foreach (PropertyInfo dbset in burrow_context_DbSets)
			{

				Type table = dbset.PropertyType.GetGenericArguments()[0];
				PropertyInfo[] table_properties = table.GetProperties();

				foreach (PropertyInfo property in table_properties)
				{
					IEnumerable<Attribute> property_attributes = property.GetCustomAttributes();

					if (((Datatype)property_attributes.Single(attribute => attribute.GetType() == typeof(Datatype))).Type.ToLower() == "uuid" &&
						property.PropertyType == typeof(string))
					{
						model_builder
							.Entity(table)
							.Property(property.Name)
							.HasConversion
							(
								new ValueConverter<string, Guid>(value => new Guid(Convert.FromBase64String(value)),
									value => Convert.ToBase64String(value.ToByteArray()))
							);
					}
				}
			}
		}

		public Burrow_Context(DbContextOptions<Burrow_Context> options) : base(options)
		{
			Burrows = Set<Burrow>();
			Rooms = Set<Room>();
			Categories = Set<Category>();
			Channels = Set<Channel>();
			Messages = Set<Message>();
		}
	}
}
