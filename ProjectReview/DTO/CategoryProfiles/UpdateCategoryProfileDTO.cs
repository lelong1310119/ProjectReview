namespace ProjectReview.DTO.CategoryProfiles
{
    public class UpdateCategoryProfileDTO
    {
        public long Id { get; set; }
        public string Symbol { get; set; }
        public string Title { get; set; }
        public string Expiry { get; set; }
        public DateTime Deadline { get; set; }
        public int OrderBy { get; set; }
    }
}
