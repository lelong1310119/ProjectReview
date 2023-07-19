namespace ProjectReview.DTO.Processes
{
    public class CreateProcessDTO
    {
        public long JobId { get; set; }
        public long InstructorId { get; set; }
        public List<long> UserId { get; set; } = new List<long>();  
    }
}
