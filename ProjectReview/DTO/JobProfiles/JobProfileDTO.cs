using ProjectReview.Models.Entities;

namespace ProjectReview.DTO.JobProfiles
{
	public class JobProfileDTO
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public long ProfileId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string? Condition { get; set; }
		public int NumberPaper { get; set; }
		public int Status { get; set; }
		public DateTime CreateDate { get; set; }
		public long CreateUserId { get; set; }

		public User CreateUser { get; set; }
		public CategoryProfile Profile { get; set; }
	}
}
