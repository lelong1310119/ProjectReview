using ProjectReview.DTO.DocumentTypes;
using ProjectReview.Paging;
using ProjectReview.Repositories;

namespace ProjectReview.Services.DocumentTypes
{
    public interface IDocumentTypeService
    {
        Task Delete(long id);
        Task Active(long id);
        Task<DocumentTypeDTO> Create(CreateDocumentTypeDTO createDocumentType);
        Task<DocumentTypeDTO> Update(UpdateDocumentTypeDTO updateDocumentType);
        Task<UpdateDocumentTypeDTO> GetById(long id);
        Task<List<DocumentTypeDTO>> GetAll();
        Task<CustomPaging<DocumentTypeDTO>> GetCustomPaging(string? filter, int page, int pageSize);
    }

    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly IUnitOfWork _UOW;
        public DocumentTypeService(IUnitOfWork unitOfWork)
        {
            _UOW = unitOfWork;
        }

        public async Task Delete(long id)
        {
            await _UOW.DocumentTypeRepository.Delete(id);
        }

        public async Task Active(long id)
        {
            await _UOW.DocumentTypeRepository.Active(id);
        }

        public async Task<DocumentTypeDTO> Create(CreateDocumentTypeDTO createDocumentType)
        {
            createDocumentType.Name = createDocumentType.Name.Trim();
            var check = await _UOW.DocumentTypeRepository.GetByName(createDocumentType.Name);
            if (check) throw new Exception("Tên loại văn bản đã tồn tại. Vui lòng nhập tên khác");
            return await _UOW.DocumentTypeRepository.Create(createDocumentType);
        }

        public async Task<DocumentTypeDTO> Update(UpdateDocumentTypeDTO updateDocumentType)
        {
            var documentType = await _UOW.DocumentTypeRepository.GetById(updateDocumentType.Id);
            updateDocumentType.Name = updateDocumentType.Name.Trim();
            if (updateDocumentType.Name != documentType.Name)
            {
                var check = await _UOW.DocumentTypeRepository.GetByName(updateDocumentType.Name);
                if (check) throw new Exception("Tên loại văn bản đã tồn tại. Vui lòng nhập tên khác");
            }
            return await _UOW.DocumentTypeRepository.Update(updateDocumentType);
        }

        public async Task<UpdateDocumentTypeDTO> GetById(long id)
        {
            return await _UOW.DocumentTypeRepository.GetById(id);
        }

        public async Task<List<DocumentTypeDTO>> GetAll()
        {
            return await _UOW.DocumentTypeRepository.GetAll();
        }

        public async Task<CustomPaging<DocumentTypeDTO>> GetCustomPaging(string? filter, int page, int pageSize)
        {
            filter = (filter ?? "");
            return await _UOW.DocumentTypeRepository.GetCustomPaging(filter, page, pageSize);
        }
    }
}
