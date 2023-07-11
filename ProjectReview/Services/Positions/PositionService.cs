using ProjectReview.DTO.Positions;
using ProjectReview.Paging;
using ProjectReview.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ProjectReview.Services.Positions
{
	public interface IPositionService
	{
		Task Delete(long id);
		Task Active(long id);
		Task<PositionDTO> Create(CreatePositionDTO createPosition);
		Task<PositionDTO> Update(UpdatePositionDTO updatePosition);
		Task<UpdatePositionDTO> GetById(long id);
		Task<List<PositionDTO>> GetAll();
		Task<CustomPaging<PositionDTO>> GetCustomPaging(string? filter, int page, int pageSize);
	}
	public class PositionService : IPositionService
	{
		private readonly IUnitOfWork _UOW;
		public PositionService(IUnitOfWork unitOfWork)
		{
			_UOW = unitOfWork;
		}

		public async Task Delete(long id)
		{
			await _UOW.PositionRepository.Delete(id);
		}

		public async Task Active(long id)
		{
			await _UOW.PositionRepository.Active(id);
		}

		public async Task<PositionDTO> Create(CreatePositionDTO createPosition)
		{
            createPosition.Name = createPosition.Name.Trim();
            var check = await _UOW.PositionRepository.GetByName(createPosition.Name);
            if (check) throw new Exception("Tên chức vụ đã tồn tại. Vui lòng nhập tên khác");
            return await _UOW.PositionRepository.Create(createPosition);
		}

		public async Task<PositionDTO> Update(UpdatePositionDTO updatePosition)
		{
            var position = await _UOW.PositionRepository.GetById(updatePosition.Id);
            updatePosition.Name = updatePosition.Name.Trim();
            if (updatePosition.Name != position.Name)
            {
                var check = await _UOW.PositionRepository.GetByName(updatePosition.Name);
                if (check) throw new Exception("Tên chức vụ đã tồn tại. Vui lòng nhập tên khác");
            }
            return await _UOW.PositionRepository.Update(updatePosition);
        }

		public async Task<UpdatePositionDTO> GetById(long id)
		{
			return await _UOW.PositionRepository.GetById(id);
		}

		public async Task<List<PositionDTO>> GetAll()
		{
			return await _UOW.PositionRepository.GetAll();
		}

		public async Task<CustomPaging<PositionDTO>> GetCustomPaging(string? filter, int page, int pageSize)
		{
			filter = (filter ?? "");
			return await _UOW.PositionRepository.GetCustomPaging(filter, page, pageSize);
		}
	}
}
