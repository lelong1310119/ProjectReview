using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Common;
using ProjectReview.DTO.Histories;
using ProjectReview.DTO.Opinions;
using ProjectReview.DTO.PermissionGroups;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using System.Text.Json;

namespace ProjectReview.Repositories
{
    public interface IHistoryRepository
    {
        Task<long> Create(CreateHistoryDTO create);
        Task<List<HistoryDTO>> GetList(long jobId);
        Task UpdateFile(string fileName, string filePath, long id);
        Task DeleteByDocument(long documentId);
        Task DeleteByJob(long jobId);
    }

    public class HistoryRepository : IHistoryRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public HistoryRepository(DataContext dataContext, IMapper mapper, ICurrentUser currentUser)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<long> Create(CreateHistoryDTO create)
        {
            History history = new History { 
                Content = create.Content,
                CreateDate = DateTime.Now,
                CreateUserId = _currentUser.UserId,
                ProcessId = create.ProcessId
            };
            await _dataContext.History.AddAsync(history);
            await _dataContext.SaveChangesAsync();
            return history.Id;
        }

        public async Task UpdateFile(string fileName, string filePath, long id)
        {
            var history = await _dataContext.History.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (history == null) return;
            history.FilePath = filePath;
            history.FileName = fileName;
            _dataContext.History.Update(history);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteByDocument(long documentId)
        {
            var jobDocument = await _dataContext.JobDocuments.Where(x => x.DocumentId == documentId).FirstOrDefaultAsync(); 
            if (jobDocument != null)
            {
                var result = await _dataContext.History
                                    .Include(x => x.Process)
                                    .Where(x => x.Process.JobId ==  jobDocument.JobId)
                                    .ToListAsync();
				if (result.Count > 0)
				{
					foreach (var item in result)
					{
						if (item.FilePath != null && item.FilePath != "")
						{
							var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", item.FilePath);
							if (System.IO.File.Exists(filePath))
							{
								System.IO.File.Delete(filePath);
							}
						}
						_dataContext.Remove(item);
					}
				}
				await _dataContext.SaveChangesAsync();
			}
        }

        public async Task DeleteByJob(long jobId)
        {
            var result =  await _dataContext.History
                                    .Include(x => x.Process)
                                    .Where(x => x.Process.JobId == jobId)
                                    .ToListAsync();
			if (result.Count > 0)
			{
				foreach (var item in result)
				{
					if (item.FilePath != null && item.FilePath != "")
					{
						var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", item.FilePath);
						if (System.IO.File.Exists(filePath))
						{
							System.IO.File.Delete(filePath);
						}
					}
					_dataContext.Remove(item);
				}
			}
			await _dataContext.SaveChangesAsync();
		}

        public async Task<List<HistoryDTO>> GetList(long jobId)
        {
            var job = await _dataContext.Jobs.Where(x => x.Id == jobId).FirstOrDefaultAsync();
            if (job == null) return new List<HistoryDTO>();
            if (_currentUser.UserId == job.CreateUserId || _currentUser.UserId == job.HostId)
            {
                var histories = await _dataContext.History
                                            .Include(x => x.Process)   
                                            .Where(x => x.Process.JobId == jobId)
                                            .ToListAsync();
                return _mapper.Map<List<History>, List<HistoryDTO>>(histories);
            }
            else
            {
                long id = 0;
                var processUser = await _dataContext.ProcessUsers
                                        .Include(x => x.Process)
                                        .Where(x => (x.UserId == _currentUser.UserId && x.Process.JobId == job.Id )).FirstOrDefaultAsync();
                var process = await _dataContext.Processes.Where(x => x.InstructorId == _currentUser.UserId).FirstOrDefaultAsync();
                if (processUser == null && process == null) return new List<HistoryDTO>();
                if (process != null)
                {
                    id = process.Id;
                } else if (processUser != null)
                {
                    id = processUser.ProcessId;
                }
                var histories = await _dataContext.History
                                            .Include(x => x.Process)
                                            .Where(x => ( (x.Process.JobId == jobId) && (x.ProcessId == id || x.CreateUserId == job.CreateUserId || x.CreateUserId == job.HostId)))
                                            .ToListAsync();
                return _mapper.Map<List<History>, List<HistoryDTO>>(histories);
            }
        }
    }
}
