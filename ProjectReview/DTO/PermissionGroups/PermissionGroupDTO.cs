using ProjectReview.Models.Entities;

namespace ProjectReview.DTO.PermissionGroups
{
    public class PermissionGroupDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }

        public List<RolePermission> RolePermissions { get; set; }
    }
}
