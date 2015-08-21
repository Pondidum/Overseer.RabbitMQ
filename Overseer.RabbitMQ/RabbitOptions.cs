namespace Overseer.RabbitMQ
{
	public class RabbitOptions
	{
		/// <summary>The RabbitMQ host to connect to</summary>
		public string HostName { get; set; }

		/// <summary>The exchange to bind a queue to</summary>
		public string ExchangeName { get; set; }

		/// <summary>Whether the exchange is durable (e.g. will survive a RabbitMQ restart).  Defaults to `true`</summary>
		public bool ExchangeDurable { get; set; }

		/// <summary>Whether to delete the exchange when all queues bound to it have been removed.  Defaults to `false`</summary>
		public bool ExchangeAutoDelete { get; set; }

		/// <summary>Prevents `.ExchangeDeclare()` from being called.  Defaults to `false`</summary>
		public bool ExchangeSkipDeclare { get; set; }

		/// <summary>The routing key to use to filter messages. Defaults to "#"</summary>
		public string RoutingKey { get; set; }

		public RabbitOptions()
		{
			ExchangeDurable = true;
			ExchangeAutoDelete = false;
			ExchangeSkipDeclare = false;

			RoutingKey = "#";
		}
	}
}
