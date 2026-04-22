using Newtonsoft.Json;
using partycli.Models;
using System;
using System.Collections.Generic;

namespace partycli
{
	public static class Helpers
	{
		public static string ProccessName(string name)
		{
			name = name.Replace("-", string.Empty);
			return name;
		}

		public static void DisplayList(string serverListString)
		{
			var serverlist = JsonConvert.DeserializeObject<List<ServerModel>>(serverListString);
			Console.WriteLine("Server list: ");

			for (var index = 0; index < serverlist.Count; index++)
			{
				Console.WriteLine("Name: " + serverlist[index].Name);
			}

			Console.WriteLine("Total servers: " + serverlist.Count);
		}
	}
}
