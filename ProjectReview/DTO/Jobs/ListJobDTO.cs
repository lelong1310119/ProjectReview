namespace ProjectReview.DTO.Jobs
{
    public class ListJobDTO
    {
        public List<JobDTO> Processed { get; set; }
        public List<JobDTO> Processing { get; set; }
        public List<JobDTO> Pending { get; set; }
        public List<JobDTO> OutOfDate { get; set; }

        public ListJobDTO()
        {
            Processed = new List<JobDTO>();
            Processing = new List<JobDTO>();
            Pending = new List<JobDTO>();
            OutOfDate = new List<JobDTO>();
        }
    }
}
