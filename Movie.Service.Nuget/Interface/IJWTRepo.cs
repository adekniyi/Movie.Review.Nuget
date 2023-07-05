using System;
using Movie.Service.Nuget.Model;

namespace Movie.Service.Nuget.Interface
{
	public interface IJWTRepo
	{
        string GenerateUserToken(User user);
    }
}

