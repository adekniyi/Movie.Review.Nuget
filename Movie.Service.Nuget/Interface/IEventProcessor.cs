using System;
namespace Movie.Service.Nuget.Interface
{
	public interface IEventProcessor
	{
		void ProcessEvent(string message);
	}
}

