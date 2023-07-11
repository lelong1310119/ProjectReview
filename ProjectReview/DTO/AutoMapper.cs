using AutoMapper;
using ProjectReview.DTO.Departments;
using ProjectReview.DTO.DocumentTypes;
using ProjectReview.DTO.PermissionGroups;
using ProjectReview.DTO.Positions;
using ProjectReview.DTO.Ranks;
using ProjectReview.DTO.Users;
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
			CreateMap<User, UserDTO>();
			CreateMap<CreateUserDTO, User>();
            CreateMap<CreateDocumentTypeDTO, DocumentType>();
            CreateMap<UpdateDocumentTypeDTO, DocumentType>();
            CreateMap<DocumentType, DocumentTypeDTO>();
            CreateMap<DocumentType, UpdateDocumentTypeDTO>();
			CreateMap<UpdateUserDTO, User>();
			CreateMap<User, UpdateUserDTO>();
			CreateMap<PermissionGroup, PermissionGroupDTO>();
        }
	}
}
