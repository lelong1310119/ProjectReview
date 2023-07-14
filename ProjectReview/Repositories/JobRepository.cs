using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Common;
using ProjectReview.DTO.JobProfiles;
using ProjectReview.DTO.Jobs;
using ProjectReview.DTO.Users;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;
using System.Drawing.Printing;
using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProjectReview.Repositories
{
    public interface IJobRepository
    {
        Task<JobDTO> GetByDocument(long id);
        Task<CustomPaging<JobDTO>> GetCustomPaging(string filter, int page, int pageSize);
        Task<JobDTO> Create(CreateJobDTO createJob);
        Task<JobDTO> UpdateFile(JobDTO update);
        Task Active(long id);
        Task Delete(long id);
        Task<JobDTO> Update(UpdateJobDTO updateJob);
        Task<UpdateJobDTO> GetById(long id);
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

        public async Task<UpdateJobDTO> GetById(long id)
        {
            var result = await _dataContext.Jobs
                                    .Where(x => x.Id == id)  
                                    .FirstOrDefaultAsync();
            return _mapper.Map<Job, UpdateJobDTO>(result);
        }

        public async Task Delete(long id)
        {
			var result = await _dataContext.Jobs
							.Where(x => x.Id == id)
							.FirstOrDefaultAsync();
			if (result == null) return;
            if (result.FilePath != null && result.FilePath != "")
            {
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", result.FilePath);
				if (System.IO.File.Exists(filePath))
				{
					System.IO.File.Delete(filePath);
				}
			}
			_dataContext.Jobs.Remove(result);
			await _dataContext.SaveChangesAsync();
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
                return paging;
			} else
            {
                List<long> jobIds = new List<long>();
                var handler = await _dataContext.Handlers
                                .Where(x => x.UserId == _currentUser.UserId)
                                .ToListAsync();
                if (handler.Count > 0)
                {
                    foreach(var i in  handler)
                    {
                        jobIds.Add(i.JobId);
                    }
                }
				int count = await _dataContext.Jobs
										.Where(x => ((x.Content.Contains(filter)) && (x.HostId == _currentUser.UserId || x.InstructorId == _currentUser.UserId || jobIds.Contains(x.Id))))
										.CountAsync();
				var result = await _dataContext.Jobs
											.Where(x => ((x.Content.Contains(filter)) && (x.HostId == _currentUser.UserId || x.InstructorId == _currentUser.UserId || jobIds.Contains(x.Id))))
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
					foreach (var item in jobs)
					{
						item.Users = new List<User>();
						List<Handler> handlers = await _dataContext.Handlers
												.Where(x => x.JobId == item.Id)
												.Include(x => x.User)
												.ToListAsync();
						if (handlers.Count > 0)
						{
							foreach (var j in handlers)
							{
								item.Users.Add(j.User);
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
				return paging;
			}
        }

        public async Task<JobDTO> UpdateFile(JobDTO update)
        {
            var job = await _dataContext.Jobs.Where(x => x.Id == update.Id).FirstOrDefaultAsync();
            job.FileName = update.FileName;
            job.FilePath = update.FilePath; 
            _dataContext.Jobs.Update(job);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<Job, JobDTO>(job);
        }

        public async Task<JobDTO> Update(UpdateJobDTO updateJob)
        {
            var job = await _dataContext.Jobs
                                    .Where(x => x.Id == updateJob.Id)
                                    .FirstOrDefaultAsync();
            if (job == null) return null;
            job.HostId = updateJob.HostId;
            job.InstructorId = updateJob.InstructorId;
            job.Deadline = updateJob.Deadline;
            job.Request = updateJob.Request;
            job.FileName = updateJob.FileName;
            job.FilePath = updateJob.FilePath;
            job.Content = updateJob.Content;
            _dataContext.Jobs.Update(job);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<Job, JobDTO>(job);
        }

        public async Task Active(long id)
        {
            var job = await _dataContext.Jobs.Where(x => x.Id == id).FirstOrDefaultAsync();
            job.Status = 1;
            _dataContext.Jobs.Update(job);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<JobDTO> Create(CreateJobDTO createJob)
        {
            var job = _mapper.Map<CreateJobDTO, Job>(createJob);
            long maxId = 0;
            if (await _dataContext.Jobs.AnyAsync())
            {
                maxId = await _dataContext.Jobs.MaxAsync(x => x.Id);
            }
            job.Id = maxId + 1;
            job.CreateUserId = _currentUser.UserId;
            job.CreateDate = DateTime.Now;
            await _dataContext.Jobs.AddAsync(job);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<Job, JobDTO>(job);
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
