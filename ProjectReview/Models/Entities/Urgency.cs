namespace ProjectReview.Models.Entities
{
    public class Urgency
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
    }
}
