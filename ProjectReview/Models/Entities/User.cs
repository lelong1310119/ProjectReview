
namespace ProjectReview.Models.Entities
{
    public class User
    {
        public long Id { get; set; }    
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public long PositionId { get; set; }
        public long DepartmentId { get; set; }
        public long PermissionGroupId { get; set; }
        public long RankId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public DateTime CreateDate { get; set; }
        public long CreateUserId { get; set; }

        public virtual User CreateUser { get; set; }
        public virtual Position Position { get; set; }
        public virtual Department Department { get; set; }
        public virtual PermissionGroup PermissionGroup { get; set; }
        public virtual Rank Rank { get; set; }

        public virtual ICollection<User> Users { get; set; }    
        public virtual ICollection<Job> CreateJobs { get; set; }
        public virtual ICollection<Job> HostJobs { get; set; }
        public virtual ICollection<Job> InstructorJobs { get; set; }
        public virtual ICollection<Profile> Profiles { get; set; }
        public virtual ICollection<JobProfile> JobProfiles { get; set; }
        public virtual ICollection<Opinion> Opinions { get; set; }
        public virtual ICollection<DocumentType> DocumentTypes { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<Position> Positions { get; set; }
        public virtual ICollection<Rank> Ranks { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<PermissionGroup> PermissionGroups { get; set; }
        public virtual ICollection<Handler> Handlers { get; set; }
    }
}
