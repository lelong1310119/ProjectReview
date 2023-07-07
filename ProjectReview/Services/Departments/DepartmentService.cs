using ProjectReview.DTO.Departments;
using ProjectReview.Paging;
using ProjectReview.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ProjectReview.Services.Departments
{
	public interface IDepartmentService
	{
		Task<bool> Delete(long id);
		Task<DepartmentDTO> Create(CreateDepartmentDTO createDepartment);
		Task<DepartmentDTO> Update(UpdateDepartmentDTO updateDepartment);
		Task<DepartmentDTO> GetById(long id);
		Task<List<DepartmentDTO>> GetAll();
		Task<CustomPaging<DepartmentDTO>> GetCustomPaging(string filter, int page, int pageSize);
	}
	public class DepartmentService : IDepartmentService
	{
		private readonly IUnitOfWork _UOW;
		public DepartmentService(IUnitOfWork unitOfWork)
		{
			_UOW = unitOfWork;
		}

		public async Task<bool> Delete(long id)
		{
			return await _UOW.DepartmentRepository.Delete(id);
		}

		public async Task<DepartmentDTO> Create(CreateDepartmentDTO createDepartment)
		{
			return await _UOW.DepartmentRepository.Create(createDepartment);
		}

		public async Task<DepartmentDTO> Update(UpdateDepartmentDTO updateDepartment)
		{
			return await _UOW.DepartmentRepository.Update(updateDepartment);
		}

		public async Task<DepartmentDTO> GetById(long id)
		{
			return await _UOW.DepartmentRepository.GetById(id);
		}

		public async Task<List<DepartmentDTO>> GetAll()
		{
			return await _UOW.DepartmentRepository.GetAll();
		}

		public async Task<CustomPaging<DepartmentDTO>> GetCustomPaging(string filter, int page, int pageSize)
		{
			return await _UOW.DepartmentRepository.GetCustomPaging(filter, page, pageSize);
		}
	} 
}
