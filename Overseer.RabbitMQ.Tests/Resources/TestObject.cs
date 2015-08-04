using System;
using System.Collections.Generic;

namespace Overseer.RabbitMQ.Tests.Resources
{
	public class PersonExactMatch
	{
		public Guid ID { get; set; }
		public string Name { get; set; }

		public IEnumerable<Address> Addresses { get; set; } 
	}

	public class Address
	{
		public string Line1 { get; set; }
		public string PostCode { get; set; }
	}
}
