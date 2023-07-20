using Humanizer;
using ProjectReview.DTO.Histories;
using ProjectReview.DTO.Jobs;
using ProjectReview.DTO.Opinions;
using ProjectReview.DTO.Processes;
using ProjectReview.DTO.Users;
using ProjectReview.Paging;
using ProjectReview.Repositories;

namespace ProjectReview.Services.Jobs
{
	public interface IJobService
	{
		Task<CustomPaging<JobDTO>> GetListJob(string filter, int page, int pageSize);
		Task<List<UserDTO>> GetListUser();
		Task<JobDTO> Create(CreateJobDTO createJobDTO);
		Task Active(long id);
		Task Delete(long id);
		Task<UpdateJobDTO> GetById(long id);
		Task<JobDTO> Update(UpdateJobDTO updateJobDTO);
		Task<List<UserDTO>> GetHostUser();
		Task AddOpinion(CreateOpinionDTO createOpinionDTO);
		Task<JobDTO> GetJob(long id);
		Task Finish(long id);
		Task CancleAssign(long id);
		Task Open(long id);
		Task AddHistory(CreateHistoryDTO create);
		Task<List<UserDTO>> GetManager();
		Task<ForwardDTO> GetForward(long id);
		Task<List<UserDTO>> GetListForward(long id);
		Task<List<UserDTO>> GetMangerForward(long id);
		Task Forward(ForwardDTO forward);
    }

	public class JobService : IJobService
	{
		private readonly IUnitOfWork _UOW;	
		public JobService(IUnitOfWork unitOfWork)
		{
			_UOW = unitOfWork;
		}

		public async Task<List<UserDTO>> GetListUser()
		{
			return await _UOW.UserRepository.GetList();
		}

		public async Task<ForwardDTO> GetForward(long id)
		{
			return await _UOW.JobRepository.GetForward(id);
		}

        public async Task<List<UserDTO>> GetListForward(long id)
        {
            return await _UOW.UserRepository.GetListForward(id);
        }

        public async Task<List<UserDTO>> GetMangerForward(long id)
        {
			return await _UOW.UserRepository.GetManagerForward(id);
        }

		public async Task Forward(ForwardDTO forward)
		{
            if (forward.ListUserId == null || forward.ListUserId.Count == 0) throw new Exception("Bạn chưa chọn người xử lý");
            if (!await _UOW.UserRepository.CheckUser(forward.ListUserId, forward.InstructorId)) throw new Exception("Người xử lý phải cùng phòng với người chỉ đạo - theo dõi");
			CreateProcessDTO create = new CreateProcessDTO
			{
				JobId = forward.Id,
				InstructorId = forward.InstructorId,
				UserId = forward.ListUserId
			};
			await _UOW.ProcessRepository.Create(create, forward.Opinion);
        }

        public async Task<UpdateJobDTO> GetById(long id)
		{
			var job = await _UOW.JobRepository.GetById(id);
			//if (job.FilePath != null && job.FilePath != "")
			//{
   //             var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", job.FilePath);
   //             using var stream = new FileStream(filePath, FileMode.Open);
   //             using var memoryStream = new MemoryStream();
   //             stream.CopyTo(memoryStream);
   //             job.FormFile = new FormFile(memoryStream, 0, memoryStream.Length, null, Path.GetFileName(filePath));
   //         }
            return job;
		}

		public async Task Finish(long id)
		{
			long result = await _UOW.ProcessRepository.Finish(id);
			CreateHistoryDTO create = new CreateHistoryDTO { ProcessId = result, JobId = id, Content = "Kết thúc công việc" };
			await _UOW.HistoryRepository.Create(create);
		}

		public async Task Open(long id)
		{
			long result = await _UOW.ProcessRepository.Active(id);
			CreateHistoryDTO create = new CreateHistoryDTO { ProcessId = result, JobId = id, Content = "Mở lại công việc" };
			await _UOW.HistoryRepository.Create(create);
		}

		public async Task CancleAssign(long id)
		{
			long result = await _UOW.ProcessRepository.CancleAssign(id);
			CreateHistoryDTO create = new CreateHistoryDTO { ProcessId = result, JobId = id, Content = "Hủy duyệt công việc" };
			await _UOW.HistoryRepository.Create(create);
		}

