using AutoMapper;
using ProjectReview.Common;
using ProjectReview.Models;

namespace ProjectReview.Repositories
{
    public interface IJobRepository
    {

    }

    public class JobRepository : IJobRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public JobRepository(DataContext dataContext, IMapper mapper, ICurrentUser currentUser)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _currentUser = currentUser;
        }
    }
}
