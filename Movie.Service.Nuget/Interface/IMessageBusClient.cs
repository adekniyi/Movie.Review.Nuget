using System;
using System.Threading.Tasks;

namespace Movie.Service.Nuget.Interface
{
	public interface IMessageBusClient
	{
		void Publish(dynamic model, string routingKey);
        void Publish(Func<Task> model, string routingKey);
        void Initialize(string exchange);
    }
}

