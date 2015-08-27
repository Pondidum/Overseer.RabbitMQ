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

			var headers = Headers.FromProperties(message.BasicProperties);
			var body = message.Body ?? new byte[0];
			var json = Encoding.UTF8.GetString(body);

			return new Message
			{
				Type = message.BasicProperties.Type,
				Headers = headers,
				Body = json
			};
		}
	}
}
