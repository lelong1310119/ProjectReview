namespace ProjectReview.DTO.JobProfiles
{
	public class UpdateJobProfileDTO
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public long ProfileId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string? Condition { get; set; }
		public int NumberPaper { get; set; }
	}
}
