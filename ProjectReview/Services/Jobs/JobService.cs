using ProjectReview.DTO.Jobs;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;

namespace ProjectReview.Services.Jobs
{
	public interface IJobService
	{

	}

	public class JobService : IJobService
	{
		private readonly IJobService _jobService;	
		public JobService(IJobService jobService)
		{
			_jobService = jobService;
		}

		public async Task<CustomPaging<JobDTO>> GetListJob(long Id)
		{
			List<Handler> handlers = await _
		}
	}
}
