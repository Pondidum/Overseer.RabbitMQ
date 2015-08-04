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

			var headers = message.BasicProperties.Headers;
			var body = message.Body;
			var json = Encoding.UTF8.GetString(body);

			return new Message
			{
				Headers = headers,
				Body = json
			};
		}
	}
}
