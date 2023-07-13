using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Common;
using ProjectReview.DTO.Jobs;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using System.Net;

namespace ProjectReview.Repositories
{
    public interface IJobRepository
    {
        Task<JobDTO> GetByDocument(long id);
	}

    public class JobRepository : IJobRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public JobRepository(DataContext dataContext, IMapper mapper, ICurrentUser currentUser)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<JobDTO> GetByDocument(long id)
        {
            var result = await _dataContext.JobDocuments
                                    .Where(x => x.DocumentId == id)
                                    .FirstOrDefaultAsync();
            if (result == null) return null;
            var job = await _dataContext.Jobs
                                .Where(x => x.Id == result.JobId)
                                .Include(x => x.CreateUser)
                                .Include(x => x.Host)
                                .Include(x => x.Instructor)
                                .FirstOrDefaultAsync();
            var jobDocument = _mapper.Map<Job, JobDTO>(job);
            jobDocument.Users = new List<User>();
            List<Handler> handlers = await _dataContext.Handlers
                                                .Where(x => x.JobId == job.Id)
                                                .Include(x => x.User)
                                                .ToListAsync();
            if(handlers.Count > 0)
            {
                foreach (var item in handlers)
                {
                    jobDocument.Users.Add(item.User);
                }
            }
            return jobDocument;
        }
    }
}
