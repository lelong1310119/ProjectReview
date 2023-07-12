using AutoMapper;
using ProjectReview.DTO.CategoryProfiles;
using ProjectReview.DTO.JobProfiles;
using ProjectReview.DTO.JobProfiles;
using ProjectReview.Paging;
using ProjectReview.Repositories;

namespace ProjectReview.Services.JobProfiles
{
	public interface IJobProfileService
	{
		Task Delete(long id);
		Task Active(long id);
		Task Close(long id);
		Task<JobProfileDTO> Create(CreateJobProfileDTO createJobProfile);
		Task<JobProfileDTO> Update(UpdateJobProfileDTO updateJobProfile);
		Task<UpdateJobProfileDTO> GetById(long id);
		Task<List<JobProfileDTO>> GetAll();
		Task<CustomPaging<JobProfileDTO>> GetCustomPaging(string? filter, int page, int pageSize);
		Task<List<CategoryProfileDTO>> GetCategoryProfile();
	}

	public class JobProfileService : IJobProfileService
	{
		private readonly IUnitOfWork _UOW;
		public JobProfileService(IUnitOfWork unitOfWork)
		{
			_UOW = unitOfWork;
		}

		public async Task Delete(long id)
		{
			await _UOW.ProfileDocumentRepository.DeleteByProfile(id);
			await _UOW.JobProfileRepository.Delete(id);
		}

		public async Task Active(long id)
		{
			await _UOW.JobProfileRepository.Active(id);
		}

		public async Task Close(long id)
		{
			await _UOW.JobProfileRepository.Close(id);
		}

		public async Task<JobProfileDTO> Create(CreateJobProfileDTO createJobProfile)
		{
            if (createJobProfile.StartDate >= createJobProfile.EndDate) throw new Exception("Ngày kết thúc phải sau ngày bắt đầu");
			createJobProfile.Name = createJobProfile.Name.Trim();
			var check = await _UOW.JobProfileRepository.GetByName(createJobProfile.Name);
			if (check) throw new Exception("Tên hồ sơ công việc đã tồn tại. Vui lòng nhập tên khác");
			return await _UOW.JobProfileRepository.Create(createJobProfile);
		}

		public async Task<JobProfileDTO> Update(UpdateJobProfileDTO updateJobProfile)
		{
            if (updateJobProfile.StartDate >= updateJobProfile.EndDate) throw new Exception("Ngày kết thúc phải sau ngày bắt đầu");
            var jobProfile = await _UOW.JobProfileRepository.GetById(updateJobProfile.Id);
			updateJobProfile.Name = updateJobProfile.Name.Trim();
			if (updateJobProfile.Name != jobProfile.Name)
			{
				var check = await _UOW.JobProfileRepository.GetByName(updateJobProfile.Name);
				if (check) throw new Exception("Tên hố sơ công việc đã tồn tại. Vui lòng nhập tên khác");
			}
			return await _UOW.JobProfileRepository.Update(updateJobProfile);
		}

		public async Task<UpdateJobProfileDTO> GetById(long id)
		{
			return await _UOW.JobProfileRepository.GetById(id);
		}

		public async Task<List<JobProfileDTO>> GetAll()
		{
			return await _UOW.JobProfileRepository.GetAll();
		}
		public async Task<List<CategoryProfileDTO>> GetCategoryProfile()
		{
			return await _UOW.CategoryProfileRepository.GetAllActive();
		}

		public async Task<CustomPaging<JobProfileDTO>> GetCustomPaging(string? filter, int page, int pageSize)
		{
			filter = (filter ?? "");
			return await _UOW.JobProfileRepository.GetCustomPaging(filter, page, pageSize);
		}
	}
}
