namespace ProjectReview.Enums
{
    public class CustomEnum : IEquatable<CustomEnum>
    {
        public long Id { get; }
        public string Name { get; }
        public string Detail { get; }
        public CustomEnum(long Id, string Name, string Detail)
        {
            this.Id = Id;
            this.Name = Name;
            this.Detail = Detail;
        }
        public bool Equals(CustomEnum other)
        {
            if (this.Id != other.Id) return false;
            if (this.Name != other.Name) return false;
            if (this.Detail != other.Detail) return false;
            return true;
        }
    }
}
