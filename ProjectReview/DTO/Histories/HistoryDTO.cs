using ProjectReview.Models.Entities;

namespace ProjectReview.DTO.Histories
{
    public class HistoryDTO
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateUserId { get; set; }

        public User CreateUser { get; set; }
    }
}
