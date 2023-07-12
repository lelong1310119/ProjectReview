namespace ProjectReview.Enums
{
    public class DensityEnum
    {
        public static CustomEnum Density_1 = new CustomEnum(Id: 1, Name: "Density_1", Detail: "Thường");
        public static CustomEnum Density_2 = new CustomEnum(Id: 2, Name: "Density_2", Detail: "Mật");
        public static CustomEnum Density_3 = new CustomEnum(Id: 3, Name: "Density_3", Detail: "Tối mật");
        public static CustomEnum Density_4 = new CustomEnum(Id: 4, Name: "Density_4", Detail: "Tuyệt mật");

        public static List<CustomEnum> DensityEnumList = new List<CustomEnum>
        {
            Density_1, Density_2, Density_3, Density_4
        };
    }
}
