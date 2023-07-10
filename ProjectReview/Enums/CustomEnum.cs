namespace ProjectReview.Enums
{
    public class CustomEnum : IEquatable<CustomEnum>
    {
        public long Id { get; }
        public string Name { get; }
        public CustomEnum(long Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }
        public bool Equals(CustomEnum other)
        {
            if (this.Id != other.Id) return false;
            if (this.Name != other.Name) return false;
            return true;
        }
    }
}
