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
		IDocumentTypeRepository DocumentTypeRepository { get; }
		IPermissionGroupRepository PermissionGroupRepository { get; }
		ICategoryProfileRepository CategoryProfileRepository { get; }
		IJobProfileRepository JobProfileRepository { get; }
		IProfileDocumentRepository ProfileDocumentRepository { get; }
		IDocumentRepository DocumentRepository { get; }
		IJobRepository JobRepository { get; }
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
		public IDocumentTypeRepository DocumentTypeRepository { get; private set; }
		public IPermissionGroupRepository PermissionGroupRepository { get; private set; }
		public ICategoryProfileRepository CategoryProfileRepository { get; private set; }
		public IJobProfileRepository JobProfileRepository { get; private set; }
		public IProfileDocumentRepository ProfileDocumentRepository { get; private set; }
		public IDocumentRepository DocumentRepository { get; private set; }
		public IJobRepository JobRepository { get; private set; }

		public UnitOfWork(DataContext context, IMapper mapper, ICurrentUser currentUser)
		{
			_context = context;
			_mapper = mapper;
			_currentUser = currentUser;
			DepartmentRepository = new DepartmentRepository(context, mapper);
			PositionRepository = new PositionRepository(context, mapper);	
			RankRepository = new RankRepository(context, mapper);
			UserRepository = new UserRepository(context, mapper);
			DocumentTypeRepository = new DocumentTypeRepository(context, mapper, currentUser);
			PermissionGroupRepository = new PermissionGroupRepository(context, mapper);
			CategoryProfileRepository = new CategoryProfileRepository(context, mapper, currentUser);
			JobProfileRepository = new JobProfileRepository(context, mapper, currentUser);
			ProfileDocumentRepository = new ProfileDocumentRepository(context, mapper);
			DocumentRepository = new DocumentRepository(context, mapper, currentUser);
			JobRepository = new JobRepository(context, mapper, currentUser);
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
