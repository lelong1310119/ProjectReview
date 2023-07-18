namespace ProjectReview.DTO.Opinions
{
    public class CreateOpinionDTO 
    {
        public string Content { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public IFormFile? FormFile { get; set; }
        public long JobId { get; set; }
    }
}
