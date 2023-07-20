using ProjectReview.Models.Entities;

namespace ProjectReview.DTO.Jobs
{
    public class ForwardDTO
    {
        public long Id { get; set; }
        public string Opinion { get; set; }
        public long HostId { get; set; }
        public long CreateUserId { get; set; }
        public string? Content { get; set; }
        public long InstructorId { get; set; }

        public List<long>? ListUserId { get; set; }
        public User? Host { get; set; }
        public User? CreateUser { get; set; }
    }
}
