using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.DTO.Positions;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;

namespace ProjectReview.Repositories
{
	public interface IPositionRepository
	{
		Task Delete(long id);
		Task Active(long id);
		Task<bool> GetByName(string name);
        Task<PositionDTO> Create(CreatePositionDTO createPosition);
		Task<PositionDTO> Update(UpdatePositionDTO updatePosition);
		Task<UpdatePositionDTO> GetById(long id);
		Task<List<PositionDTO>> GetAll();
        Task<List<PositionDTO>> GetAllActive();
        Task<CustomPaging<PositionDTO>> GetCustomPaging(string filter, int page, int pageSize);
	}

	public class PositionRepository : IPositionRepository
	{
		private readonly DataContext _dataContext;
		private readonly IMapper _mapper;

		public PositionRepository(DataContext dataContext, IMapper mapper)
		{
			_dataContext = dataContext;
			_mapper = mapper;
		}

        public async Task<bool> GetByName(string name)
        {
            name = name.Trim();
            var result = await _dataContext.Positions
                                    .Where(x => x.Name == name)
                                    .FirstOrDefaultAsync();
            if (result == null) return false;
            return true;
        }

        public async Task Delete(long id)
		{
			var result = await _dataContext.Positions
							.Where(x => x.Id == id)
							.FirstOrDefaultAsync();
			if (result == null) return;
			_dataContext.Remove(result);
			await _dataContext.SaveChangesAsync();
		}

		public async Task Active(long id)
		{
			var result = await _dataContext.Positions
							.Where(x => x.Id == id)
							.FirstOrDefaultAsync();
			if (result == null) return;
			result.Status = 1;
			_dataContext.Positions.Update(result);
			await _dataContext.SaveChangesAsync();
		}

		public async Task<PositionDTO> Create(CreatePositionDTO createPosition)
		{
			var position = _mapper.Map<CreatePositionDTO, Position>(createPosition);
			position.CreateDate = DateTime.Now;
			await _dataContext.Positions.AddAsync(position);
			await _dataContext.SaveChangesAsync();
			return _mapper.Map<Position, PositionDTO>(position);
		}

		public async Task<PositionDTO> Update(UpdatePositionDTO updatePosition)
		{
			var position = await _dataContext.Positions
									.Where(x => x.Id == updatePosition.Id)
									.FirstOrDefaultAsync();
			if (position == null) return null;

			position.Name = updatePosition.Name;
			position.Note = updatePosition.Note;

			_dataContext.Positions.Update(position);
			await _dataContext.SaveChangesAsync();
			return _mapper.Map<Position, PositionDTO>(position);
		}

		public async Task<UpdatePositionDTO> GetById(long id)
		{
			var position = await _dataContext.Positions
									.Where(x => x.Id == id)
									.FirstOrDefaultAsync();
			if (position == null) return null;	
			return _mapper.Map<Position, UpdatePositionDTO>(position);
		}

		public async Task<List<PositionDTO>> GetAll()
		{
			var result = await _dataContext.Positions.ToListAsync();
			return _mapper.Map<List<Position>, List<PositionDTO>>(result);
		}

        public async Task<List<PositionDTO>> GetAllActive()
        {
            var result = await _dataContext.Positions
									.Where(x => x.Status == 1)
									.ToListAsync();
            return _mapper.Map<List<Position>, List<PositionDTO>>(result);
        }

        public async Task<CustomPaging<PositionDTO>> GetCustomPaging(string filter, int page, int pageSize)
		{
			int count = await _dataContext.Positions
										.Where(x => x.Name.Contains(filter))
										.CountAsync();
			var result = await _dataContext.Positions
										.Where(x => x.Name.Contains(filter))
										.Skip((page - 1) * pageSize)
										.Take(pageSize)
										.ToListAsync();
			int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
			var positions = _mapper.Map<List<Position>, List<PositionDTO>>(result);
			CustomPaging<PositionDTO> paging = new CustomPaging<PositionDTO>
			{
				TotalPage = totalPage,
				PageSize = pageSize,
				Data = positions
			};
			return paging;
		}
	}
}
