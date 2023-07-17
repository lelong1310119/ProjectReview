using ProjectReview.DTO.Departments;
using ProjectReview.DTO.Documents;
using ProjectReview.Models.Entities;
using ProjectReview.Models;
using ProjectReview.Paging;
using ProjectReview.Repositories;
using ProjectReview.DTO.DocumentTypes;

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

	}
	public class DocumentService : IDocumentService
	{
		private readonly IUnitOfWork _UOW;
		public DocumentService(IUnitOfWork unitOfWork)
		{
			_UOW = unitOfWork;
		}

		public async Task Delete(long id)
		{
			await _UOW.DocumentRepository.Delete(id);
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
			return await _UOW.DocumentRepository.CreateDocumentReceived(createDocumentDTO);
		}

		public async Task<DocumentDTO> CreateDocumentSent(CreateDocumentDTO createDocumentDTO)
		{
			return await _UOW.DocumentRepository.CreateDocumentReceived(createDocumentDTO);
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