using ProjectReview.DTO.Jobs;
using ProjectReview.DTO.Users;
using ProjectReview.Models.Entities;

namespace ProjectReview.DTO.Documents
{
    public class DocumentDTO
    {
        public long Id { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateUserId { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }
        public string Symbol { get; set; }
        public DateTime DateIssued { get; set; }
        public string Content { get; set; }
        public long DocumentTypeId { get; set; }
        public string? FileName { get; set; }
        public long DensityId { get; set; }
        public long UrgencyId { get; set; }
        public int NumberPaper { get; set; }
        public string Language { get; set; }
        public string Signer { get; set; }
        public string Position { get; set; }
        public string? Note { get; set; }
        public Boolean IsAssign { get; set; }

        public Urgency Urgency { get; set; }
        public Density Density { get; set; }    
        public User CreateUser { get; set; }
        public DocumentType DocumentType { get; set; }
        public JobDTO Job { get; set; }
        public List<UserDTO> Users { get; set; }
    }
}
