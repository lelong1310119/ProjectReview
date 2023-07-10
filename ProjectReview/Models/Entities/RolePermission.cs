namespace ProjectReview.Models.Entities
{
	public class RolePermission
	{
		public long PermissionGroupId { get; set; }
		public long RoleId { get; set; }

		public virtual Role Role { get; set; }	
		public virtual PermissionGroup PermissionGroup { get; set; }
	}
}
