using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Models;

namespace ProjectReview.Repositories
{
	public interface IProfileDocumentRepository
	{
		Task DeleteByProfile(long id);
	}

	public class ProfileDocumentRepository : IProfileDocumentRepository
	{
		private readonly DataContext _dataContext;
		private readonly IMapper _mapper;

		public ProfileDocumentRepository(DataContext dataContext, IMapper mapper)
		{
			_dataContext = dataContext;
			_mapper = mapper;
		}

		public async Task DeleteByProfile(long id)
		{
			await _dataContext.ProfileDocuments
						.Where(x => x.JobProfileId == id)
						.ExecuteDeleteAsync();
			await _dataContext.SaveChangesAsync();
		}
	}
}
