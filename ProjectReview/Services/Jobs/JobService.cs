using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectReview.DTO.Jobs;
using ProjectReview.DTO.Opinions;
using ProjectReview.DTO.Users;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;
using ProjectReview.Repositories;
using System.Net.WebSockets;

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
			return await _UOW.UserRepository.GetAllUser();
		}

		public async Task<UpdateJobDTO> GetById(long id)
		{
			var job = await _UOW.JobRepository.GetById(id);
			if (job.FilePath != null && job.FilePath != "")
			{
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", job.FilePath);
                using var stream = new FileStream(filePath, FileMode.Open);
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                job.FormFile = new FormFile(memoryStream, 0, memoryStream.Length, null, Path.GetFileName(filePath));
            }
			job.ListUserId = await _UOW.JobUserRepository.GetAll(job.Id);
            return job;
		}

		public async Task<JobDTO> Update(UpdateJobDTO updateJobDTO)
		{
			return await _UOW.JobRepository.Update(updateJobDTO);
		}

		public async Task<JobDTO> Create(CreateJobDTO createJobDTO)
		{
			if (createJobDTO.ListUserId.Count > 0) throw new Exception("Bạn chưa chọn người xử lý");
			List<long> longs = createJobDTO.ListUserId;
			var job = await _UOW.JobRepository.Create(createJobDTO);
			if (job == null) return null;
			CreateOpinionDTO createOpinionDTO = new CreateOpinionDTO { Content = "Thêm mới công việc và phân công xử lý", JobId = job.Id };
			await _UOW.OpinionRepository.Create(createOpinionDTO);
			await _UOW.JobUserRepository.Create(longs, job.Id);
			//if (createJobDTO.FormFile != null)
			//{
			//	job.FileName = createJobDTO.FormFile.FileName;
   //             var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", "Job_" + job.Id.ToString() + "_" + job.FileName);
   //             using (var fileStream = new FileStream(filePath, FileMode.Create))
   //             {
   //                 await createJobDTO.FormFile.CopyToAsync(fileStream);
   //             }
			//	job.FilePath = "Job_" + job.Id.ToString() + "_" + job.FileName;
			//	return await _UOW.JobRepository.UpdateFile(job);
			//}
			return job;
		}

		public async Task Active(long id)
		{
			await _UOW.JobRepository.Active(id);
		}

		public async Task Delete(long id)
		{
			await _UOW.JobUserRepository.Delete(id);
			await _UOW.OpinionRepository.Delete(id);
			await _UOW.JobRepository.Delete(id);
		}

		public async Task<CustomPaging<JobDTO>> GetListJob(string filter, int page, int pageSize)
		{
			filter = filter ?? "";
			return await _UOW.JobRepository.GetCustomPaging(filter, page, pageSize);
		}
	}
}
