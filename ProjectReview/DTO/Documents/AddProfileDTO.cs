namespace ProjectReview.DTO.Documents
{
    public class AddProfileDTO
    {
        public long Id { get; set; }
        public int Number { get; set; }
        public string Symbol { get; set; }
        public DateTime DateIssued { get; set; }
        public string Content { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public List<long>? ProfileIds { get; set; }
    }
}
