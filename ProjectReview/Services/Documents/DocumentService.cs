using ProjectReview.DTO.Departments;
using ProjectReview.DTO.Documents;
using ProjectReview.Models.Entities;
using ProjectReview.Models;
using ProjectReview.Paging;
using ProjectReview.Repositories;
using ProjectReview.DTO.DocumentTypes;
using ProjectReview.DTO.Jobs;
using ProjectReview.DTO.JobProfiles;

namespace ProjectReview.Services.Documents
{
	public interface IDocumentService
	{
		Task<CustomPaging<DocumentDTO>> GetListDocumentSent(string? filter, int page, int pageSize);
		Task<CustomPaging<DocumentDTO>> GetListDocumentReceived(string? filter, int page, int pageSize);
        Task<List<Density>> GetListDensity();
        Task<List<Urgency>> GetListUrgency();
		Task<List<DocumentTypeDTO>> GetListDocument();
		Task<DocumentDTO> CreateDocumentSent(CreateDocumentDTO createDocumentDTO);
		Task<DocumentDTO> CreateDocumentReceived(CreateDocumentDTO createDocumentDTO);
		Task Delete(long id);
		Task<DocumentDTO> GetById(long id);
		Task Assign(AssignDocumentDTO assignDocumentDTO);
		Task Recall(long id);
		Task<UpdateDocumentDTO> GetUpdate(long id);
		Task<List<JobProfileDTO>> GetListProfile();
    }
	public class DocumentService : IDocumentService
	{
		private readonly IUnitOfWork _UOW;
		public DocumentService(IUnitOfWork unitOfWork)
		{
			_UOW = unitOfWork;
		}

		public async Task<UpdateDocumentDTO> GetUpdate(long id)
		{
			return await _UOW.DocumentRepository.GetUpdate(id);
		}

		public async Task<DocumentDTO> Update(UpdateDocumentDTO updateDocumentDTO)
		{
			return await _UOW.DocumentRepository.Update(updateDocumentDTO);
		}

		public async Task Recall(long id)
		{
			long jobId = await _UOW.DocumentRepository.GetJobId(id);
			if (jobId !=0 )
			{
				await _UOW.OpinionRepository.Delete(jobId);
			}
			await _UOW.DocumentRepository.Recall(id);
		}

		public async Task<DocumentDTO> GetById(long id)
		{
			return await _UOW.DocumentRepository.GetById(id);
		}
			 
		public async Task Delete(long id)
		{
			await _UOW.DocumentRepository.Delete(id);
		}

		public async Task<List<JobProfileDTO>> GetListProfile()
		{
			return await _UOW.JobProfileRepository.GetAllActive();
		}

		public async Task<List<DocumentTypeDTO>> GetListDocument()
		{
			return await _UOW.DocumentTypeRepository.GetAllActive();
		}

        public async Task<List<Density>> GetListDensity()
        {
            return await _UOW.DocumentRepository.GetListDensity();
        }

        public async Task<List<Urgency>> GetListUrgency()
        {
            return await _UOW.DocumentRepository.GetListUrgency();
        }

		public async Task<DocumentDTO> CreateDocumentReceived(CreateDocumentDTO createDocumentDTO)
		{
			List<long> profile = createDocumentDTO.ProfileIds;
			if (createDocumentDTO.FormFile != null) ;
			var document = await _UOW.DocumentRepository.CreateDocumentReceived(createDocumentDTO);
			if (profile != null && profile.Count > 0) {
				await _UOW.ProfileDocumentRepository.Create(document.Id, profile);
			}
			if (createDocumentDTO.FormFile != null)
			{
				document.FileName = createDocumentDTO.FormFile.FileName;
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", "Document_" + document.Id.ToString() + "_" + document.FileName);
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await createDocumentDTO.FormFile.CopyToAsync(fileStream);
				}
				document.FilePath = "Document_" + document.Id.ToString() + "_" + document.FileName;
				return await _UOW.DocumentRepository.UpdateFile(document);
			}
			return document;
		}

		public async Task<DocumentDTO> CreateDocumentSent(CreateDocumentDTO createDocumentDTO)
		{
			List<long> profile = createDocumentDTO.ProfileIds;
			if (createDocumentDTO.FormFile != null) ;
			var document = await _UOW.DocumentRepository.CreateDocumentSent(createDocumentDTO);
			if (profile != null && profile.Count > 0)
			{
				await _UOW.ProfileDocumentRepository.Create(document.Id, profile);
			}
			if (createDocumentDTO.FormFile != null)
			{
				document.FileName = createDocumentDTO.FormFile.FileName;
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", "Document_" + document.Id.ToString() + "_" + document.FileName);
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await createDocumentDTO.FormFile.CopyToAsync(fileStream);
				}
				document.FilePath = "Document_" + document.Id.ToString() + "_" + document.FileName;
				return await _UOW.DocumentRepository.UpdateFile(document);
			}
			return document;
		}

		public async Task Assign(AssignDocumentDTO assignDocumentDTO)
		{
			if (assignDocumentDTO.ListUserId == null || assignDocumentDTO.ListUserId.Count == 0)
			{
				throw new Exception("Bạn chưa chọn người xử lý công việc.");
			}
			createDocumentDTO createDocumentDTO = new createDocumentDTO
			{
				Content = assignDocumentDTO.Content,
				FilePath = assignDocumentDTO.FilePath,
				FileName = assignDocumentDTO.FileName,
				Request = assignDocumentDTO.Request,
				Deadline = assignDocumentDTO.Deadline,
				HostId = assignDocumentDTO.HostId,
				InstructorId = assignDocumentDTO.InstructorId
			};
			var result = await _UOW.JobRepository.Create(createDocumentDTO);
			if (result != null)
			{
				await _UOW.JobRepository.CreateJobDocument(result.Id, assignDocumentDTO.DocumentId);
				await _UOW.JobUserRepository.Create(assignDocumentDTO.ListUserId, result.Id);
				await _UOW.DocumentRepository.Assign(assignDocumentDTO.DocumentId);
			}
		} 

		public async Task<CustomPaging<DocumentDTO>> GetListDocumentSent(string? filter, int page, int pageSize)
		{
			filter = (filter ?? "");
			var result = await _UOW.DocumentRepository.GetListDocumentSent(filter, page, pageSize);
			if (result.Data.Count > 0)
			{
				foreach (var document in result.Data)
				{
					document.Job = await _UOW.JobRepository.GetByDocument(document.Id);
				}
			}
			return result;
		}

		public async Task<CustomPaging<DocumentDTO>> GetListDocumentReceived(string? filter, int page, int pageSize)
		{
			filter = (filter ?? "");
			var result = await _UOW.DocumentRepository.GetListDocumentReceived(filter, page, pageSize);
			if (result.Data.Count > 0)
			{
				foreach (var document in result.Data)
				{
					document.Job = await _UOW.JobRepository.GetByDocument(document.Id);
				}
			}
			return result;
		}
	}
}