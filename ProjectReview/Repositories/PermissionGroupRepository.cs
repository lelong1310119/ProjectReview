using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.DTO.PermissionGroups;
using ProjectReview.Models;
using ProjectReview.Models.Entities;

namespace ProjectReview.Repositories
{
    public interface IPermissionGroupRepository
    {
        Task<List<PermissionGroupDTO>> GetAllActive();
    }

    public class PermissionGroupRepository : IPermissionGroupRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public PermissionGroupRepository(DataContext dataContext, IMapper mapper) { 
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<List<PermissionGroupDTO>> GetAllActive()
        {
            var result = await _dataContext.PermissionGroups
                                    .Where(x => x.Status == 1)
                                    .ToListAsync();
            return _mapper.Map<List<PermissionGroup>, List<PermissionGroupDTO>>(result);
        }
    }
}
