using ProjectReview.DTO.Documents;
using ProjectReview.DTO.Opinions;
using ProjectReview.DTO.Users;
using ProjectReview.Models.Entities;

namespace ProjectReview.DTO.Jobs
{
	public class JobDTO
	{
		public long Id { get; set; }
		public long HostId { get; set; }
		public long InstructorId { get; set; }
		public long CreateUserId { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime Deadline { get; set; }
		public string Request { get; set; }
		public int Status { get; set; }
		public string? FileName { get; set; }
		public string? FilePath { get; set; }
		public string Content { get; set; }

		public User Host { get; set; }
		public User Instructor { get; set; }
		public User CreateUser { get; set; }
		public Document Document { get; set; }
		public List<User> Users { get; set; }
		public List<Opinion> Opinions { get; set; }
	}
}
