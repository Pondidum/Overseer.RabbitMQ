using System.Collections.Generic;
using System.Text;
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

			var headers = new Dictionary<string, object>();

			if (message.BasicProperties.IsCorrelationIdPresent())
			{
				headers["CorrelationId"] = message.BasicProperties.CorrelationId;
			}

			
			var body = message.Body ?? new byte[0];
			var json = Encoding.UTF8.GetString(body);

			return new Message
			{
				Headers = headers,
				Body = json
			};
		}
	}
}
