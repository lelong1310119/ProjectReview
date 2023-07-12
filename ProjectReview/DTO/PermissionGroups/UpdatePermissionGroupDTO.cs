namespace ProjectReview.DTO.PermissionGroups
{
	public class UpdatePermissionGroupDTO
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public List<long> ListRole { get; set; }
	}
}
