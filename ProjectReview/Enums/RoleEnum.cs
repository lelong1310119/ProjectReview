namespace ProjectReview.Enums
{
	public class RoleEnum
	{
		public static CustomEnum Role_1 = new CustomEnum(Id: 1, Name: "ManageDocument", Detail: "Thêm, sửa, xóa văn bản");
		public static CustomEnum Role_2 = new CustomEnum(Id: 2, Name: "AssignDocument", Detail: "Chuyển xử lý văn bản");
        public static CustomEnum Role_3 = new CustomEnum(Id: 3, Name: "ManageJob", Detail: "Quản lý công việc");
		public static CustomEnum Role_4 = new CustomEnum(Id: 4, Name: "ManageProfile", Detail: "Quản lý danh mục hồ sơ");
		public static CustomEnum Role_5 = new CustomEnum(Id: 5, Name: "ManageJobProfile", Detail: "Quản lý hồ sơ công việc");
		public static CustomEnum Role_6 = new CustomEnum(Id: 6, Name: "ManageDocumentType", Detail: "Quản lý danh mục loại văn bản");
        public static CustomEnum Role_7 = new CustomEnum(Id: 7, Name: "ManageDepartment", Detail: "Quản lý danh mục đơn vị, bộ phận");
        public static CustomEnum Role_8 = new CustomEnum(Id: 8, Name: "ManagePosition", Detail: "Quản lý danh mục chức danh, chức vụ");
        public static CustomEnum Role_9 = new CustomEnum(Id: 9, Name: "ManageRank", Detail: "Quản lý danh mục cấp bậc");
        public static CustomEnum Role_10 = new CustomEnum(Id: 10, Name: "ManageUser", Detail: "Quản lý cán bộ");
        public static CustomEnum Role_11 = new CustomEnum(Id: 11, Name: "ManagePermissionGroup", Detail: "Quản lý nhóm quyền");

        public static List<CustomEnum> RoleEnumList = new List<CustomEnum>
        {
            Role_1, Role_2, Role_3, Role_4, Role_5, Role_6, Role_7, Role_8, Role_9, Role_10, Role_11,
        };
    }
}
