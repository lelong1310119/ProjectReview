
using Microsoft.AspNetCore.Identity;

namespace ProjectReview.Models.Entities
{
    public class User : IdentityUser<long>
    {
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public long PositionId { get; set; }
        public long DepartmentId { get; set; }
        public long PermissionGroupId { get; set; }
        public long RankId { get; set; }
        public string? Note { get; set; }
        public long Status { get; set; } = 0;
        public DateTime CreateDate { get; set; }

        public virtual Position Position { get; set; }
        public virtual Department Department { get; set; }
        public virtual PermissionGroup PermissionGroup { get; set; }
        public virtual Rank Rank { get; set; }
        public virtual ICollection<Job> CreateJobs { get; set; }
        public virtual ICollection<Job> HostJobs { get; set; }
        public virtual ICollection<Job> InstructorJobs { get; set; }
        public virtual ICollection<CategoryProfile> Profiles { get; set; }
        public virtual ICollection<JobProfile> JobProfiles { get; set; }
        public virtual ICollection<Opinion> Opinions { get; set; }
        public virtual ICollection<DocumentType> DocumentTypes { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<Handler> Handlers { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<ProcessUser> ProcessUsers { get; set; }
        public virtual ICollection<History> Histories { get; set; }
        public virtual ICollection<Process> Processes { get; set; }
    }
}
