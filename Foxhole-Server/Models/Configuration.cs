namespace Foxhole_Server.Models
{
	public static class Config
	{
		public static string Host_URL { get; set; } = "https://[::]:443;http://[::]:80;https://0.0.0.0:443;http://0.0.0.0:80;";

		public static class PostgreSQL
		{
			public static string Server_Address { get; set; } = "127.0.0.1:5432";
			public static string API_Provider_Username { get; set; } = string.Empty;
			public static string API_Provider_Password { get; set; } = string.Empty;

		}
	}
}
