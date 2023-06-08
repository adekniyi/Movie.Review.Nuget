using System;

namespace Movie.Service.Nuget.Interface
{
	public interface IMessageBusClient
	{
		void Publish(dynamic model, string routingKey);
		void Initialize(string exchange);
    }
}

