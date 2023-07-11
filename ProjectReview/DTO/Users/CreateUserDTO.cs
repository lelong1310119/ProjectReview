using NuGet.Protocol.Plugins;

namespace ProjectReview.DTO.Users
{
	public class CreateUserDTO
	{
		public string UserName { get; set; }
		public string? FullName { get; set; }
		public string? Email { get; set; }
		public long PositionId { get; set; }
		public long DepartmentId { get; set; }
		public long PermissionGroupId { get; set; }
		public long RankId { get; set; }
		public string? Note { get; set; }
		public DateTime Birthday { get; set; }
		public string? Gender { get; set; }
		public string? PasswordHash { get; set; }
	}
}
