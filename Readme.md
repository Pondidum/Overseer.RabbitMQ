# Overseer.RabbitMQ

Provides message reading and conversion for a RabbitMQ source.

It assumes you have an exchange of type `Topic`, and will bind a temporary queue to this exchange to read messages with.  Exchange Name, Durability and AutoDelete can all be configured, and setting SkipExchangeDeclare will force no `.ExchangeDeclare()` call to be made.

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

## Options

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
Whether the exchange is durable (e.g. will surivive a RabbitMQ restart).  Defaults to `true`.
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
