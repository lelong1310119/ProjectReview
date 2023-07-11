using ProjectReview.DTO.Departments;
using ProjectReview.Paging;
using ProjectReview.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ProjectReview.Services.Departments
{
	public interface IDepartmentService
	{
		Task Delete(long id);
		Task Active(long id);
		Task<DepartmentDTO> Create(CreateDepartmentDTO createDepartment);
		Task<DepartmentDTO> Update(UpdateDepartmentDTO updateDepartment);
		Task<UpdateDepartmentDTO> GetById(long id);
		Task<List<DepartmentDTO>> GetAll();
		Task<CustomPaging<DepartmentDTO>> GetCustomPaging(string? filter, int page, int pageSize);
	}
	public class DepartmentService : IDepartmentService
	{
		private readonly IUnitOfWork _UOW;
		public DepartmentService(IUnitOfWork unitOfWork)
		{
			_UOW = unitOfWork;
		}

		public async Task Delete(long id)
		{
			await _UOW.DepartmentRepository.Delete(id);
		}

		public async Task Active(long id)
		{
			await _UOW.DepartmentRepository.Active(id);
		}

		public async Task<DepartmentDTO> Create(CreateDepartmentDTO createDepartment)
		{
            createDepartment.Name = createDepartment.Name.Trim();
            var check = await _UOW.DepartmentRepository.GetByName(createDepartment.Name);
			if (check) throw new Exception("Tên đơn vị đã tồn tại. Vui lòng nhập tên khác");
			return await _UOW.DepartmentRepository.Create(createDepartment);
		}

		public async Task<DepartmentDTO> Update(UpdateDepartmentDTO updateDepartment)
		{
			var department = await _UOW.DepartmentRepository.GetById(updateDepartment.Id);
			updateDepartment.Name = updateDepartment.Name.Trim();
			if (updateDepartment.Name != department.Name)
			{
				var check = await _UOW.DepartmentRepository.GetByName(updateDepartment.Name);
                if (check) throw new Exception("Tên đơn vị đã tồn tại. Vui lòng nhập tên khác");
            }
            return await _UOW.DepartmentRepository.Update(updateDepartment);
        }

		public async Task<UpdateDepartmentDTO> GetById(long id)
		{
			return await _UOW.DepartmentRepository.GetById(id);
		}

		public async Task<List<DepartmentDTO>> GetAll()
		{
			return await _UOW.DepartmentRepository.GetAll();
		}

		public async Task<CustomPaging<DepartmentDTO>> GetCustomPaging(string? filter, int page, int pageSize)
		{
			filter = (filter ?? "");
			return await _UOW.DepartmentRepository.GetCustomPaging(filter, page, pageSize);
		}
	} 
}
