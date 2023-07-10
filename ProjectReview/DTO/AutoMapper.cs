using AutoMapper;
using ProjectReview.DTO.Departments;
using ProjectReview.DTO.Positions;
using ProjectReview.DTO.Ranks;
using ProjectReview.Models.Entities;

namespace ProjectReview.DTO
{
	public class AutoMapper : Profile
	{
		public AutoMapper() { 
			CreateMap<CreateDepartmentDTO, Department>();
			CreateMap<UpdateDepartmentDTO, Department>();
			CreateMap<Department, DepartmentDTO>();
            CreateMap<Department, UpdateDepartmentDTO>();
			CreateMap<CreatePositionDTO, Position>();
			CreateMap<UpdatePositionDTO, Position>();
			CreateMap<Position, PositionDTO>();
			CreateMap<Position, UpdatePositionDTO>();
            CreateMap<CreateRankDTO, Rank>();
            CreateMap<UpdateRankDTO, Rank>();
            CreateMap<Rank, RankDTO>();
            CreateMap<Rank, UpdateRankDTO>();
        }
	}
}
