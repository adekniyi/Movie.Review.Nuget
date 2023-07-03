using System;
using System.Threading.Tasks;

namespace Movie.Service.Nuget.Interface
{
	public interface IEventProcessor
	{
		void ProcessEvent(string message);
        void ProcessEvent(Func<Task> message);
    }
}

