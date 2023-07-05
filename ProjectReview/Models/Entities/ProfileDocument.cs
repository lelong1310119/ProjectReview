namespace ProjectReview.Models.Entities
{
    public class ProfileDocument
    {
        public long JobProfileId { get; set; }
        public long DocumentId { get; set; }

        public virtual JobProfile JobProfile { get; set; }  
        public virtual Document Document { get; set; }  
    }
}
