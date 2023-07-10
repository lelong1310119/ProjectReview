namespace ProjectReview.Models.Entities
{
    public class JobProfile
    {
        public long Id { get; set; }    
        public long ProfileId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }   
        public string? Condition { get; set; }
        public int NumberPaper { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }    
        public long CreateUserId { get; set; }

		public virtual User CreateUser { get; set; }
        public virtual CategoryProfile Profile { get; set; }
        public virtual ICollection<ProfileDocument> ProfileDocuments { get; set; }
    }
}
