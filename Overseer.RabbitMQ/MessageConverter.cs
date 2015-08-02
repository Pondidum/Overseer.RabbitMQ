using System.Collections.Generic;
using RabbitMQ.Client.Events;

namespace Overseer.RabbitMQ
{
	public class MessageConverter : IMessageConverter
	{
		public Message Convert(object input)
		{
			var message = input as BasicDeliverEventArgs;

			if (message == null)
			{
				return null;
			}

			return new Message
			{
				Headers = new Dictionary<string, object>(),
				Body = new object()
			};
		}
	}
}
