using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client;

namespace Overseer.RabbitMQ
{
	public class Headers
	{
		private static readonly List<Getter> Map = BuildHeaderMapper();

		public static Dictionary<string, object> FromProperties(IBasicProperties properties)
		{
			var headers = Map
				.Where(g => g.HasValue(properties))
				.ToDictionary(
					g => g.Name,
					g => g.GetValue(properties));

			if (properties.IsHeadersPresent())
			{
				foreach (var pair in properties.Headers)
				{
					headers.Add(pair.Key, pair.Value);
				}
			}

			return headers;
		}

		private static List<Getter> BuildHeaderMapper()
		{
			var type = typeof(IBasicProperties);
			var methods = type.GetMethods();

			var headers = new List<Getter>();

			foreach (var property in type.GetProperties())
			{
				var checkMethod = methods.FirstOrDefault(m => m.Name == "Is" + property.Name + "Present");

				if (checkMethod != null)
				{
					var getMethod = property.GetGetMethod();

					Func<object, bool> check = input => (bool)checkMethod.Invoke(input, new object[] { });
					Func<object, object> get = input => getMethod.Invoke(input, new object[] { });

					headers.Add(new Getter
					{
						Name = property.Name,
						HasValue = check,
						GetValue = get
					});
				}
			}

			//remove custom cases
			headers.RemoveAll(g => g.Name == "Headers");

			return headers;
		}

		private struct Getter
		{
			public string Name;
			public Func<object, bool> HasValue;
			public Func<object, object> GetValue;
		}
	}
}
