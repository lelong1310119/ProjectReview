using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Common;
using ProjectReview.DTO.DocumentTypes;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;

namespace ProjectReview.Repositories
{
    public interface IDocumentTypeRepository
    {
        Task Delete(long id);
        Task Active(long id);
        Task<bool> GetByName(string name);
        Task<DocumentTypeDTO> Create(CreateDocumentTypeDTO createDocumentType);
        Task<DocumentTypeDTO> Update(UpdateDocumentTypeDTO updateDocumentType);
        Task<UpdateDocumentTypeDTO> GetById(long id);
        Task<List<DocumentTypeDTO>> GetAll();
        Task<List<DocumentTypeDTO>> GetAllActive();
        Task<CustomPaging<DocumentTypeDTO>> GetCustomPaging(string filter, int page, int pageSize);
    }

    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public DocumentTypeRepository(DataContext dataContext, IMapper mapper, ICurrentUser currentUser)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<bool> GetByName(string name)
        {
            name = name.Trim();
            var result = await _dataContext.DocumentTypes
                                    .Where(x => x.Name == name)
                                    .FirstOrDefaultAsync();
            if (result == null) return false;
            return true;
        }

        public async Task Delete(long id)
        {
            var result = await _dataContext.DocumentTypes
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
            if (result == null) return;
            _dataContext.Remove(result);
            await _dataContext.SaveChangesAsync();
        }

        public async Task Active(long id)
        {
            var result = await _dataContext.DocumentTypes
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
            if (result == null) return;
            result.Status = 1;
            _dataContext.DocumentTypes.Update(result);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<DocumentTypeDTO> Create(CreateDocumentTypeDTO createDocumentType)
        {
            var documentType = _mapper.Map<CreateDocumentTypeDTO, DocumentType>(createDocumentType);
            long maxId = await _dataContext.DocumentTypes.MaxAsync(x => x.Id);
            documentType.Id = maxId + 1;
            documentType.CreateDate = DateTime.Now;
            documentType.CreateUserId = _currentUser.UserId;
            await _dataContext.DocumentTypes.AddAsync(documentType);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<DocumentType, DocumentTypeDTO>(documentType);
        }

        public async Task<DocumentTypeDTO> Update(UpdateDocumentTypeDTO updateDocumentType)
        {
            var documentType = await _dataContext.DocumentTypes
                                    .Where(x => x.Id == updateDocumentType.Id)
                                    .FirstOrDefaultAsync();
            if (documentType == null) return null;

            documentType.Name = updateDocumentType.Name;
            documentType.Note = updateDocumentType.Note;

            _dataContext.DocumentTypes.Update(documentType);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<DocumentType, DocumentTypeDTO>(documentType);
        }

        public async Task<UpdateDocumentTypeDTO> GetById(long id)
        {
            var documentType = await _dataContext.DocumentTypes
                                    .Where(x => x.Id == id)
                                    .FirstOrDefaultAsync();
            if (documentType == null) return null;
            return _mapper.Map<DocumentType, UpdateDocumentTypeDTO>(documentType);
        }

        public async Task<List<DocumentTypeDTO>> GetAll()
        {
            var result = await _dataContext.DocumentTypes
                                .Include(x => x.CreateUser)
                                .ToListAsync();
            return _mapper.Map<List<DocumentType>, List<DocumentTypeDTO>>(result);
        }

        public async Task<List<DocumentTypeDTO>> GetAllActive()
        {
            var result = await _dataContext.DocumentTypes
                                .Where(x => x.Status == 1)
                                .Include(x => x.CreateUser)
                                .ToListAsync();
            return _mapper.Map<List<DocumentType>, List<DocumentTypeDTO>>(result);
        }

        public async Task<CustomPaging<DocumentTypeDTO>> GetCustomPaging(string filter, int page, int pageSize)
        {
            int count = await _dataContext.DocumentTypes
                                        .Where(x => x.Name.Contains(filter))
                                        .CountAsync();
            var result = await _dataContext.DocumentTypes
                                        .Where(x => x.Name.Contains(filter))
                                        .Include(x => x.CreateUser)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
            int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
            var DocumentTypes = _mapper.Map<List<DocumentType>, List<DocumentTypeDTO>>(result);
            CustomPaging<DocumentTypeDTO> paging = new CustomPaging<DocumentTypeDTO>
            {
                TotalPage = totalPage,
                PageSize = pageSize,
                Data = DocumentTypes
            };
            return paging;
        }
    }
}
