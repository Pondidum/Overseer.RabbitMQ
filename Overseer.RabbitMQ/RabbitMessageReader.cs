using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Overseer.RabbitMQ
{
	public class RabbitMessageReader : IMessageReader, IDisposable
	{
		private readonly RabbitOptions _options;
		private readonly ConnectionFactory _factory;

		private IModel _channel;
		private IConnection _connection;
		private QueueDeclareOk _queueName;

		private Action _unhook;
		private Action _hook;

		public RabbitMessageReader(RabbitOptions options)
		{
			_options = options;

			_factory = new ConnectionFactory { HostName = options.HostName };
		}

		public void Start(Action<object> onMessage)
		{
			Stop();

			_connection = _factory.CreateConnection();
			_channel = _connection.CreateModel();

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
			_channel.Dispose();
			_connection.Dispose();

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

		public void Dispose()
		{
			Stop();
		}
	}
}
