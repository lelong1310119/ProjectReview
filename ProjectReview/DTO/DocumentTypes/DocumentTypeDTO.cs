using ProjectReview.DTO.Users;
using ProjectReview.Models.Entities;

namespace ProjectReview.DTO.DocumentTypes
{
    public class DocumentTypeDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }

        public long CreateUserId { get; set; }
        public User CreateUser { get; set; }
    }
}
