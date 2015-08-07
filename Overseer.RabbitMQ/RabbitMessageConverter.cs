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
		public Message Convert(object input)
		{
			var message = input as BasicDeliverEventArgs;

			if (message == null)
			{
				return null;
			}

			var headers = ConvertHeaders(message.BasicProperties);

			var body = message.Body ?? new byte[0];
			var json = Encoding.UTF8.GetString(body);

			return new Message
			{
				Headers = headers,
				Body = json
			};
		}

		private Dictionary<string, object> ConvertHeaders(IBasicProperties properties)
		{
			var type = properties.GetType();
			var methods = type.GetMethods();

			var headers = new Dictionary<string, object>();

			foreach (var property in type.GetProperties())
			{
				var check = methods.FirstOrDefault(m => m.Name == "Is" + property.Name + "Present");

				if (check != null)
				{
					if ((bool) check.Invoke(properties, Type.EmptyTypes))
					{
						headers[property.Name] = property.GetGetMethod().Invoke(properties, Type.EmptyTypes);
					}
				}
			}

			return headers;
		}
	}
}
