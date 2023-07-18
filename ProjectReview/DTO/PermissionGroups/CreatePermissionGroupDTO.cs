namespace ProjectReview.DTO.PermissionGroups
{
	public class CreatePermissionGroupDTO
	{
		public string Name { get; set; }
		public List<long>? RoleIds { get; set; }
	}
}
