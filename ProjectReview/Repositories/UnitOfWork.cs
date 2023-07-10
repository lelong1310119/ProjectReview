using AutoMapper;
using ProjectReview.Models;

namespace ProjectReview.Repositories
{
	public interface IUnitOfWork : IDisposable
	{
		void Save();
		IDepartmentRepository DepartmentRepository { get; }
		IPositionRepository PositionRepository { get; }
		IRankRepository RankRepository { get; }
	}

	public class UnitOfWork : IUnitOfWork
	{
		private readonly DataContext _context;

		public IDepartmentRepository DepartmentRepository { get; private set; }
		public IPositionRepository PositionRepository { get; private set; }
		public IRankRepository RankRepository { get; private set; }

		public UnitOfWork(DataContext context, IMapper mapper)
		{
			_context = context;
			DepartmentRepository = new DepartmentRepository(context, mapper);
			PositionRepository = new PositionRepository(context, mapper);	
			RankRepository = new RankRepository(context, mapper);	
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
