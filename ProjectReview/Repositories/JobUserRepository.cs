using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using ProjectReview.Models;
using ProjectReview.Models.Entities;

namespace ProjectReview.Repositories
{
    public interface IJobUserRepository
    {
        Task Create(List<long> userIds, long jobId);
        Task Delete(long id);
        Task Update(List<long> userIds, long jobId);
        Task<List<long>> GetAll(long id);
    }

    public class JobUserRepository : IJobUserRepository
    {
        private readonly DataContext _dataContext;

        public JobUserRepository(DataContext dataContext) { 
            _dataContext = dataContext;
        }

        public async Task Create(List<long> userIds, long jobId)
        {
            foreach (long userId in userIds) { 
                await _dataContext.Handlers.AddAsync(new Handler { UserId = userId, JobId = jobId });
            }
            await _dataContext.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            await _dataContext.Handlers
                        .Where(x => x.JobId == id)
                        .ExecuteDeleteAsync();
            await _dataContext.SaveChangesAsync();
        }

        public async Task Update(List<long> userIds, long jobId)
        {
            await Delete(jobId);
            await Create(userIds, jobId);
        }

        public async Task<List<long>> GetAll(long id)
        {
            List<long> userIds = new List<long>();
            var result = await _dataContext.Handlers
                                    .Where(x => x.JobId == id)
                                    .ToListAsync();
            if (result.Count > 0)
            {
                foreach (var user in result)
                {
                    userIds.Add(user.UserId);
                }
            }
            return userIds;
        }
    }
}
