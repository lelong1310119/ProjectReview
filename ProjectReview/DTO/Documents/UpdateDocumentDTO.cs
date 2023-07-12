namespace ProjectReview.DTO.Documents
{
    public class UpdateDocumentDTO
    {
        public long Id { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }
        public string Symbol { get; set; }
        public DateTime DateIssued { get; set; }
        public string Content { get; set; }
        public long DocumentTypeId { get; set; }
        public string Receiver { get; set; }
        public string FileName { get; set; }
        public long DensityId { get; set; }
        public long UrgencyId { get; set; }
        public int NumberPaper { get; set; }
        public string Language { get; set; }
        public string Signer { get; set; }
        public string Position { get; set; }
        public string? Note { get; set; }
    }
}
