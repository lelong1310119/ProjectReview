using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.DTO.Ranks;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;

namespace ProjectReview.Repositories
{
    public interface IRankRepository
    {
        Task Delete(long id);
        Task Active(long id);
        Task<bool> GetByName(string name);
        Task<RankDTO> Create(CreateRankDTO createRank);
        Task<RankDTO> Update(UpdateRankDTO updateRank);
        Task<UpdateRankDTO> GetById(long id);
        Task<List<RankDTO>> GetAll();
        Task<List<RankDTO>> GetAllActive();
        Task<CustomPaging<RankDTO>> GetCustomPaging(string filter, int page, int pageSize);
    }

    public class RankRepository : IRankRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public RankRepository(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<bool> GetByName(string name)
        {
            name = name.Trim();
            var result = await _dataContext.Ranks
                                    .Where(x => x.Name == name)
                                    .FirstOrDefaultAsync();
            if (result == null) return false;
            return true;
        }

        public async Task Delete(long id)
        {
            var result = await _dataContext.Ranks
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
            if (result == null) return;
            _dataContext.Remove(result);
            await _dataContext.SaveChangesAsync();
        }

        public async Task Active(long id)
        {
            var result = await _dataContext.Ranks
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
            if (result == null) return;
            result.Status = 1;
            _dataContext.Ranks.Update(result);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<RankDTO> Create(CreateRankDTO createRank)
        {
            var rank = _mapper.Map<CreateRankDTO, Rank>(createRank);
            long maxId = await _dataContext.Ranks.MaxAsync(x => x.Id);
            rank.Id = maxId + 1;
            rank.CreateDate = DateTime.Now;
            await _dataContext.Ranks.AddAsync(rank);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<Rank, RankDTO>(rank);
        }

        public async Task<RankDTO> Update(UpdateRankDTO updateRank)
        {
            var rank = await _dataContext.Ranks
                                    .Where(x => x.Id == updateRank.Id)
                                    .FirstOrDefaultAsync();
            if (rank == null) return null;

            rank.Name = updateRank.Name;
            rank.Note = updateRank.Note;

            _dataContext.Ranks.Update(rank);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<Rank, RankDTO>(rank);
        }

        public async Task<UpdateRankDTO> GetById(long id)
        {
            var rank = await _dataContext.Ranks
                                    .Where(x => x.Id == id)
                                    .FirstOrDefaultAsync();
            if (rank == null) return null;
            return _mapper.Map<Rank, UpdateRankDTO>(rank);
        }

        public async Task<List<RankDTO>> GetAll()
        {
            var result = await _dataContext.Ranks.ToListAsync();
            return _mapper.Map<List<Rank>, List<RankDTO>>(result);
        }

        public async Task<List<RankDTO>> GetAllActive()
        {
            var result = await _dataContext.Ranks.Where(x => x.Status == 1).ToListAsync();
            return _mapper.Map<List<Rank>, List<RankDTO>>(result);
        }

        public async Task<CustomPaging<RankDTO>> GetCustomPaging(string filter, int page, int pageSize)
        {
            int count = await _dataContext.Ranks
                                        .Where(x => x.Name.Contains(filter))
                                        .CountAsync();
            var result = await _dataContext.Ranks
                                        .Where(x => x.Name.Contains(filter))
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
            int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
            var ranks = _mapper.Map<List<Rank>, List<RankDTO>>(result);
            CustomPaging<RankDTO> paging = new CustomPaging<RankDTO>
            {
                TotalPage = totalPage,
                PageSize = pageSize,
                Data = ranks
            };
            return paging;
        }
    }
}
