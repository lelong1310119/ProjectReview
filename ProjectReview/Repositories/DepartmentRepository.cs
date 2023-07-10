using AutoMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ProjectReview.DTO.Departments;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;

namespace ProjectReview.Repositories
{
	public interface IDepartmentRepository
	{
		Task<bool> Delete(long id);
		Task<DepartmentDTO> Create(CreateDepartmentDTO createDepartment);
		Task<DepartmentDTO> Update(UpdateDepartmentDTO updateDepartment);
		Task<DepartmentDTO> GetById(long id);
		Task<List<DepartmentDTO>> GetAll();
		Task<CustomPaging<DepartmentDTO>> GetCustomPaging(string filter, int page, int pageSize);
	}

	public class DepartmentRepository : IDepartmentRepository
	{
		private readonly DataContext _dataContext;
		private readonly IMapper _mapper;

		public DepartmentRepository(DataContext dataContext, IMapper mapper)
		{
			_dataContext = dataContext;
			_mapper = mapper;
		}

		public async Task<bool> Delete(long id)
		{
			return true;
		}

		public async Task<DepartmentDTO> Create(CreateDepartmentDTO createDepartment) {
			var department = _mapper.Map<CreateDepartmentDTO, Department>(createDepartment);
			department.CreateDate = DateTime.Now;
			await _dataContext.Departments.AddAsync(department);
			await _dataContext.SaveChangesAsync();
			return _mapper.Map<Department, DepartmentDTO>(department);
		}

		public async Task<DepartmentDTO> Update(UpdateDepartmentDTO updateDepartment)
		{
			var department = await _dataContext.Departments
									.Where(x => x.Id == updateDepartment.Id)
									.FirstOrDefaultAsync();
			if (department == null) return null;
			
			department.Name = updateDepartment.Name;
			department.Phone = updateDepartment.Phone;
			department.Status = updateDepartment.Status;
			department.Address = updateDepartment.Address;

			_dataContext.Departments.Update(department);
			await _dataContext.SaveChangesAsync();
			return _mapper.Map<Department, DepartmentDTO>(department);
		}

		public async Task<DepartmentDTO> GetById(long id)
		{
			var department = await _dataContext.Departments
									.Where(x => x.Id == id)
									.FirstOrDefaultAsync();
			if (department == null) return null;
			return _mapper.Map<Department, DepartmentDTO>(department);
		}

		public async Task<List<DepartmentDTO>> GetAll()
		{
			var result = await _dataContext.Departments.ToListAsync();
			return _mapper.Map<List<Department>, List<DepartmentDTO>>(result);
		}

		public async Task<CustomPaging<DepartmentDTO>> GetCustomPaging(string filter, int page, int pageSize)
		{
			int count = await _dataContext.Departments
										.Where(x => x.Name.Contains(filter))
										.CountAsync();
			var result = await _dataContext.Departments
										.Where(x => x.Name.Contains(filter))
										.Skip((page - 1) * pageSize)
										.Take(pageSize)
										.ToListAsync();
			int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
			var departments = _mapper.Map<List<Department>, List<DepartmentDTO>>(result);
			CustomPaging<DepartmentDTO> paging = new CustomPaging<DepartmentDTO>
			{
				TotalPage = totalPage,
				PageSize = pageSize,
				Data = departments
			};
			return paging;
		}
	}
}
