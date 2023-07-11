using AutoMapper;
using ProjectReview.Common;
using ProjectReview.Models;

namespace ProjectReview.Repositories
{
	public interface IUnitOfWork : IDisposable
	{
		void Save();
		IDepartmentRepository DepartmentRepository { get; }
		IPositionRepository PositionRepository { get; }
		IRankRepository RankRepository { get; }
		IUserRepository UserRepository { get; }
	}

	public class UnitOfWork : IUnitOfWork
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;
		private readonly ICurrentUser _currentUser;

		public IDepartmentRepository DepartmentRepository { get; private set; }
		public IPositionRepository PositionRepository { get; private set; }
		public IRankRepository RankRepository { get; private set; }
		public IUserRepository UserRepository { get; private set; }

		public UnitOfWork(DataContext context, IMapper mapper, ICurrentUser currentUser)
		{
			_context = context;
			_mapper = mapper;
			_currentUser = currentUser;
			DepartmentRepository = new DepartmentRepository(context, mapper);
			PositionRepository = new PositionRepository(context, mapper);	
			RankRepository = new RankRepository(context, mapper);
			UserRepository = new UserRepository(context, mapper);
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
