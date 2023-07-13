using ProjectReview.Models.Entities;

namespace ProjectReview.DTO.Jobs
{
    public class CreateJobDTO
    {
        public long HostId { get; set; }
        public long InstructorId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Request { get; set; }
        public int Status { get; set; }
        public string? FileName { get; set; }
		public string? FilePath { get; set; }
		public string Content { get; set; }

        public virtual User Host { get; set; }
        public virtual User Instructor { get; set; }
    }
}
