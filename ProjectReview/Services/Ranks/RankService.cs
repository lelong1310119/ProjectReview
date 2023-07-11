using ProjectReview.DTO.Ranks;
using ProjectReview.Paging;
using ProjectReview.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ProjectReview.Services.Ranks
{
    public interface IRankService
    {
        Task Delete(long id);
        Task Active(long id);
        Task<RankDTO> Create(CreateRankDTO createRank);
        Task<RankDTO> Update(UpdateRankDTO updateRank);
        Task<UpdateRankDTO> GetById(long id);
        Task<List<RankDTO>> GetAll();
        Task<CustomPaging<RankDTO>> GetCustomPaging(string? filter, int page, int pageSize);
    }
    public class RankService : IRankService
    {
        private readonly IUnitOfWork _UOW;
        public RankService(IUnitOfWork unitOfWork)
        {
            _UOW = unitOfWork;
        }

        public async Task Delete(long id)
        {
            await _UOW.RankRepository.Delete(id);
        }

        public async Task Active(long id)
        {
            await _UOW.RankRepository.Active(id);
        }

        public async Task<RankDTO> Create(CreateRankDTO createRank)
        {
            createRank.Name = createRank.Name.Trim();
            var check = await _UOW.RankRepository.GetByName(createRank.Name);
            if (check) throw new Exception("Tên cấp bậc đã tồn tại. Vui lòng nhập tên khác");
            return await _UOW.RankRepository.Create(createRank);
        }

        public async Task<RankDTO> Update(UpdateRankDTO updateRank)
        {
            var rank = await _UOW.RankRepository.GetById(updateRank.Id);
            updateRank.Name = updateRank.Name.Trim();
            if (updateRank.Name != rank.Name)
            {
                var check = await _UOW.RankRepository.GetByName(updateRank.Name);
                if (check) throw new Exception("Tên cấp bậc đã tồn tại. Vui lòng nhập tên khác");
            }
            return await _UOW.RankRepository.Update(updateRank);
        }

        public async Task<UpdateRankDTO> GetById(long id)
        {
            return await _UOW.RankRepository.GetById(id);
        }

        public async Task<List<RankDTO>> GetAll()
        {
            return await _UOW.RankRepository.GetAll();
        }

        public async Task<CustomPaging<RankDTO>> GetCustomPaging(string? filter, int page, int pageSize)
        {
            filter = (filter ?? "");
            return await _UOW.RankRepository.GetCustomPaging(filter, page, pageSize);
        }
    }
}
