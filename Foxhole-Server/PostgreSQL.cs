using Foxhole_Server.Models;
using Npgsql;
using System.Reflection;

namespace Foxhole_Server
{
	public class PostgreSQL
	{
		public static void Colored_Write(string output, ConsoleColor color)
		{
			Console.ForegroundColor = color;

			Console.Write(output);

			Console.ResetColor();
		}

		public static async void Setup_Database()
		{
			bool missing_parent = false;

			using (NpgsqlConnection connection = new("Host=" + Config.PostgreSQL.Server_Address + ';' +
						"Username=" + Config.PostgreSQL.API_Provider_Username + ';' +
						"Password=" + Config.PostgreSQL.API_Provider_Password + ';'))
			{
				await connection.OpenAsync();

				bool database_exists;

				await using (NpgsqlCommand cmd = new("SELECT EXISTS(SELECT FROM \"pg_catalog\".\"pg_database\" WHERE \"datname\" = 'Foxhole');", connection))
				{
					await using var reader = await cmd.ExecuteReaderAsync();
					await reader.ReadAsync();
					database_exists = reader.GetBoolean(0);
				}

				if (!database_exists)
				{
					await using (NpgsqlCommand cmd = new("CREATE DATABASE \"Foxhole\";", connection)) { await cmd.ExecuteNonQueryAsync(); }

					if (!missing_parent)
					{
						Colored_Write("Warning: ", ConsoleColor.DarkYellow);

						Console.WriteLine($"Database \"Foxhole\" does not exist...\n" +
							"	Created new database.");

						missing_parent = true;
					}
				}

				await connection.CloseAsync();
			}

			using (NpgsqlConnection connection = new("Host=" + Config.PostgreSQL.Server_Address + ';' +
				"Database=Foxhole;" +
				"Username=" + Config.PostgreSQL.API_Provider_Username + ';' +
				"Password=" + Config.PostgreSQL.API_Provider_Password + ';'))
			{
				await connection.OpenAsync();

				IEnumerable<PropertyInfo> burrow_context_DbSets = typeof(Burrow_Context).GetProperties().Where(property => property.PropertyType.Name[..5] == "DbSet");

				await using (NpgsqlCommand cmd = new("CREATE OR REPLACE FUNCTION pg_temp.Ensure_Element_Exists(" +
							"Elements TEXT," +
							"Query TEXT) " +
					"RETURNS boolean AS $$ " +
					"DECLARE element_exists boolean DEFAULT FALSE;" +
					"BEGIN " +
						"EXECUTE 'SELECT Exists(SELECT FROM ' || Elements || ');' INTO element_exists;" +

						"IF NOT element_exists THEN " +
							$"EXECUTE Query;" +
						"END IF;" +

						"RETURN(element_exists);" +
					"END $$ LANGUAGE PLPGSQL;", connection)
					) { await cmd.ExecuteNonQueryAsync(); }


				bool missing_schema = missing_parent;

				await using (NpgsqlCommand cmd = new("SELECT pg_temp.Ensure_Element_Exists(" +
					"'\"information_schema\".\"schemata\" WHERE \"schema_name\" = ''Server'''," +
					"'CREATE SCHEMA \"Server\"');", connection))
				{

					await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

					await reader.ReadAsync();

					bool schema_exists = reader.GetBoolean(0);

					if (!schema_exists && !missing_schema)
					{
						Colored_Write("Warning: ", ConsoleColor.DarkYellow);

						Console.WriteLine("Schema \"Server\" does not exist...\n" +
							"	Created new schema.");

						missing_schema = true;
					}
				};



				foreach (PropertyInfo dbset in burrow_context_DbSets)
				{

					dbset.GetType().GetGenericArguments();

					Type table = dbset.PropertyType.GetGenericArguments()[0];
					PropertyInfo[] table_properties = table.GetProperties();

					string table_arguments = string.Empty;
					string table_constraints = string.Empty;

					foreach (PropertyInfo property in table_properties)
					{
						table_arguments += '\"' + property.Name + '\"';

						IEnumerable<Attribute> property_attributes = property.GetCustomAttributes();

						table_arguments += ' ' + ((Datatype)property_attributes.Single(attribute => attribute.GetType() == typeof(Datatype))).Type;

						if (Nullable.GetUnderlyingType(property.GetType()) == null) { table_arguments += " NOT NULL"; }

						foreach (Attribute property_attribute in property_attributes)
						{
							Type property_attribute_type = property_attribute.GetType();

							if (property_attribute_type == typeof(Default_Value)) { table_arguments += " DEFAULT " + ((Default_Value)property_attribute).Value; }

							if (property_attribute_type == typeof(Primary_Key)) { table_constraints += $"CONSTRAINT \"{dbset.Name}_pkey\" PRIMARY KEY (\"{property.Name}\"),"; }

							if (property_attribute_type == typeof(Unique_Key)) { table_constraints += $"CONSTRAINT \"{dbset.Name}_Unique_{property.Name}\" UNIQUE (\"{property.Name}\"),"; }

							if (property_attribute_type == typeof(Foreign_Key))
							{
								Foreign_Key foreign_key = (Foreign_Key)property_attribute;

								string table_name = foreign_key.Table_Name;
								string key_name = foreign_key.Key_Name;



								table_constraints += $"CONSTRAINT \"{dbset.Name}_{property.Name}\" FOREIGN KEY (\"{property.Name}\") " +
									$"REFERENCES \"Server\".\"{table_name}\" (\"{key_name}\"),";
							}
						}
						table_arguments += ',';
					}

					table_arguments = (table_arguments + table_constraints)[0..^1];

					await using NpgsqlCommand cmd = new("SELECT pg_temp.Ensure_Element_Exists(Format('\"information_schema\".\"tables\" WHERE \"table_schema\" = ''Server'' AND \"table_name\" = %L', $1)," +
						"Format('CREATE TABLE \"Server\".%I (%s)', $1, $2));", connection);

					cmd.Parameters.AddWithValue(dbset.Name);
					cmd.Parameters.AddWithValue(table_arguments);

					await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
					await reader.ReadAsync();

					bool table_exists = reader.GetBoolean(0);


					if (!table_exists && !missing_schema)
					{
						Colored_Write("Warning: ", ConsoleColor.DarkYellow);

						Console.WriteLine($"Table \"Server\".\"{dbset.Name}\" does not exist...\n" +
							"	Created new table.");
					}
				}
				await connection.CloseAsync();
			}
		}
	}
}
