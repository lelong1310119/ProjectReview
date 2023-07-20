using ProjectReview.DTO.Departments;
using ProjectReview.DTO.Documents;
using ProjectReview.Models.Entities;
using ProjectReview.Models;
using ProjectReview.Paging;
using ProjectReview.Repositories;
using ProjectReview.DTO.DocumentTypes;
using ProjectReview.DTO.Jobs;
using ProjectReview.DTO.JobProfiles;
using ProjectReview.DTO.Processes;

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
        Task<DocumentDTO> Update(UpdateDocumentDTO updateDocumentDTO);
		Task<AddProfileDTO> GetToMove(long id);
		Task UpdateProfile(AddProfileDTO addProfile);
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

        public async Task<AddProfileDTO> GetToMove(long id)
        {
            return await _UOW.DocumentRepository.GetToMove(id);
        }

        public async Task<DocumentDTO> Update(UpdateDocumentDTO updateDocumentDTO)
		{
            if (updateDocumentDTO.FormFile != null)
            {
                if (updateDocumentDTO.FilePath != null && updateDocumentDTO.FilePath != "")
                {
                    var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", updateDocumentDTO.FilePath);
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                }
                updateDocumentDTO.FileName = updateDocumentDTO.FormFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", "Document_" + updateDocumentDTO.Id.ToString() + "_" + updateDocumentDTO.FileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await updateDocumentDTO.FormFile.CopyToAsync(fileStream);
                }
                updateDocumentDTO.FilePath = "Document_" + updateDocumentDTO.Id.ToString() + "_" + updateDocumentDTO.FileName;
                if (updateDocumentDTO.IsAssign == true)
                {
                    await _UOW.JobRepository.UpdateFromDocument(updateDocumentDTO.Id, updateDocumentDTO.FileName, updateDocumentDTO.FilePath);
                }
            }
			if (updateDocumentDTO.ProfileIds == null)
			{
				updateDocumentDTO.ProfileIds = new List<long>();
			}
			await _UOW.ProfileDocumentRepository.Create(updateDocumentDTO.Id, updateDocumentDTO.ProfileIds);
            return await _UOW.DocumentRepository.Update(updateDocumentDTO);
		}

		public async Task UpdateProfile(AddProfileDTO addProfile) 
		{
			if (addProfile.ProfileIds == null)
			{
				addProfile.ProfileIds = new List<long>();
			}
			await _UOW.ProfileDocumentRepository.Create(addProfile.Id, addProfile.ProfileIds);
		}

		public async Task Recall(long id)
		{
			await _UOW.HistoryRepository.DeleteByDocument(id);
			await _UOW.DocumentRepository.Recall(id);
		}
			 
		public async Task Delete(long id)
		{
			await _UOW.HistoryRepository.DeleteByDocument(id);
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
            if (!await _UOW.UserRepository.CheckUser(assignDocumentDTO.ListUserId, assignDocumentDTO.InstructorId)) throw new Exception("Người xử lý phải cùng phòng với người chỉ đạo - theo dõi");
            CreateJobDTO createJobDTO = new CreateJobDTO
            {
                Content = assignDocumentDTO.Content,
                FilePath = assignDocumentDTO.FilePath,
                FileName = assignDocumentDTO.FileName,
                Request = assignDocumentDTO.Request,
                Deadline = assignDocumentDTO.Deadline,
                HostId = assignDocumentDTO.HostId,
                InstructorId = assignDocumentDTO.InstructorId
            };
			var result = await _UOW.JobRepository.Create(createJobDTO);
			if (result != null)
			{
				CreateProcessDTO process = new CreateProcessDTO
				{
					JobId = result.Id,
					InstructorId = result.InstructorId,
					UserId = assignDocumentDTO.ListUserId
				};
				await _UOW.ProcessRepository.CreateFromDocument(process);
				await _UOW.JobRepository.CreateJobDocument(result.Id, assignDocumentDTO.DocumentId);
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

		public async Task<DocumentDTO> GetById(long id)
		{
			var document = await _UOW.DocumentRepository.GetById(id);
			document.Job = await _UOW.JobRepository.GetByDocument(document.Id);
			return document;
		}
	}
}