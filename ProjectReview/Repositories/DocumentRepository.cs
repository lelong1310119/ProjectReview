using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Common;
using ProjectReview.DTO.Documents;
using ProjectReview.DTO.Jobs;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;

namespace ProjectReview.Repositories
{
    public interface IDocumentRepository
    {
        Task Delete(long id);
        Task Assign(long id);
        Task Recall(long id);
        Task<DocumentDTO> CreateDocumentReceived(CreateDocumentDTO createDocument);
        Task<DocumentDTO> CreateDocumentSent(CreateDocumentDTO createDocument);
        Task<DocumentDTO> Update(UpdateDocumentDTO updateDocument);
        Task<CustomPaging<DocumentDTO>> GetListDocumentSent(string filter, int page, int pageSize);
        Task<CustomPaging<DocumentDTO>> GetListDocumentReceived(string filter, int page, int pageSize);
        Task<List<Density>> GetListDensity();
        Task<List<Urgency>> GetListUrgency();
        Task<int> QuantityDocumentSent(DateTime date);
        Task<int> QuantityDocumentReceived(DateTime date);
        Task<DocumentDTO> GetById(long id);
        Task<UpdateDocumentDTO> GetUpdate(long id);
        Task<long> GetJobId(long id);
        Task<DocumentDTO> UpdateFile(DocumentDTO update);
    }

    public class DocumentRepository : IDocumentRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public DocumentRepository(DataContext dataContext, IMapper mapper, ICurrentUser currentUser)
        {
            _dataContext = dataContext; 
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<DocumentDTO> UpdateFile(DocumentDTO update)
        {
            var document = await _dataContext.Documents.Where(x => x.Id == update.Id).FirstOrDefaultAsync();
            document.FileName = update.FileName;
            document.FilePath = update.FilePath;
            _dataContext.Documents.Update(document);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<Document, DocumentDTO>(document);
        }
        public async Task<UpdateDocumentDTO> GetUpdate(long id)
        {
            var result = await _dataContext.Documents
                                    .Where(x => x.Id == id)
                                    .FirstOrDefaultAsync();
            if (result == null) return null;
            return _mapper.Map<Document, UpdateDocumentDTO>(result);
        }

        public async Task<DocumentDTO> GetById(long id)
        {
            var result = await _dataContext.Documents
                                    .Where(x => x.Id == id)
                                    .FirstOrDefaultAsync();
            if (result == null) return null;
            return _mapper.Map<Document, DocumentDTO>(result);
        }

        public async Task<List<Density>> GetListDensity()
        {
            return await _dataContext.Densities.ToListAsync();
        }

        public async Task<List<Urgency>> GetListUrgency()
        {
            return await _dataContext.Urgencies.ToListAsync();
        }

        public async Task Delete(long id)
        {
            await _dataContext.ProfileDocuments
                            .Where(x => x.DocumentId == id)
                            .ExecuteDeleteAsync();
            await _dataContext.Documents
                            .Where(x => x.Id == id)
                            .ExecuteDeleteAsync();
            await _dataContext.SaveChangesAsync();
        }

        public async Task<int> QuantityDocumentSent(DateTime date)
        {
            return await _dataContext.Documents
                                .Where(x => (x.CreateDate.Year == date.Year && x.Type == 1))
                                .CountAsync();
        }

        public async Task<int> QuantityDocumentReceived(DateTime date)
        {
            return await _dataContext.Documents
                                .Where(x => (x.CreateDate.Year == date.Year && x.Type == 0))
                                .CountAsync();
        }

