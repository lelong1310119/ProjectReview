namespace ProjectReview.Models.Entities
{
    public class Opinion
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public string? FileName { get; set; }
        public DateTime CreateDate { get; set; }    
        public long CreateUserId { get; set; }
        public long JobId { get; set; }
		public Boolean isDelete { get; set; }

		public virtual User CreateUser { get; set; }    
        public virtual Job Job { get; set;}
    }
}
