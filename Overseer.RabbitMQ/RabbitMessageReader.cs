using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Overseer.RabbitMQ
{
	public class RabbitMessageReader : IMessageReader
	{
		private readonly string _exchange;
		private readonly IModel _channel;
		private readonly QueueDeclareOk _queueName;
		private Action _unhook;

		public RabbitMessageReader(string hostName, string exchangeName)
		{
			_exchange = exchangeName;

			var factory = new ConnectionFactory { HostName = hostName };
			var connection = factory.CreateConnection();

			_channel = connection.CreateModel();
			_queueName = _channel.QueueDeclare();
		}

		public void Start(Action<object> onMessage)
		{
			var handler = new EventHandler<BasicDeliverEventArgs>((o, e) => onMessage(e));
			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += handler;
			_unhook = () => consumer.Received -= handler;

			_channel.ExchangeDeclare(_exchange, ExchangeType.Topic);

			var queueName = _channel.QueueDeclare();

			_channel.QueueBind(queueName, _exchange, "#");	// # = all
			_channel.BasicConsume(queueName, false, consumer);
		}

		public void Stop()
		{
			_channel.QueueUnbind(_queueName, _exchange, "#", null);
			_unhook();
		}
	}
}
