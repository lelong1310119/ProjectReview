using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.DTO.PermissionGroups;
using ProjectReview.DTO.PermissionGroups;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;

namespace ProjectReview.Repositories
{
    public interface IPermissionGroupRepository
    {
		Task Delete(long id);
		Task Active(long id);
		Task<bool> GetByName(string name);
		Task<PermissionGroupDTO> Create(CreatePermissionGroupDTO createPermissionGroup);
		Task<PermissionGroupDTO> Update(UpdatePermissionGroupDTO updatePermissionGroup);
		Task<UpdatePermissionGroupDTO> GetById(long id);
		Task<List<PermissionGroupDTO>> GetAll();
		Task<CustomPaging<PermissionGroupDTO>> GetCustomPaging(int page, int pageSize);
		Task<List<PermissionGroupDTO>> GetAllActive();
    }

    public class PermissionGroupRepository : IPermissionGroupRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public PermissionGroupRepository(DataContext dataContext, IMapper mapper) { 
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<List<PermissionGroupDTO>> GetAllActive()
        {
            var result = await _dataContext.PermissionGroups
                                    .Where(x => x.Status == 1)
                                    .ToListAsync();
            return _mapper.Map<List<PermissionGroup>, List<PermissionGroupDTO>>(result);
        }

		public async Task<bool> GetByName(string name)
		{
			name = name.Trim();
			var result = await _dataContext.PermissionGroups
									.Where(x => x.Name == name)
									.FirstOrDefaultAsync();
			if (result == null) return false;
			return true;
		}

		public async Task Delete(long id)
		{
			var result = await _dataContext.PermissionGroups
							.Where(x => x.Id == id)
							.FirstOrDefaultAsync();
			if (result == null) return;
			_dataContext.Remove(result);
			await _dataContext.SaveChangesAsync();
		}

		public async Task Active(long id)
		{
			var result = await _dataContext.PermissionGroups
							.Where(x => x.Id == id)
							.FirstOrDefaultAsync();
			if (result == null) return;
			result.Status = 1;
			_dataContext.PermissionGroups.Update(result);
			await _dataContext.SaveChangesAsync();
		}

		public async Task<PermissionGroupDTO> Create(CreatePermissionGroupDTO createPermissionGroup)
		{
			var PermissionGroup = _mapper.Map<CreatePermissionGroupDTO, PermissionGroup>(createPermissionGroup);
			PermissionGroup.CreateDate = DateTime.Now;
			await _dataContext.PermissionGroups.AddAsync(PermissionGroup);
			await _dataContext.SaveChangesAsync();
			return _mapper.Map<PermissionGroup, PermissionGroupDTO>(PermissionGroup);
		}

		public async Task<PermissionGroupDTO> Update(UpdatePermissionGroupDTO updatePermissionGroup)
		{
			var permissionGroup = await _dataContext.PermissionGroups
									.Where(x => x.Id == updatePermissionGroup.Id)
									.FirstOrDefaultAsync();
			if (permissionGroup == null) return null;
			permissionGroup.Name = updatePermissionGroup.Name;
			_dataContext.PermissionGroups.Update(permissionGroup);
			await _dataContext.SaveChangesAsync();
			return _mapper.Map<PermissionGroup, PermissionGroupDTO>(permissionGroup);
		}

		public async Task<UpdatePermissionGroupDTO> GetById(long id)
		{
			var permissionGroup = await _dataContext.PermissionGroups
									.Where(x => x.Id == id)
									.FirstOrDefaultAsync();
			if (permissionGroup == null) return null;
			return _mapper.Map<PermissionGroup, UpdatePermissionGroupDTO>(permissionGroup);
		}

		public async Task<List<PermissionGroupDTO>> GetAll()
		{
			var result = await _dataContext.PermissionGroups.ToListAsync();
			return _mapper.Map<List<PermissionGroup>, List<PermissionGroupDTO>>(result);
		}

		public async Task<CustomPaging<PermissionGroupDTO>> GetCustomPaging(int page, int pageSize)
		{
			int count = await _dataContext.PermissionGroups
										.Where(x => x.Id != 1)
										.CountAsync();
			var result = await _dataContext.PermissionGroups
										.Where(x => x.Id != 1)
										.Skip((page - 1) * pageSize)
										.Take(pageSize)
										.ToListAsync();
			int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
			var PermissionGroups = _mapper.Map<List<PermissionGroup>, List<PermissionGroupDTO>>(result);
			CustomPaging<PermissionGroupDTO> paging = new CustomPaging<PermissionGroupDTO>
			{
				TotalPage = totalPage,
				PageSize = pageSize,
				Data = PermissionGroups
			};
			return paging;
		}
	}
}
