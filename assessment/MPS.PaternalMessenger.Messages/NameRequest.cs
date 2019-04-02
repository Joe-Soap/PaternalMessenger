using RawRabbit.Configuration.Exchange;
using RawRabbit.Enrichers.Attributes;

namespace MPS.PaternalMessenger.Messages
{
	[Exchange(Type = ExchangeType.Topic, Name = "custom.rpc.exchange")]
	[Queue(Name = "custom.name.queue", Durable = false)]
	[Routing(RoutingKey = "custom.name.key")]
	public class NameRequest
	{
		public string MessageTemplate { get; set; }
		public string Name { get; set; }
	}
}
