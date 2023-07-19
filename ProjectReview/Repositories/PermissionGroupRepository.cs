using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
		Task CreateRolePermission(List<long> RoleId, long permissionId);
		Task UpdateUseRole(long id, List<long> RoleId);
		Task DeletePermissionRole(long id);
    }

    public class PermissionGroupRepository : IPermissionGroupRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public PermissionGroupRepository(DataContext dataContext, IMapper mapper) { 
            _dataContext = dataContext;
            _mapper = mapper;
        }

		public async Task CreateRolePermission(List<long> RoleId, long permissionId)
		{
			var result = await _dataContext.RolePermissions.Where(x => x.PermissionGroupId == permissionId).ToListAsync();
			_dataContext.RolePermissions.RemoveRange(result);
			foreach(var role in RoleId)
			{
				await _dataContext.RolePermissions.AddAsync(new RolePermission { RoleId = role, PermissionGroupId = permissionId });
			}
			await _dataContext.SaveChangesAsync();
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
			var permissionGroup = _mapper.Map<CreatePermissionGroupDTO, PermissionGroup>(createPermissionGroup);
            long maxId = await _dataContext.PermissionGroups.MaxAsync(x => x.Id);
            permissionGroup.Id = maxId + 1;
            permissionGroup.CreateDate = DateTime.Now;
			await _dataContext.PermissionGroups.AddAsync(permissionGroup);
			await _dataContext.SaveChangesAsync();
			return _mapper.Map<PermissionGroup, PermissionGroupDTO>(permissionGroup);
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

		public async Task DeletePermissionRole(long id)
		{
			await _dataContext.RolePermissions
						.Where(x => x.PermissionGroupId == id)
						.ExecuteDeleteAsync();
			await _dataContext.SaveChangesAsync();
		}

		public async Task UpdateUseRole(long id, List<long> RoleId)
		{
			List<long> UserId = new List<long>();
			var result = await _dataContext.Users
									.Where(x => x.PermissionGroupId == id)
									.ToListAsync();
			if (result.Count > 0) { 
				foreach(var user in result)
				{
					UserId.Add(user.Id);
				}
			}
			if (UserId.Count > 0) {
				foreach(var user in UserId)
				{
					var userRole = await _dataContext.UserRoles.Where(x => x.UserId == user).ToListAsync();
					_dataContext.UserRoles.RemoveRange(userRole);
					if (RoleId.Count > 0)
					{
                        foreach (var role in RoleId)
                        {
                            await _dataContext.UserRoles.AddAsync(new UserRole { RoleId = role, UserId = user });
                        }
                    }
				}
			}
			await _dataContext.SaveChangesAsync();
		}

		public async Task<UpdatePermissionGroupDTO> GetById(long id)
		{
			var permissionGroup = await _dataContext.PermissionGroups
									.Where(x => x.Id == id)
									.FirstOrDefaultAsync();
			if (permissionGroup == null) return null;
			var result = _mapper.Map<PermissionGroup, UpdatePermissionGroupDTO>(permissionGroup);
			var ListRole = await _dataContext.RolePermissions
								.Where(x => x.PermissionGroupId == id)
								.ToListAsync();
            result.ListRole = new List<long>();
            if (ListRole.Count > 0)
			{
				foreach (var role in ListRole) {
					result.ListRole.Add(role.RoleId);
				}
			}
			return result;
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
