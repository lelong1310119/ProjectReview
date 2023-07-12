using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Common;
using ProjectReview.DTO.CategoryProfiles;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;

namespace ProjectReview.Repositories
{
    public interface ICategoryProfileRepository
    {
        Task Close(long id);
        Task Delete(long id);
        Task Active(long id);
        Task<bool> GetByName(string name);
        Task<CategoryProfileDTO> Create(CreateCategoryProfileDTO createCategoryProfile);
        Task<CategoryProfileDTO> Update(UpdateCategoryProfileDTO updateCategoryProfile);
        Task<UpdateCategoryProfileDTO> GetById(long id);
        Task<List<CategoryProfileDTO>> GetAll();
        Task<List<CategoryProfileDTO>> GetAllActive();
        Task<CustomPaging<CategoryProfileDTO>> GetCustomPaging(string filter, int page, int pageSize);
    }

    public class CategoryProfileRepository : ICategoryProfileRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public CategoryProfileRepository(DataContext dataContext, IMapper mapper, ICurrentUser currentUser)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<bool> GetByName(string name)
        {
            name = name.Trim();
            var result = await _dataContext.Profiles
                                    .Where(x => x.Title == name)
                                    .FirstOrDefaultAsync();
            if (result == null) return false;
            return true;
        }

        public async Task Delete(long id)
        {
            var result = await _dataContext.Profiles
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
            if (result == null) return;
            _dataContext.Remove(result);
            await _dataContext.SaveChangesAsync();
        }

        public async Task Active(long id)
        {
            var result = await _dataContext.Profiles
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
            if (result == null) return;
            result.Status = 1;
            _dataContext.Profiles.Update(result);
            await _dataContext.SaveChangesAsync();
        }

        public async Task Close(long id)
        {
            var result = await _dataContext.Profiles
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
            if (result == null) return;
            result.Status = 0;
            _dataContext.Profiles.Update(result);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<CategoryProfileDTO> Create(CreateCategoryProfileDTO createCategoryProfile)
        {
            var categoryProfile = _mapper.Map<CreateCategoryProfileDTO, CategoryProfile>(createCategoryProfile);
            long maxId = await _dataContext.Profiles.MaxAsync(x => x.Id);
            categoryProfile.Id = maxId + 1;
            categoryProfile.CreateDate = DateTime.Now;
            categoryProfile.CreateUserId = _currentUser.UserId;
            await _dataContext.Profiles.AddAsync(categoryProfile);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<CategoryProfile, CategoryProfileDTO>(categoryProfile);
        }

        public async Task<CategoryProfileDTO> Update(UpdateCategoryProfileDTO updateCategoryProfile)
        {
            var categoryProfile = await _dataContext.Profiles
                                    .Where(x => x.Id == updateCategoryProfile.Id)
                                    .FirstOrDefaultAsync();
            if (categoryProfile == null) return null;

            categoryProfile.Title = updateCategoryProfile.Title;
            categoryProfile.Symbol = updateCategoryProfile.Symbol;
            categoryProfile.OrderBy = updateCategoryProfile.OrderBy;
            categoryProfile.Expiry = updateCategoryProfile.Expiry;
            categoryProfile.Deadline = updateCategoryProfile.Deadline;  

            _dataContext.Profiles.Update(categoryProfile);
            await _dataContext.SaveChangesAsync();
            return _mapper.Map<CategoryProfile, CategoryProfileDTO>(categoryProfile);
        }

        public async Task<UpdateCategoryProfileDTO> GetById(long id)
        {
            var categoryProfile = await _dataContext.Profiles
                                    .Where(x => x.Id == id)
                                    .FirstOrDefaultAsync();
            if (categoryProfile == null) return null;
            return _mapper.Map<CategoryProfile, UpdateCategoryProfileDTO>(categoryProfile);
        }

        public async Task<List<CategoryProfileDTO>> GetAll()
        {
            var result = await _dataContext.Profiles
                                .Include(x => x.CreateUser)
                                .OrderBy(x => x.OrderBy)
                                .ToListAsync();
            return _mapper.Map<List<CategoryProfile>, List<CategoryProfileDTO>>(result);
        }

        public async Task<List<CategoryProfileDTO>> GetAllActive()
        {
            var result = await _dataContext.Profiles
                                .Where(x => x.Status == 1)
                                .Include(x => x.CreateUser)
                                .OrderBy(x => x.OrderBy)
                                .ToListAsync();
            return _mapper.Map<List<CategoryProfile>, List<CategoryProfileDTO>>(result);
        }

        public async Task<CustomPaging<CategoryProfileDTO>> GetCustomPaging(string filter, int page, int pageSize)
        {
            int count = await _dataContext.Profiles
                                        .Where(x => x.Title.Contains(filter))
                                        .CountAsync();
            var result = await _dataContext.Profiles
                                        .Where(x => x.Title.Contains(filter))
                                        .Include(x => x.CreateUser)
                                        .OrderBy(x => x.OrderBy)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
            int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
            var CategoryProfiles = _mapper.Map<List<CategoryProfile>, List<CategoryProfileDTO>>(result);
            CustomPaging<CategoryProfileDTO> paging = new CustomPaging<CategoryProfileDTO>
            {
                TotalPage = totalPage,
                PageSize = pageSize,
                Data = CategoryProfiles
            };
            return paging;
        }
    }
}
