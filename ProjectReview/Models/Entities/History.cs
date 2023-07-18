namespace ProjectReview.Models.Entities
{
    public class History
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateUserId { get; set; }
        public long ProcessId { get; set; }

        public virtual User CreateUser { get; set; }
        public virtual Process Process { get; set; }
    }
}
