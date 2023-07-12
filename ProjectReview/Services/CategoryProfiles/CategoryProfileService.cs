using ProjectReview.DTO.CategoryProfiles;
using ProjectReview.DTO.JobProfiles;
using ProjectReview.Paging;
using ProjectReview.Repositories;

namespace ProjectReview.Services.CategoryProfiles
{
    public interface ICategoryProfileService
    {
        Task Delete(long id);
        Task Active(long id);
        Task Close(long id);
        Task<CategoryProfileDTO> Create(CreateCategoryProfileDTO createCategoryProfile);
        Task<CategoryProfileDTO> Update(UpdateCategoryProfileDTO updateCategoryProfile);
        Task<UpdateCategoryProfileDTO> GetById(long id);
        Task<List<CategoryProfileDTO>> GetAll();
        Task<CustomPaging<CategoryProfileDTO>> GetCustomPaging(string? filter, int page, int pageSize);
    }

    public class CategoryProfileService : ICategoryProfileService
    {
        private readonly IUnitOfWork _UOW;
        public CategoryProfileService(IUnitOfWork unitOfWork)
        {
            _UOW = unitOfWork;
        }

        public async Task Delete(long id)
        {
            var result = await _UOW.JobProfileRepository.GetListByProfile(id);
            if (result.Count > 0)
            {
                foreach(var profile in result) {
                    await _UOW.ProfileDocumentRepository.DeleteByProfile(profile.Id);
                    await _UOW.JobProfileRepository.Delete(id);
                }
            }
            await _UOW.CategoryProfileRepository.Delete(id);
        }

        public async Task Active(long id)
        {
            await _UOW.CategoryProfileRepository.Active(id);
            await _UOW.JobProfileRepository.OpenList(id);
        }

        public async Task Close(long id)
        {
            await _UOW.CategoryProfileRepository.Close(id);
            await _UOW.JobProfileRepository.CloseList(id);
        }

        public async Task<CategoryProfileDTO> Create(CreateCategoryProfileDTO createCategoryProfile)
        {
            createCategoryProfile.Title= createCategoryProfile.Title.Trim();
            var check = await _UOW.CategoryProfileRepository.GetByName(createCategoryProfile.Title);
            if (check) throw new Exception("Tên hồ sơ đã tồn tại. Vui lòng nhập tên khác");
            return await _UOW.CategoryProfileRepository.Create(createCategoryProfile);
        }

        public async Task<CategoryProfileDTO> Update(UpdateCategoryProfileDTO updateCategoryProfile)
        {
            var CategoryProfile = await _UOW.CategoryProfileRepository.GetById(updateCategoryProfile.Id);
            updateCategoryProfile.Title = updateCategoryProfile.Title.Trim();
            if (updateCategoryProfile.Title != CategoryProfile.Title)
            {
                var check = await _UOW.CategoryProfileRepository.GetByName(updateCategoryProfile.Title);
                if (check) throw new Exception("Tên hố sơ đã tồn tại. Vui lòng nhập tên khác");
            }
            return await _UOW.CategoryProfileRepository.Update(updateCategoryProfile);
        }

        public async Task<UpdateCategoryProfileDTO> GetById(long id)
        {
            return await _UOW.CategoryProfileRepository.GetById(id);
        }

        public async Task<List<CategoryProfileDTO>> GetCategoryProfile()
        {
            return await _UOW.CategoryProfileRepository.GetAllActive();
        }

        public async Task<List<CategoryProfileDTO>> GetAll()
        {
            return await _UOW.CategoryProfileRepository.GetAll();
        }

        public async Task<CustomPaging<CategoryProfileDTO>> GetCustomPaging(string? filter, int page, int pageSize)
        {
            filter = (filter ?? "");
            return await _UOW.CategoryProfileRepository.GetCustomPaging(filter, page, pageSize);
        }
    }
}
