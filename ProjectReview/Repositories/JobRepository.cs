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
        Task<List<JobDTO>> GetList();
        Task CreateJobDocument(long jobId, long documentId);
        Task<JobDTO> GetJob(long id);
        Task Finish(long id);
        Task Open(long id);
        Task CancleAssign(long id);
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

        public async Task Finish(long id)
        {
			var result = await _dataContext.Jobs
									.Where(x => x.Id == id)
									.FirstOrDefaultAsync();
            if (result != null)
            {
				result.Status = 2;
				_dataContext.Jobs.Update(result);
				await _dataContext.SaveChangesAsync();
			}  
		}

		public async Task Open(long id)
		{
			var result = await _dataContext.Jobs
									.Where(x => x.Id == id)
									.FirstOrDefaultAsync();
			if (result != null)
            {
				result.Status = 1;
				_dataContext.Jobs.Update(result);
				await _dataContext.SaveChangesAsync();
			}
		}

        public async Task CancleAssign(long id)
        {
			var job = await _dataContext.Jobs.Where(x => x.Id == id).FirstOrDefaultAsync();
			if (job != null)
            {
                job.Status = 0;
				_dataContext.Jobs.Update(job);
				await _dataContext.SaveChangesAsync();
			}
		}

		public async Task Delete(long id)
        {
			var result = await _dataContext.Jobs
							.Where(x => x.Id == id)
							.FirstOrDefaultAsync();
			if (result == null) return;
            var jobDocument = await _dataContext.JobDocuments.Where(x => x.JobId == id).FirstOrDefaultAsync();
            if (jobDocument != null)
            {
                var document = await _dataContext.Documents
                            .Where(x => x.Id == jobDocument.DocumentId)
                            .FirstOrDefaultAsync();
                if (document != null)
                {
                    document.IsAssign = false;
                    _dataContext.Documents.Update(document);
                }
            }
            await _dataContext.JobDocuments.Where(x => x.JobId == id).ExecuteDeleteAsync();
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
   //         var role = await _dataContext.UserRoles
   //                             .Include(x => x.Role)
			//					.Where(x => (x.UserId == _currentUser.UserId && x.Role.Name == "ManageJob"))
			//					.FirstOrDefaultAsync();
   //         if (role != null)
   //         {
			//	int count = await _dataContext.Jobs
			//							.Where(x => x.Content.Contains(filter))
			//							.CountAsync();
			//	var result = await _dataContext.Jobs
			//								.Where(x => x.Content.Contains(filter))
			//								.Include(x => x.CreateUser)
			//								.Include(x => x.Host)
			//	                            .Include(x => x.Instructor)
			//								.Skip((page - 1) * pageSize)
			//	                            .Take(pageSize)
			//								.ToListAsync();
			//	int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
			//	var jobs = _mapper.Map<List<Job>, List<JobDTO>>(result);
   //             if (jobs.Count > 0)
   //             {
   //                 foreach(var item  in jobs)
   //                 {
   //                     item.Users = new List<User>();
			//			List<Handler> handlers = await _dataContext.Handlers
			//									.Where(x => x.JobId == item.Id)
			//									.Include(x => x.User)
			//									.ToListAsync();
			//			if (handlers.Count > 0)
			//			{
			//				foreach (var handler in handlers)
			//				{
			//					item.Users.Add(handler.User);
			//				}
			//			}
			//		}
   //             }
   //             CustomPaging<JobDTO> paging = new CustomPaging<JobDTO>
   //             {
   //                 TotalPage = totalPage,
   //                 PageSize = pageSize,
   //                 Data = jobs,
			//	};
   //             return paging;
			//} else
   //         {
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
			//}
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

        public async Task<JobDTO> GetJob(long id)
        {
            var result = await _dataContext.Jobs
                                .Where(x => x.Id == id)
								.Include(x => x.CreateUser)
								.Include(x => x.Host)
								.Include(x => x.Instructor)
								.FirstOrDefaultAsync();
			var job = _mapper.Map<Job, JobDTO>(result);
			job.Users = new List<User>();
			List<Handler> handlers = await _dataContext.Handlers
									.Where(x => x.JobId == job.Id)
									.Include(x => x.User)
									.ToListAsync();
			if (handlers.Count > 0)
			{
				foreach (var j in handlers)
				{
					job.Users.Add(j.User);
				}
			}
            job.Opinions = new List<Opinion>();
            List<Opinion> opinions = await _dataContext.Opinions
                                                    .Where(x => x.JobId == job.Id)
                                                    .Include(x => x.CreateUser)
                                                    .ToListAsync();
			if (opinions.Count > 0)
			{
				foreach (var item in opinions)
				{
					job.Opinions.Add(item);
				}
			}
			var jobdocument = await _dataContext.JobDocuments.Where(x => x.JobId == job.Id).FirstOrDefaultAsync();
            if (jobdocument  != null)
            {
                job.Document = await _dataContext.Documents.Where(x => x.Id == jobdocument.DocumentId).FirstOrDefaultAsync();
            }
            return job;

		}

        public async Task<List<JobDTO>> GetList()
        {
            List<long> jobIds = new List<long>();
            var handler = await _dataContext.Handlers
                            .Where(x => x.UserId == _currentUser.UserId)
                            .ToListAsync();
            if (handler.Count > 0)
            {
                foreach (var i in handler)
                {
                    jobIds.Add(i.JobId);
                }
            }
            int count = await _dataContext.Jobs
                                    .Where(x => (x.HostId == _currentUser.UserId || x.InstructorId == _currentUser.UserId || jobIds.Contains(x.Id)))
                                    .CountAsync();
            var result = await _dataContext.Jobs
                                        .Where(x => (x.HostId == _currentUser.UserId || x.InstructorId == _currentUser.UserId || jobIds.Contains(x.Id)))
                                        .Include(x => x.CreateUser)
                                        .Include(x => x.Host)
                                        .Include(x => x.Instructor)
                                        .ToListAsync();
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
            return jobs;
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

        public async Task CreateJobDocument(long jobId, long documentId)
        {
            await _dataContext.JobDocuments.AddAsync(new JobDocument { JobId = jobId, DocumentId = documentId });
            await _dataContext.SaveChangesAsync();
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