        public async Task Assign(long id)
        {
            var result = await _dataContext.Documents
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
            if (result == null) return;
            result.IsAssign = true;
            _dataContext.Documents.Update(result);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<long> GetJobId(long id)
        {
			var result = await _dataContext.Documents
							.Where(x => x.Id == id)
							.FirstOrDefaultAsync();
			if (result == null) return 0;
			var jobDocument = await _dataContext.JobDocuments.Where(x => x.DocumentId == result.Id).FirstOrDefaultAsync();
			if (jobDocument != null)
			{
				return jobDocument.JobId;
			}
            return 0;
		}

        public async Task Recall(long id)
        {
            var result = await _dataContext.Documents
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
            if (result == null) return;
            var jobDocument = await _dataContext.JobDocuments.Where(x => x.DocumentId == result.Id).FirstOrDefaultAsync();
            if (jobDocument != null)
            {
                await _dataContext.JobDocuments.Where(x => x.DocumentId == result.Id).ExecuteDeleteAsync();
                await _dataContext.Handlers.Where(x => x.JobId == jobDocument.JobId).ExecuteDeleteAsync();
                await _dataContext.Jobs.Where(x => x.Id == jobDocument.JobId).ExecuteDeleteAsync();
            }
            if (result == null) return;
            result.IsAssign = false;
            _dataContext.Documents.Update(result);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<DocumentDTO> CreateDocumentSent(CreateDocumentDTO createDocument)
        {
            var document = _mapper.Map<CreateDocumentDTO, Document>(createDocument);
            long maxId = 0;
            if (await _dataContext.Documents.AnyAsync())
            {
                maxId = await _dataContext.Documents.MaxAsync(x => x.Id);
            }
            document.Id = maxId + 1;
            document.CreateDate = DateTime.Now;
            document.CreateUserId = _currentUser.UserId;
            document.IsAssign = false;
            document.Type = 1;
            await _dataContext.Documents.AddAsync(document);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<Document, DocumentDTO>(document);
        }

        public async Task<DocumentDTO> CreateDocumentReceived(CreateDocumentDTO createDocument)
        {
            var document = _mapper.Map<CreateDocumentDTO, Document>(createDocument);
            long maxId = 0;
            if (await _dataContext.Documents.AnyAsync())
            {
                maxId = await _dataContext.Documents.MaxAsync(x => x.Id);
            }
            document.Id = maxId + 1;
            document.CreateDate = DateTime.Now;
            document.CreateUserId = _currentUser.UserId;
            document.IsAssign = false;
            document.Type = 0;
            await _dataContext.Documents.AddAsync(document);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<Document, DocumentDTO>(document);
        }

        public async Task<DocumentDTO> Update(UpdateDocumentDTO updateDocument)
        {
            var document = await _dataContext.Documents
                                    .Where(x => x.Id == updateDocument.Id)
                                    .FirstOrDefaultAsync();
            if (document == null) return null;

            document.Number = updateDocument.Number;
            document.Author = updateDocument.Author;
            document.DateIssued = updateDocument.DateIssued;
            document.Content = updateDocument.Content;
            document.DocumentTypeId = updateDocument.DocumentTypeId;
            document.FileName = updateDocument.FileName;
            document.DensityId = updateDocument.DensityId;
            document.UrgencyId = updateDocument.UrgencyId;
            document.NumberPaper = updateDocument.NumberPaper;
            document.Language = updateDocument.Language;
            document.Signer = updateDocument.Signer;
            document.Position = updateDocument.Position;
            document.Note = updateDocument.Note;
            document.Symbol = updateDocument.Symbol;
            _dataContext.Documents.Update(document);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<Document, DocumentDTO>(document);
        }

        public async Task<CustomPaging<DocumentDTO>> GetListDocumentSent(string filter, int page, int pageSize)
        {
            int count = await _dataContext.Documents
                                        .Where(x =>( x.Content.Contains(filter) && x.Type == 1))
                                        .CountAsync();
            var result = await _dataContext.Documents
                                        .Where(x => (x.Content.Contains(filter) && x.Type == 1))
                                        .Include(x => x.CreateUser)
                                        .Include(x => x.DocumentType)
                                        .Include(x => x.Density)
                                        .Include(x => x.Urgency)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
            int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
            var Documents = _mapper.Map<List<Document>, List<DocumentDTO>>(result);
            CustomPaging<DocumentDTO> paging = new CustomPaging<DocumentDTO>
            {
                TotalPage = totalPage,
                PageSize = pageSize,
                Data = Documents
            };
            return paging;
        }

		public async Task<CustomPaging<DocumentDTO>> GetListDocumentReceived(string filter, int page, int pageSize)
		{
			int count = await _dataContext.Documents
										.Where(x => (x.Content.Contains(filter) && x.Type == 0))
										.CountAsync();
			var result = await _dataContext.Documents
										.Where(x => (x.Content.Contains(filter) && x.Type == 0))
										.Include(x => x.CreateUser)
										.Include(x => x.DocumentType)
										.Include(x => x.Density)
										.Include(x => x.Urgency)
										.Skip((page - 1) * pageSize)
										.Take(pageSize)
										.ToListAsync();
			int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
			var Documents = _mapper.Map<List<Document>, List<DocumentDTO>>(result);
			CustomPaging<DocumentDTO> paging = new CustomPaging<DocumentDTO>
			{
				TotalPage = totalPage,
				PageSize = pageSize,
				Data = Documents
			};
			return paging;
		}
	}
}
