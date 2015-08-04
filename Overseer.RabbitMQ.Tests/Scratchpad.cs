using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Overseer.RabbitMQ.Tests
{
	public class Scratchpad
	{
		private readonly ITestOutputHelper _output;

		public Scratchpad(ITestOutputHelper output)
		{
			_output = output;
		}

		[Fact]
		public void When_testing_something()
		{
			var wait = new AutoResetEvent(false);

			var converter = new RabbitMessageConverter();
			var rabbit = new RabbitMessageReader(new RabbitOptions
			{
				HostName = "192.168.59.103",
				ExchangeName = "DomainEvents"
			});

			rabbit.Start(m =>
			{
				_output.WriteLine(converter.Convert(m).Body);
				wait.Set();
			});

			var publisher = new Publisher("192.168.59.103", "DomainEvents");

			publisher.Send("candidate.created", new DomainMessage { Action = "Testing" });

			wait.WaitOne();
			rabbit.Stop();
		}

		public class DomainMessage
		{
			public string Action { get; set; }
		}
	}

}
