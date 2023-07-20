using AutoMapper;
using ProjectReview.DTO.CategoryProfiles;
using ProjectReview.DTO.Departments;
using ProjectReview.DTO.Documents;
using ProjectReview.DTO.DocumentTypes;
using ProjectReview.DTO.Histories;
using ProjectReview.DTO.JobProfiles;
using ProjectReview.DTO.Jobs;
using ProjectReview.DTO.Opinions;
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
			CreateMap<UpdatePermissionGroupDTO, PermissionGroupDTO>();
			CreateMap<PermissionGroup, PermissionGroupDTO>();
			CreateMap<CreatePermissionGroupDTO, PermissionGroup>();
			CreateMap<PermissionGroup, UpdatePermissionGroupDTO>();
            CreateMap<CreateCategoryProfileDTO, CategoryProfile>();
            CreateMap<UpdateCategoryProfileDTO, CategoryProfile>();
            CreateMap<CategoryProfile, CategoryProfileDTO>();
            CreateMap<CategoryProfile, UpdateCategoryProfileDTO>();
			CreateMap<JobProfile, JobProfileDTO>();
            CreateMap<CreateJobProfileDTO, JobProfile>();
            CreateMap<UpdateJobProfileDTO, JobProfile>();
            CreateMap<JobProfile, UpdateJobProfileDTO>();
			CreateMap<Job, JobDTO>();
			CreateMap<CreateJobDTO, Job>();
			CreateMap<UpdateJobDTO, JobDTO>();
			CreateMap<Job, UpdateJobDTO>();
			CreateMap<CreateDocumentDTO, Document>();
			CreateMap<UpdateDocumentTypeDTO, Document>();
			CreateMap<Document, DocumentDTO>();
			CreateMap<Document, UpdateDocumentDTO>();
            CreateMap<Document, AddProfileDTO>();
            CreateMap<Opinion, OpinionDTO>();
			CreateMap<CreateOpinionDTO, Opinion>();
			CreateMap<History, HistoryDTO>();
		}
	}
}
