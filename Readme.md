# Overseer.RabbitMQ

Provides message reading and conversion for a RabbitMQ source.

## Installation

```powershell
PM> install-package Overseer.RabbitMQ
```
## Configuration

```csharp
var reader = new RabbitMessageReader(new RabbitOptions
{
  HostName = "192.168.59.103",
  ExchangeName = "DomainEvents"
});

//...
var monitor = new QueueMonitor(reader, new RabbitMessageConverter(), validationSource, output);
```

# RabbitMessageReader

The reader assumes you have an exchange of type `Topic`, and will bind a temporary queue to this exchange to read messages with.  Exchange Name, Durability and AutoDelete can all be configured, and setting SkipExchangeDeclare will force no `.ExchangeDeclare()` call to be made at all.

# RabbitOptions

The configuration options for `RabbitMessageReader`

### HostName
The RabbitMQ host to connect to.
```csharp
options.HostName = "192.168.59.103";
```

### ExchangeName
The exchange to bind a queue to.
```csharp
.ExchangeName = "DomainEvents";
```

### RoutingKey
The routing key to use to filter messages. Defaults to `#`.
```csharp
options.RoutingKey = "#"; //all messages
options.RoutingKey = "People.*";
```

### ExchangeDurable
Whether the exchange is durable (e.g. will survive a RabbitMQ restart).  Defaults to `true`.
```csharp
options.ExchangeDurable = true;
```

### ExchangeAutoDelete
Whether to delete the exchange when all queues bound to it have been removed.  Defaults to `false`.
```csharp
options.ExchangeAutoDelete = false;
```

### ExchangeSkipDeclare
Prevents `.ExchangeDeclare()` from being called.  Defaults to `false`.  When set to `true`, `ExchangeDurable`, `ExchangeAutoDelete` values are ignored.
```csharp
options.ExchangeSkipDeclare = false;
```

# RabbitMessageConverter

The converter assumes two things: that the message body is a `UTF8 Byte Array` (or null), which it will convert to a string representation; and that the `Type` property on the `BasicDeliverEventArgs` is populated with the message type (which is used to locate the correct `validationSource`).

If you need to do conversion differently, re-implementing the `RabbitMessageConverter` is very straight forward.  See [the source][message-converter] of it for more details.



[message-converter]: https://github.com/Pondidum/Overseer.RabbitMQ/blob/master/Overseer.RabbitMQ/RabbitMessageConverter.cs
