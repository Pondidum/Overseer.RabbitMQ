using System;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using Shouldly;
using Xunit;

namespace Overseer.RabbitMQ.Tests
{
	public class RabbitMessageConverterTests
	{
		private readonly BasicDeliverEventArgs _message;
		private readonly RabbitMessageConverter _converter;

		public RabbitMessageConverterTests()
		{
			_message = new BasicDeliverEventArgs();
			_message.BasicProperties = new BasicProperties();

			_converter = new RabbitMessageConverter();
		}

		[Fact]
		public void When_there_is_a_correlation_id()
		{
			var correlationId = Guid.NewGuid().ToString();
			_message.BasicProperties.CorrelationId = correlationId;

			var result = _converter.Convert(_message);

			result.Headers["CorrelationId"].ShouldBe(correlationId);
		}

		[Fact]
		public void When_there_isnt_a_correlation_id()
		{
			var result = _converter.Convert(_message);

			result.Headers.ShouldNotContainKey("CorrelationId");
		}
	}
}
