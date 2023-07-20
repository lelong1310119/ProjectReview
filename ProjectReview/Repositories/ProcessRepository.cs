using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Common;
using ProjectReview.DTO.Processes;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using System.Net.WebSockets;

namespace ProjectReview.Repositories
{
    public interface IProcessRepository
    {
        Task CreateProcess(CreateProcessDTO create);
        Task Create(CreateProcessDTO create, string opinion);
        Task Done(long id);
        Task<long> Active(long id);
        Task CancleActive(long id);
        Task<long> CancleAssign(long id);
        Task<long> Finish(long id);
        Task Delete(long id);
        Task CreateFromDocument(CreateProcessDTO create);

    }
    public class ProcessRepository : IProcessRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public ProcessRepository(DataContext dataContext, IMapper mapper, ICurrentUser currentUser)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task Delete(long id)
        {
            var result = await _dataContext.ProcessUsers
                                    .Include(x => x.Process)
                                    .Where(x => x.Process.JobId == id)
                                    .ToListAsync();
            _dataContext.ProcessUsers.RemoveRange(result);
            var process = await _dataContext.Processes
                                    .Where(x => x.JobId == id)
                                    .ToListAsync();
            _dataContext.Processes.RemoveRange(process);  
            await _dataContext.SaveChangesAsync();
        }

		public async Task<long> CancleAssign(long id)
		{
			var process = await _dataContext.Processes.Where(x => (x.JobId == id && x.ProcessEnd == true)).FirstOrDefaultAsync();
			if (process != null)
			{
				process.Status = 1;
				_dataContext.Processes.Update(process);
				await _dataContext.SaveChangesAsync();
			}
			return process.Id;
		}

		public async Task<long> Active(long id)
        {
            var process = await _dataContext.Processes.Where(x => (x.JobId  == id && x.ProcessEnd == true)).FirstOrDefaultAsync();
            process.Status = 2;
            _dataContext.Processes.Update(process);
            await _dataContext.SaveChangesAsync();
            return process.Id;
        }

		public async Task<long> Finish(long id)
		{
			var process = await _dataContext.Processes.Where(x => (x.JobId == id && x.ProcessEnd == true)).FirstOrDefaultAsync();
			process.Status = 3;
			_dataContext.Processes.Update(process);
			await _dataContext.SaveChangesAsync();
			return process.Id;
		}

		public async Task CancleActive(long id)
        {
            var process = await _dataContext.Processes.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (process == null) return;
            process.Status = 1;
            _dataContext.Processes.Update(process);
            await _dataContext.SaveChangesAsync();
        }

        public async Task Done(long id)
        {
            var process = await _dataContext.Processes.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (process == null) return;
            process.Status = 3;
            _dataContext.Processes.Update(process);
            await _dataContext.SaveChangesAsync();
        }

        public async Task CreateProcess(CreateProcessDTO create)
        {
            Process process = new Process
            {
                JobId = create.JobId,
                InstructorId = create.InstructorId,
                Status = 1,
                ProcessEnd = true,
            };
            await _dataContext.Processes.AddAsync(process);
            await _dataContext.SaveChangesAsync();

            if (create.UserId.Count > 0)
            {
                foreach(var item in create.UserId)
                {
                    await _dataContext.ProcessUsers.AddAsync(new ProcessUser { UserId = item, ProcessId = process.Id });
                }
            }
            History history = new History
            {
                ProcessId = process.Id,
                Content = "Thêm mới công việc",
                CreateDate = DateTime.Now,
                CreateUserId = _currentUser.UserId,
            };
            await _dataContext.History.AddAsync(history);
            await _dataContext.SaveChangesAsync();
        }

        public async Task CreateFromDocument(CreateProcessDTO create)
        {
            Process process = new Process
            {
                JobId = create.JobId,
                InstructorId = create.InstructorId,
                Status = 1,
                ProcessEnd = true,
            };
            await _dataContext.Processes.AddAsync(process);
            await _dataContext.SaveChangesAsync();

            if (create.UserId.Count > 0)
            {
                foreach (var item in create.UserId)
                {
                    await _dataContext.ProcessUsers.AddAsync(new ProcessUser { UserId = item, ProcessId = process.Id });
                }
            }
            History history = new History
            {
                ProcessId = process.Id,
                Content = "Chuyển xử lý văn bản",
                CreateDate = DateTime.Now,
                CreateUserId = _currentUser.UserId,
            };
            await _dataContext.History.AddAsync(history);
            await _dataContext.SaveChangesAsync();
        }

        public async Task Create(CreateProcessDTO create, string opinion)
        {
            Process process = new Process
            {
                JobId = create.JobId,
                InstructorId = create.InstructorId,
                Status = 2,
                ProcessEnd = true,
            };
            await _dataContext.Processes.AddAsync(process);
            await _dataContext.SaveChangesAsync();
            var result = await _dataContext.Processes
                                    .Where(x => (x.ProcessEnd == true && x.JobId == create.JobId))
                                    .FirstOrDefaultAsync();
            if (result != null)
            {
                result.ProcessEnd = false;
            }
            _dataContext.Processes.Update(process);
            if (create.UserId.Count > 0)
            {
                foreach (var item in create.UserId)
                {
                    await _dataContext.ProcessUsers.AddAsync(new ProcessUser { UserId = item, ProcessId = process.Id });
                }
            }
            History history = new History
            {
                ProcessId = process.Id,
                Content = opinion + "(Chuyển công việc)",
                CreateDate = DateTime.Now,
                CreateUserId = _currentUser.UserId,
            };
            await _dataContext.History.AddAsync(history);
            await _dataContext.SaveChangesAsync();
        }
    }
}
