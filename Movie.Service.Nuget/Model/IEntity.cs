using System;
namespace Movie.Service.Nuget.Model
{
	public interface IEntity
	{
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

