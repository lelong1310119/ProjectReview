using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Common;
using ProjectReview.DTO.Documents;
using ProjectReview.DTO.Jobs;
using ProjectReview.DTO.Opinions;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using System.IO;

namespace ProjectReview.Repositories
{
	public interface IOpinionRepository
	{
		Task Delete(long jobId);
		Task<OpinionDTO> Create(CreateOpinionDTO createOpinionDTO);
		Task<OpinionDTO> UpdateFile(OpinionDTO update);
		Task<List<OpinionDTO>> GetList();
    }

	public class OpinionRepository : IOpinionRepository
	{
		private readonly DataContext _dataContext;
		private readonly IMapper _mapper;
		private readonly ICurrentUser _currentUser;

		public OpinionRepository(DataContext dataContext, IMapper mapper, ICurrentUser currentUser)
		{
			_dataContext = dataContext;
			_mapper = mapper;
			_currentUser = currentUser;
		}	


		public async Task<OpinionDTO> Create(CreateOpinionDTO createOpinionDTO) 
		{
			var opinion = _mapper.Map<CreateOpinionDTO, Opinion>(createOpinionDTO);
            long maxId = 0;
            if (await _dataContext.Opinions.AnyAsync())
            {
                maxId = await _dataContext.Opinions.MaxAsync(x => x.Id);
            }
            opinion.Id = maxId + 1;
            opinion.CreateDate = DateTime.Now;
            opinion.CreateUserId = _currentUser.UserId;
            await _dataContext.Opinions.AddAsync(opinion);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<Opinion, OpinionDTO>(opinion);
        }

        public async Task<OpinionDTO> UpdateFile(OpinionDTO update)
        {
            var opinion = await _dataContext.Opinions.Where(x => x.Id == update.Id).FirstOrDefaultAsync();
            opinion.FileName = update.FileName;
            opinion.FilePath = update.FilePath;
            _dataContext.Opinions.Update(opinion);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<Opinion, OpinionDTO>(opinion);
        }

		public async Task<List<OpinionDTO>> GetList()
		{
			var result = await _dataContext.Opinions.ToListAsync();
			return _mapper.Map<List<Opinion>, List<OpinionDTO>>(result);
		}

        public async Task Delete(long jobId)
		{
			var result = await _dataContext.Opinions
									.Where(x => x.JobId == jobId)
									.ToListAsync();
			if (result.Count > 0) {
				foreach(var item in result)
				{
					if (item.FilePath != null && item.FilePath != "")
					{
						var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "file", item.FilePath);
						if (System.IO.File.Exists(filePath))
						{
							System.IO.File.Delete(filePath);
						}
					}
					_dataContext.Remove(item);
				}
			}
			await _dataContext.SaveChangesAsync();
		}
	}
}
