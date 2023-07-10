namespace ProjectReview.Models.Entities
{
    public class Handler
    {
        public long UserId { get; set; }
        public long JobId { get; set; }

		public virtual User User { get; set; }
        public virtual Job Job { get; set; }    
    }
}
