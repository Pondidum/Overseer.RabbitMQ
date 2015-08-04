namespace Overseer.RabbitMQ
{
	public class RabbitOptions
	{
		public string HostName { get; set; }
		public string ExchangeName { get; set; }

		public bool ExchangeDurable { get; set; }
		public bool ExchangeAutoDelete { get; set; }

		public string RoutingKey { get; set; }

		public RabbitOptions()
		{
			ExchangeDurable = false;
			ExchangeAutoDelete = false;
			RoutingKey = "#";
		}
	}
}
