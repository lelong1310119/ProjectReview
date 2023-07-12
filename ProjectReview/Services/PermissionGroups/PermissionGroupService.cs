using ProjectReview.DTO.PermissionGroups;
using ProjectReview.Paging;
using ProjectReview.Repositories;
using Microsoft.AspNetCore.Mvc;

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
			createPermissionGroup.Name = createPermissionGroup.Name.Trim();
			var check = await _UOW.PermissionGroupRepository.GetByName(createPermissionGroup.Name);
			if (check) throw new Exception("Tên nhóm đã tồn tại. Vui lòng nhập tên khác");
			return await _UOW.PermissionGroupRepository.Create(createPermissionGroup);
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
