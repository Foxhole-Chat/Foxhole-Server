namespace Foxhole_Server.Models
{
	[AttributeUsage(AttributeTargets.Property)]
	public class Default_Value : Attribute
	{
		public string? Value { get; set; }
		public Default_Value(string value) { Value = value; }
	}

	[AttributeUsage(AttributeTargets.Property)]
	internal class Primary_Key : Attribute {}

	[AttributeUsage(AttributeTargets.Property)]
	public class Datatype : Attribute
	{
		public string Type { get; set; } = string.Empty;

		public Datatype(string type) { Type = type; }
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class Unique_Key : Attribute {}

	[AttributeUsage(AttributeTargets.Property)]
	public class Foreign_Key : Attribute
	{
		public string Table_Name { get; set; } = string.Empty;
		public string Key_Name { get; set; } = string.Empty;
		public Foreign_Key(string table_name, string key_name)
		{
			Table_Name = table_name;
			Key_Name = key_name;
		}
	}




	public class Burrow
	{
		[Datatype("uuid")]
		[Primary_Key]
		[Unique_Key]
		[Default_Value("gen_random_uuid()")]
		public Guid ID { get; set; }

		[Datatype("text")]
		public string Name { get; set; } = string.Empty;
	}
	public class Room
	{
		[Datatype("uuid")]
		[Primary_Key]
		[Unique_Key]
		[Default_Value("gen_random_uuid()")]
		public Guid ID { get; set; }

		[Datatype("uuid")]
		[Foreign_Key("Burrows", "ID")]
		public Guid Burrow_ID { get; set; }

		[Datatype("text")]
		public string Name { get; set; } = string.Empty;
	}
	public class Category
	{
		[Datatype("uuid")]
		[Primary_Key]
		[Unique_Key]
		[Default_Value("gen_random_uuid()")]
		public Guid ID { get; set; }

		[Datatype("uuid")]
		[Foreign_Key("Rooms", "ID")]
		public Guid Room_ID { get; set; }

		[Datatype("text")]
		public string Name { get; set; } = string.Empty;
	}
}
