using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Models;
using ProjectReview.Models.Entities;

namespace ProjectReview.Repositories
{
	public interface IProfileDocumentRepository
	{
		Task DeleteByProfile(long id);
		Task Create(long documentId, List<long> profiles);

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

		public async Task Create(long documentId, List<long> profiles)
		{
			var result = await _dataContext.ProfileDocuments.Where(x => x.DocumentId == documentId).ToListAsync();
			_dataContext.ProfileDocuments.RemoveRange(result);
			if (profiles.Count > 0)
			{
				foreach(var profile in profiles)
				{
					await _dataContext.ProfileDocuments.AddAsync(new ProfileDocument { DocumentId = documentId, JobProfileId = profile });
				}
			}
			await _dataContext.SaveChangesAsync();
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
