using System;
namespace cortoespana
{
	public class Festivals
	{
		[Newtonsoft.Json.JsonProperty("Id")]
		public string Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
	}
}