		public async Task<JobDTO> Update(UpdateJobDTO updateJobDTO)
		{
			if (updateJobDTO.Deadline.Date < DateTime.Now.Date) throw new Exception("Hạn xử lý không hợp lệ");
			if (updateJobDTO.FormFile != null)
			{
				if (updateJobDTO.FilePath != null && updateJobDTO.FilePath != "")
				{
                    var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", updateJobDTO.FilePath);
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                }
                updateJobDTO.FileName = updateJobDTO.FormFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", "Job_" + updateJobDTO.Id.ToString() + "_" + updateJobDTO.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await updateJobDTO.FormFile.CopyToAsync(fileStream);
                }
                updateJobDTO.FilePath = "Job_" + updateJobDTO.Id.ToString() + "_" + updateJobDTO.FileName;
            }
			return await _UOW.JobRepository.Update(updateJobDTO);
		}

		public async Task<List<UserDTO>> GetHostUser()
		{
			return await _UOW.UserRepository.GetLeader();
		}

        public async Task<List<UserDTO>> GetManager()
        {
            return await _UOW.UserRepository.GetManager();
        }

        public async Task<JobDTO> GetJob(long id)
		{
			var result = await _UOW.JobRepository.GetJob(id);
			result.Histories = await _UOW.HistoryRepository.GetList(id);
			return result;
		}

        public async Task AddHistory(CreateHistoryDTO create)
        {
            var result = await _UOW.HistoryRepository.Create(create);
            if (create.FormFile != null)
            {
                string fileName = create.FormFile.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", "History_" + result.ToString() + "_" + fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await create.FormFile.CopyToAsync(fileStream);
                }
                filePath = "History_" + result.ToString() + "_" + fileName;
                await _UOW.HistoryRepository.UpdateFile(fileName, filePath, result);
            }
        }

        public async Task AddOpinion(CreateOpinionDTO createOpinionDTO)
		{
			var result = await _UOW.OpinionRepository.Create(createOpinionDTO);
            if (result != null)
            {
                if (createOpinionDTO.FormFile != null)
				{
					result.FileName = createOpinionDTO.FormFile.FileName;
					var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", "Opinion_" + result.Id.ToString() + "_" + result.FileName);
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await createOpinionDTO.FormFile.CopyToAsync(fileStream);
					}
					result.FilePath = "Opinion_" + result.Id.ToString() + "_" + result.FileName;
					await _UOW.OpinionRepository.UpdateFile(result);
				}
            }
        }

		public async Task<JobDTO> Create(CreateJobDTO createJobDTO)
		{
			if (createJobDTO.Deadline.Date < DateTime.Now.Date) throw new Exception("Hạn xử lý không hợp lệ");
            if (createJobDTO.ListUserId == null || createJobDTO.ListUserId .Count == 0) throw new Exception("Bạn chưa chọn người xử lý");
			if (!await _UOW.UserRepository.CheckUser(createJobDTO.ListUserId, createJobDTO.InstructorId)) throw new Exception("Người xử lý phải cùng phòng với người chỉ đạo - theo dõi");
			List<long> ListUserId = createJobDTO.ListUserId;
			var job = await _UOW.JobRepository.Create(createJobDTO);
			if (job == null) return null;
			CreateProcessDTO process = new CreateProcessDTO {
				JobId = job.Id,
				InstructorId = job.InstructorId,
				UserId = ListUserId
			};
			await _UOW.ProcessRepository.CreateProcess(process);
			if (createJobDTO.FormFile != null)
			{
				job.FileName = createJobDTO.FormFile.FileName;
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", "Job_" + job.Id.ToString() + "_" + job.FileName);
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await createJobDTO.FormFile.CopyToAsync(fileStream);
				}
				job.FilePath = "Job_" + job.Id.ToString() + "_" + job.FileName;
				return await _UOW.JobRepository.UpdateFile(job);
			}
			return job;
		}

		public async Task Active(long id)
		{
			long result = await _UOW.ProcessRepository.Active(id);
			CreateHistoryDTO create = new CreateHistoryDTO { ProcessId = result, JobId = id, Content = "Duyệt công việc" };
			await _UOW.HistoryRepository.Create(create);
		}

		public async Task Delete(long id)
		{
			await _UOW.HistoryRepository.DeleteByJob(id);
			await _UOW.ProcessRepository.Delete(id);
			await _UOW.JobRepository.Delete(id);
		}

		public async Task<CustomPaging<JobDTO>> GetListJob(string filter, int page, int pageSize)
		{
			filter = filter ?? "";
			return await _UOW.JobRepository.GetCustomPaging(filter, page, pageSize);
		}
	}
}
