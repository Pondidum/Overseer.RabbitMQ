using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Overseer.RabbitMQ.Tests
{
	public class Publisher
	{
		private readonly string _exchange;
		private readonly ConnectionFactory _connectionFactory;

		public Publisher(string hostName, string exchange)
		{
			_exchange = exchange;
			_connectionFactory = new ConnectionFactory { HostName = hostName };
		}

		public void Send(string topic, object message)
		{
			var json = JsonConvert.SerializeObject(message);
			var body = Encoding.UTF8.GetBytes(json);

			using (var connection = _connectionFactory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(_exchange, ExchangeType.Topic);

				IBasicProperties basicProperties = channel.CreateBasicProperties();

				channel.BasicPublish(_exchange, topic, basicProperties, body);

			}
		}
	}
}
