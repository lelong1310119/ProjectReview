namespace ProjectReview.DTO.Documents
{
	public class AssignDocumentDTO
	{
		public long DocumentId { get; set; }
		public long HostId { get; set; }
		public long InstructorId { get; set; }
		public DateTime Deadline { get; set; }
		public string Request { get; set; }
		public string? FileName { get; set; }
		public string? FilePath { get; set; }
		public string Content { get; set; }
		public IFormFile? FormFile { get; set; }
		public List<long>? ListUserId { get; set; }
	}
}
