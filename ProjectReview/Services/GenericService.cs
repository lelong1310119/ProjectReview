using ProjectReview.DTO.Jobs;
using ProjectReview.DTO.Users;
using ProjectReview.Repositories;

namespace ProjectReview.Services
{
    public interface IGenericService
    {
        Task<int> QuantityDocumentSent(DateTime date);
        Task<int> QuantityDocumentReceived(DateTime date);
        Task<int> QuantityProfile();
        Task<ListJobDTO> ListJob(DateTime date);
        Task<List<UserDTO>> GetListByBirthday(DateTime date);
    }


    public class GenericService : IGenericService
    {
        private readonly IUnitOfWork _UOW;
        public GenericService(IUnitOfWork UOW)
        {
            _UOW = UOW;
        }

        public async Task<int> QuantityDocumentSent(DateTime date)
        {
            return await _UOW.DocumentRepository.QuantityDocumentSent(date);
        }

        public async Task<int> QuantityDocumentReceived(DateTime date)
        {
            return await _UOW.DocumentRepository.QuantityDocumentReceived(date);
        }

        public async Task<int> QuantityProfile()
        {
            return await _UOW.JobProfileRepository.QuantityProfile();
        }

        public async Task<ListJobDTO> ListJob(DateTime date)
        {
            ListJobDTO listJobDTO = new ListJobDTO();
            var jobs = await _UOW.JobRepository.GetList();
            if (jobs.Count > 0)
            {
                foreach(var job in jobs)
                {
                    if (job.Status == 2)
                    {
                        listJobDTO.Processed.Add(job);
                    } else if (job.Deadline.Date < date.Date)
                    {
                        listJobDTO.OutOfDate.Add(job);
                    }
                    else if (job.Status == 0)
                    {
                         listJobDTO.Pending.Add(job);
                    } else
                    {
                        listJobDTO.Processing.Add(job);
                    }
                }
            }
            return listJobDTO;
        }

        public async Task<List<UserDTO>> GetListByBirthday(DateTime date)
        {
            return await _UOW.UserRepository.GetUserByBirthday(date);
        }
    }
}
