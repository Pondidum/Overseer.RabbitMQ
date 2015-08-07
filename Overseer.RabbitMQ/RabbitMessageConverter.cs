using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Overseer.RabbitMQ
{
	public class RabbitMessageConverter : IMessageConverter
	{
		private readonly IEnumerable<Getter> _headerMapper;

		public RabbitMessageConverter()
		{
			_headerMapper = BuildHeaderMapper();
		}

		public Message Convert(object input)
		{
			var message = input as BasicDeliverEventArgs;

			if (message == null)
			{
				return null;
			}

			var headers = _headerMapper
				.Where(g => g.HasValue(message.BasicProperties))
				.ToDictionary(
					g => g.Name,
					g => g.GetValue(message.BasicProperties));

			if (message.BasicProperties.IsHeadersPresent())
			{
				foreach (var pair in message.BasicProperties.Headers)
				{
					headers.Add(pair.Key, pair.Value);
				}
			}

			var body = message.Body ?? new byte[0];
			var json = Encoding.UTF8.GetString(body);

			return new Message
			{
				Headers = headers,
				Body = json
			};
		}

		private static IEnumerable<Getter> BuildHeaderMapper()
		{
			var type = typeof (IBasicProperties);
			var methods = type.GetMethods();

			var headers = new List<Getter>();

			foreach (var property in type.GetProperties())
			{
				var checkMethod = methods.FirstOrDefault(m => m.Name == "Is" + property.Name + "Present");

				if (checkMethod != null)
				{
					var getMethod = property.GetGetMethod();

					Func<object, bool> check = input => (bool) checkMethod.Invoke(input, Type.EmptyTypes);
					Func<object, object> get = input => getMethod.Invoke(input, Type.EmptyTypes);

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
