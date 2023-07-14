using ProjectReview.Models.Entities;

namespace ProjectReview.DTO.Opinions
{
	public class OpinionDTO
	{
        public long Id { get; set; }
        public string Content { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateUserId { get; set; }
        public long JobId { get; set; }

        public User CreateUser { get; set; }
        public Job Job { get; set; }
    }
}
