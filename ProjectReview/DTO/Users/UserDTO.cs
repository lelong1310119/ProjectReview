using ProjectReview.Models.Entities;

namespace ProjectReview.DTO.Users
{
	public class UserDTO
	{
		public long Id { get; set; }	
		public string UserName { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public long PositionId { get; set; }
		public long DepartmentId { get; set; }
		public long PermissionGroupId { get; set; }
		public long RankId { get; set; }
		public Position Position { get; set; }
		public Department Department { get; set; }
		public PermissionGroup PermissionGroup { get; set; }
		public Rank Rank { get; set; }
		public string Note { get; set; }
		public DateTime Birthday { get; set; }
		public string Gender { get; set; }
		public DateTime CreateDate { get; set; }
	}
}
