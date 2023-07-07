using AutoMapper;
using ProjectReview.Models;

namespace ProjectReview.Repositories
{
	public interface IUnitOfWork : IDisposable
	{
		void Save();
		IDepartmentRepository DepartmentRepository { get; }
	}

	public class UnitOfWork : IUnitOfWork
	{
		private readonly DataContext _context;

		public IDepartmentRepository DepartmentRepository { get; private set; }

		public UnitOfWork(DataContext context, IMapper mapper)
		{
			_context = context;
			DepartmentRepository = new DepartmentRepository(context, mapper);
		}

		public void Save()
		{
			_context.SaveChanges();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
