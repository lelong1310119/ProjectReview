namespace ProjectReview.Enums
{
    public class UrgencyEnum
    {
        public static CustomEnum Urgency_1 = new CustomEnum(Id: 1, Name: "Urgency_1", Detail: "Thường");
        public static CustomEnum Urgency_2 = new CustomEnum(Id: 2, Name: "Urgency_2", Detail: "Khẩn");
        public static CustomEnum Urgency_3 = new CustomEnum(Id: 3, Name: "Urgency_3", Detail: "Thượng khẩn");
        public static CustomEnum Urgency_4 = new CustomEnum(Id: 4, Name: "Urgency_4", Detail: "Hỏa tốc");

        public static List<CustomEnum> UrgencyEnumList = new List<CustomEnum>
        {
            Urgency_1, Urgency_2, Urgency_3, Urgency_4
        };
    }
}
