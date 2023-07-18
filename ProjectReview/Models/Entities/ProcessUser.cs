namespace ProjectReview.Models.Entities
{
    public class ProcessUser
    {
        public long ProcessId { get; set; }
        public long UserId { get; set; }

        public virtual Process Process { get; set; }
        public virtual User User { get; set; }
    }
}
