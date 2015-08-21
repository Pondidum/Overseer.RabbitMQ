using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Overseer.RabbitMQ
{
	public class RabbitMessageReader : IMessageReader
	{
		private readonly RabbitOptions _options;
		private readonly IModel _channel;
		private QueueDeclareOk _queueName;

		private Action _unhook;
		private Action _hook;

		public RabbitMessageReader(RabbitOptions options)
		{
			_options = options;

			var factory = new ConnectionFactory { HostName = options.HostName };
			var connection = factory.CreateConnection();

			_channel = connection.CreateModel();
		}

		public void Start(Action<object> onMessage)
		{
			Stop();
			DeclareExchangeIfRequired();

			var consumer = CreateConsumer(onMessage);

			_queueName = _channel.QueueDeclare();

			_channel.QueueBind(_queueName, _options.ExchangeName, _options.RoutingKey);
			_channel.BasicConsume(_queueName, false, consumer);

			_hook();
		}

		public void Stop()
		{
			if (_unhook == null)
				return;

			_channel.QueueUnbind(_queueName, _options.ExchangeName, _options.RoutingKey, null);
			_unhook();

			_hook = null;
			_unhook = null;
		}

		private void DeclareExchangeIfRequired()
		{
			if (_options.ExchangeSkipDeclare)
				return;

			_channel.ExchangeDeclare(
				_options.ExchangeName,
				ExchangeType.Topic,
				_options.ExchangeDurable,
				_options.ExchangeAutoDelete,
				null);
		}

		private EventingBasicConsumer CreateConsumer(Action<object> onMessage)
		{
			var handler = new EventHandler<BasicDeliverEventArgs>((o, e) => onMessage(e));
			var consumer = new EventingBasicConsumer(_channel);

			_hook = () => consumer.Received += handler;
			_unhook = () => consumer.Received -= handler;

			return consumer;
		}
	}
}
