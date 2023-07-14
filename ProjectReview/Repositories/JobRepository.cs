using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Common;
using ProjectReview.DTO.Jobs;
using ProjectReview.DTO.Users;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;
using System.Drawing.Printing;
using System.Net;
using System.Text.Json;

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

        public async Task<CustomPaging<JobDTO>> GetCustomPaging(string filter, int page, int pageSize)
        {
            var role = await _dataContext.UserRoles
                                .Include(x => x.Role)
								.Where(x => (x.UserId == _currentUser.UserId && x.Role.Name == "ManageJob"))
								.FirstOrDefaultAsync();
            if (role != null)
            {
				int count = await _dataContext.Jobs
										.Where(x => x.Content.Contains(filter))
										.CountAsync();
				var result = await _dataContext.Jobs
											.Where(x => x.Content.Contains(filter))
											.Include(x => x.CreateUser)
											.Include(x => x.Host)
				                            .Include(x => x.Instructor)
											.Include(x => x.JobDocument)
											.Skip((page - 1) * pageSize)
				                            .Take(pageSize)
											.ToListAsync();
				int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
				var jobs = _mapper.Map<List<Job>, List<JobDTO>>(result);
                if (jobs.Count > 0)
                {
                    foreach(var item  in jobs)
                    {
                        item.Users = new List<User>();
						List<Handler> handlers = await _dataContext.Handlers
												.Where(x => x.JobId == item.Id)
												.Include(x => x.User)
												.ToListAsync();
						if (handlers.Count > 0)
						{
							foreach (var handler in handlers)
							{
								item.Users.Add(handler.User);
							}
						}
					}
                }
                CustomPaging<JobDTO> paging = new CustomPaging<JobDTO>
                {
                    TotalPage = totalPage,
                    PageSize = pageSize,
                    Data = jobs,
				};
			} else
            {

            }
            
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
