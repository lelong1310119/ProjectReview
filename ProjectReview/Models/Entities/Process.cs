namespace ProjectReview.Models.Entities
{
	public class Process
	{
		public long Id { get; set; }
		public long JobId { get; set; }
		public long InstructorId { get; set; }
		public int Status { get; set; }
		public Boolean ProcessEnd { get; set; }

		public virtual Job Job { get; set; }
		public virtual User Instructor { get; set; }

		public virtual ICollection<History> Histories { get; set; }
		public virtual ICollection<ProcessUser> ProcessUsers { get; set; }
	}
}
