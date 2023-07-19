using ProjectReview.DTO.PermissionGroups;
using ProjectReview.Paging;
using ProjectReview.Repositories;

namespace ProjectReview.Services.PermissionGroups
{
	public interface IPermissionGroupService
	{
		Task Delete(long id);
		Task Active(long id);
		Task<PermissionGroupDTO> Create(CreatePermissionGroupDTO createPermissionGroup);
		Task<PermissionGroupDTO> Update(UpdatePermissionGroupDTO updatePermissionGroup);
		Task<UpdatePermissionGroupDTO> GetById(long id);
		Task<List<PermissionGroupDTO>> GetAll();
		Task<CustomPaging<PermissionGroupDTO>> GetCustomPaging(int page, int pageSize);
	}
	public class PermissionGroupService : IPermissionGroupService
	{
		private readonly IUnitOfWork _UOW;
		public PermissionGroupService(IUnitOfWork unitOfWork)
		{
			_UOW = unitOfWork;
		}

		public async Task Delete(long id)
		{
			await _UOW.PermissionGroupRepository.Delete(id);
		}

		public async Task Active(long id)
		{
			await _UOW.PermissionGroupRepository.Active(id);
		}

		public async Task<PermissionGroupDTO> Create(CreatePermissionGroupDTO createPermissionGroup)
		{
			List<long> RoleIds = new List<long>();
			createPermissionGroup.Name = createPermissionGroup.Name.Trim();
			var check = await _UOW.PermissionGroupRepository.GetByName(createPermissionGroup.Name);
			if (check) throw new Exception("Tên nhóm đã tồn tại. Vui lòng nhập tên khác");
			if (createPermissionGroup.RoleIds == null || createPermissionGroup.RoleIds.Count == 0)
			{
				return await _UOW.PermissionGroupRepository.Create(createPermissionGroup);
			}
			var permission = await _UOW.PermissionGroupRepository.Create(createPermissionGroup);
			if (permission != null)
			{
                RoleIds = createPermissionGroup.RoleIds;
				await _UOW.PermissionGroupRepository.CreateRolePermission(RoleIds, permission.Id);
            }
			return permission;
		}

		public async Task<PermissionGroupDTO> Update(UpdatePermissionGroupDTO updatePermissionGroup)
		{
			var PermissionGroup = await _UOW.PermissionGroupRepository.GetById(updatePermissionGroup.Id);
			updatePermissionGroup.Name = updatePermissionGroup.Name.Trim();
			if (updatePermissionGroup.Name != PermissionGroup.Name)
			{
				var check = await _UOW.PermissionGroupRepository.GetByName(updatePermissionGroup.Name);
				if (check) throw new Exception("Tên nhóm đã tồn tại. Vui lòng nhập tên khác");
			}
			if (updatePermissionGroup.ListRole == null) updatePermissionGroup.ListRole = new List<long>();
			await _UOW.PermissionGroupRepository.UpdateUseRole(updatePermissionGroup.Id, updatePermissionGroup.ListRole);
			await _UOW.PermissionGroupRepository.CreateRolePermission(updatePermissionGroup.ListRole, updatePermissionGroup.Id);
			return await _UOW.PermissionGroupRepository.Update(updatePermissionGroup);
		}

		public async Task<UpdatePermissionGroupDTO> GetById(long id)
		{
			return await _UOW.PermissionGroupRepository.GetById(id);
		}

		public async Task<List<PermissionGroupDTO>> GetAll()
		{
			return await _UOW.PermissionGroupRepository.GetAll();
		}

		public async Task<CustomPaging<PermissionGroupDTO>> GetCustomPaging(int page, int pageSize)
		{
			return await _UOW.PermissionGroupRepository.GetCustomPaging(page, pageSize);
		}
	}
}
