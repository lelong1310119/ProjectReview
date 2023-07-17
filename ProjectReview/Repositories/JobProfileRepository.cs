using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Common;
using ProjectReview.DTO.JobProfiles;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;

namespace ProjectReview.Repositories
{
	public interface IJobProfileRepository
	{
		Task CloseList(long id);
		Task OpenList(long id);
		Task<List<JobProfileDTO>> GetListByProfile(long id);
		Task Delete(long id);
		Task Close(long id);
		Task Active(long id);
		Task<bool> GetByName(string name);
		Task<JobProfileDTO> Create(CreateJobProfileDTO createJobProfile);
		Task<JobProfileDTO> Update(UpdateJobProfileDTO updateJobProfile);
		Task<UpdateJobProfileDTO> GetById(long id);
		Task<List<JobProfileDTO>> GetAll();
		Task<List<JobProfileDTO>> GetAllActive();
		Task<CustomPaging<JobProfileDTO>> GetCustomPaging(string filter, int page, int pageSize);
		Task<int> QuantityProfile();

    }
	public class JobProfileRepository : IJobProfileRepository
	{
		private readonly DataContext _dataContext;
		private readonly IMapper _mapper;
		private readonly ICurrentUser _currentUser;

		public JobProfileRepository(DataContext dataContext, IMapper mapper, ICurrentUser currentUser)
		{
			_dataContext = dataContext;
			_mapper = mapper;
			_currentUser = currentUser;
		}

		public async Task Delete(long id)
		{
			var result = await _dataContext.JobProfiles
							.Where(x => x.Id == id)
							.FirstOrDefaultAsync();
			if (result == null) return;
			_dataContext.Remove(result);
			await _dataContext.SaveChangesAsync();
		}

		public async Task OpenList(long id)
		{
			var result = await _dataContext.JobProfiles
							.Where(x => x.ProfileId == id)
							.ToListAsync();
			if (result.Count == 0) return;
			foreach (var profile in result)
			{
				if (profile.Status == 2)
				{
					profile.Status = 0;
				}
				else
				{
					profile.Status = 1;
				}
				_dataContext.JobProfiles.Update(profile);
			}
			await _dataContext.SaveChangesAsync();
		}

		public async Task<int> QuantityProfile()
		{
			return await _dataContext.JobProfiles
								.Where(x => x.Status == 1)
								.CountAsync();
		}

		public async Task CloseList(long id)
		{
			var result = await _dataContext.JobProfiles
							.Where(x => x.ProfileId == id)
							.ToListAsync();
			if (result.Count == 0) return;
			foreach (var profile in result)
			{
				if (profile.Status == 0)
				{
					profile.Status = 2;
				}
				else
				{
					profile.Status = 3;
				}
				_dataContext.JobProfiles.Update(profile);
			}
			await _dataContext.SaveChangesAsync();
		}

		public async Task<List<JobProfileDTO>> GetListByProfile(long id)
		{
			var result = await _dataContext.JobProfiles
									.Where(x => x.ProfileId == id)
									.ToListAsync();
			return _mapper.Map<List<JobProfile>, List<JobProfileDTO>>(result);
		}

		public async Task<bool> GetByName(string name)
		{
			name = name.Trim();
			var result = await _dataContext.JobProfiles
									.Where(x => x.Name == name)
									.FirstOrDefaultAsync();
			if (result == null) return false;
			return true;
		}

		public async Task Active(long id)
		{
			var result = await _dataContext.JobProfiles
							.Where(x => x.Id == id)
							.FirstOrDefaultAsync();
			if (result == null) return;
			result.Status = 1;
			_dataContext.JobProfiles.Update(result);
			await _dataContext.SaveChangesAsync();
		}

		public async Task Close(long id)
		{
			var result = await _dataContext.JobProfiles
							.Where(x => x.Id == id)
							.FirstOrDefaultAsync();
			if (result == null) return;
			result.Status = 0;
			_dataContext.JobProfiles.Update(result);
			await _dataContext.SaveChangesAsync();
		}

		public async Task<JobProfileDTO> Create(CreateJobProfileDTO createJobProfile)
		{
			var jobProfile = _mapper.Map<CreateJobProfileDTO, JobProfile>(createJobProfile);
            long maxId = await _dataContext.JobProfiles.MaxAsync(x => x.Id);
            jobProfile.Id = maxId + 1;
            jobProfile.CreateDate = DateTime.Now;
			jobProfile.CreateUserId = _currentUser.UserId;
			await _dataContext.JobProfiles.AddAsync(jobProfile);
			await _dataContext.SaveChangesAsync();
			return _mapper.Map<JobProfile, JobProfileDTO>(jobProfile);
		}

		public async Task<JobProfileDTO> Update(UpdateJobProfileDTO updateJobProfile)
		{
			var jobProfile = await _dataContext.JobProfiles
									.Where(x => x.Id == updateJobProfile.Id)
									.FirstOrDefaultAsync();
			if (jobProfile == null) return null;

			jobProfile.Name = updateJobProfile.Name;
			jobProfile.StartDate = updateJobProfile.StartDate;
			jobProfile.EndDate = updateJobProfile.EndDate;
			jobProfile.Condition = updateJobProfile.Condition;
			jobProfile.ProfileId = updateJobProfile.ProfileId;
			jobProfile.NumberPaper = updateJobProfile.NumberPaper;

			_dataContext.JobProfiles.Update(jobProfile);
			await _dataContext.SaveChangesAsync();
			return _mapper.Map<JobProfile, JobProfileDTO>(jobProfile);
		}

		public async Task<UpdateJobProfileDTO> GetById(long id)
		{
			var jobProfile = await _dataContext.JobProfiles
									.Where(x => x.Id == id)
									.FirstOrDefaultAsync();
			if (jobProfile == null) return null;
			return _mapper.Map<JobProfile, UpdateJobProfileDTO>(jobProfile);
		}

		public async Task<List<JobProfileDTO>> GetAll()
		{
			var result = await _dataContext.JobProfiles
								.Include(x => x.CreateUser)
								.Include(x => x.Profile)
								.ToListAsync();
			return _mapper.Map<List<JobProfile>, List<JobProfileDTO>>(result);
		}

		public async Task<List<JobProfileDTO>> GetAllActive()
		{
			var result = await _dataContext.JobProfiles
								.Where(x => x.Status == 1)
								.Include(x => x.CreateUser)
								.Include(x => x.Profile)
								.ToListAsync();
			return _mapper.Map<List<JobProfile>, List<JobProfileDTO>>(result);
		}

		public async Task<CustomPaging<JobProfileDTO>> GetCustomPaging(string filter, int page, int pageSize)
		{
			int count = await _dataContext.JobProfiles
										.Where(x => x.Name.Contains(filter))
										.CountAsync();
			var result = await _dataContext.JobProfiles
										.Where(x => ((x.Name.Contains(filter)) && (x.Status < 2)))
										.Include(x => x.CreateUser)
										.Include(x => x.Profile)
										.Skip((page - 1) * pageSize)
										.Take(pageSize)
										.ToListAsync();
			int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
			var jobProfiles = _mapper.Map<List<JobProfile>, List<JobProfileDTO>>(result);
			CustomPaging<JobProfileDTO> paging = new CustomPaging<JobProfileDTO>
			{
				TotalPage = totalPage,
				PageSize = pageSize,
				Data = jobProfiles
			};
			return paging;
		}
	}
}
