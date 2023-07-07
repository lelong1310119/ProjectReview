namespace ProjectReview.Models.Entities
{
    public class Department
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        //public long CreateUserId { get; set; }
        public Boolean isDelete { get; set; }  

        //public virtual User CreateUser { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public Department()
        {

        }
    }
}
