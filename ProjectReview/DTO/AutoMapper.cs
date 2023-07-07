using AutoMapper;
using ProjectReview.DTO.Departments;
using ProjectReview.Models.Entities;

namespace ProjectReview.DTO
{
	public class AutoMapper : Profile
	{
		public AutoMapper() { 
			CreateMap<CreateDepartmentDTO, Department>();
			CreateMap<UpdateDepartmentDTO, Department>();
			CreateMap<Department, DepartmentDTO>();
		}
	}
}
