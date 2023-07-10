namespace ProjectReview.Models.Entities
{
	public class JobDocument
	{
		public long JobId { get; set; }
		public long DocumentId { get; set; }

		public virtual Job Job { get; set; }	
		public virtual Document Document { get; set; }
	}
}
