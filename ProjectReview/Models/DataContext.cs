using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectReview.Models.Entities;
namespace ProjectReview.Models
{
    public partial class DataContext : IdentityDbContext<User, Role, long, IdentityUserClaim<long>, UserRole, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<DocumentType> DocumentTypes { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobProfile> JobProfiles { get; set; }
        public virtual DbSet<PermissionGroup> PermissionGroups { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<CategoryProfile> Profiles { get; set; }
        public virtual DbSet<ProfileDocument> ProfileDocuments { get; set; }
        public virtual DbSet<Rank> Ranks { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<JobDocument> JobDocuments { get; set; }
        public virtual DbSet<Density> Densities { get; set; }
        public virtual DbSet<Urgency> Urgencies { get; set; }
        public virtual DbSet<Process> Processes { get; set; }
        public virtual DbSet<ProcessUser> ProcessUsers { get; set; }
        public virtual DbSet<History> History { get; set; }

        public DataContext()
        {

        }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Data Source=DESKTOP-PL7Q9Q6; Initial Catalog=Review;Integrated Security=True;TrustServerCertificate=True;");
//            }
//        }
//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Data Source=LAPTOP-TC1PJ34D\\LONG;Initial Catalog=Review;Integrated Security=True;TrustServerCertificate=True;");
//            }
//        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(x => x.Phone).HasMaxLength(400);
                entity.Property(x => x.Address).HasMaxLength(400);
                entity.Property(x => x.CreateDate)
                    .IsRequired()
                    .HasColumnType("datetime");

                //entity.HasOne(x => x.CreateUser)
                //    .WithMany(y => y.Departments)
                //    .HasForeignKey(x => x.CreateUserId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_Department_User");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Document");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();

                entity.Property(x => x.Number).IsRequired();
                entity.Property(x => x.Author)
                    .IsRequired()
                    .HasMaxLength(400);
                entity.Property(x => x.Symbol)
                    .IsRequired()
                    .HasMaxLength(400);
                entity.Property(x => x.Content).IsRequired(); 
                entity.Property(x => x.FileName)
                    .HasMaxLength(400); 
                entity.Property(x => x.NumberPaper).IsRequired();
                entity.Property(x => x.Language)
                    .IsRequired()
                    .HasMaxLength(400); 
                entity.Property(x => x.Signer)
                    .IsRequired()
                    .HasMaxLength(400);
                entity.Property(x => x.Position)
                    .IsRequired()
                    .HasMaxLength(400); 
                entity.Property(x => x.IsAssign).IsRequired();
                entity.Property(x => x.Type).IsRequired(); 

                entity.Property(x => x.CreateDate)
                    .IsRequired()
                    .HasColumnType("datetime");
                entity.Property(x => x.DateIssued)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.HasOne(x => x.CreateUser)
                    .WithMany(y => y.Documents)
                    .HasForeignKey(x => x.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Document_User");

                entity.HasOne(x => x.Density)
                    .WithMany(y => y.Documents)
                    .HasForeignKey(x => x.DensityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Document_Density");

                entity.HasOne(x => x.Urgency)
                    .WithMany(y => y.Documents)
                    .HasForeignKey(x => x.UrgencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Document_Urgency");

                entity.HasOne(x => x.DocumentType)
                    .WithMany(y => y.Documents)
                    .HasForeignKey(x => x.DocumentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Document_DocumentType");
            });

            modelBuilder.Entity<Process>(entity =>
            {
                entity.ToTable("Process");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.HasOne(x => x.Instructor)
                    .WithMany(y => y.Processes)
                    .HasForeignKey(x => x.InstructorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Process_User");
                entity.HasOne(x => x.Job)
                    .WithMany(y => y.Processes)
                    .HasForeignKey(x => x.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Process_Job");
            });

            modelBuilder.Entity<History>(entity =>
            {
                entity.ToTable("History");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.HasOne(x => x.Process)
                    .WithMany(y => y.Histories)
                    .HasForeignKey(x => x.ProcessId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_History_Process");
                entity.HasOne(x => x.CreateUser)
                    .WithMany(y => y.Histories)
                    .HasForeignKey(x => x.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_History_User");
            });

            modelBuilder.Entity<ProcessUser>(entity =>
            {
                entity.ToTable("ProcessUser");
                entity.HasKey(x => new { x.ProcessId, x.UserId });

                entity.HasOne(x => x.User)
                    .WithMany(y => y.ProcessUsers)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProcessUser_User");

                entity.HasOne(x => x.Process)
                    .WithMany(y => y.ProcessUsers)
                    .HasForeignKey(x => x.ProcessId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProcessUser_Process");
            });

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.ToTable("DocumentType");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(x => x.Status).IsRequired();
                entity.Property(x => x.CreateDate)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.HasOne(x => x.CreateUser)
                    .WithMany(y => y.DocumentTypes)
                    .HasForeignKey(x => x.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DocumentType_User");
            });


            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();

                entity.Property(x => x.Request).IsRequired();
                entity.Property(x => x.Content).IsRequired();
                entity.Property(x => x.FileName)
                    .HasMaxLength(400);
                entity.Property(x => x.Status).IsRequired();

                entity.Property(x => x.CreateDate)
                    .IsRequired()
                    .HasColumnType("datetime");
                entity.Property(x => x.Deadline)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.HasOne(x => x.CreateUser)
                    .WithMany(y => y.CreateJobs)
                    .HasForeignKey(x => x.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Job_CreateUser");

                entity.HasOne(x => x.Host)
                    .WithMany(y => y.HostJobs)
                    .HasForeignKey(x => x.HostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Job_HostUser");

                entity.HasOne(x => x.Instructor)
                    .WithMany(y => y.InstructorJobs)
                    .HasForeignKey(x => x.InstructorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Job_InstructorUser");
            });

            modelBuilder.Entity<JobProfile>(entity =>
            {
                entity.ToTable("JobProfile");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();

                entity.Property(x => x.Name).IsRequired().HasMaxLength(400);
                entity.Property(x => x.NumberPaper).IsRequired();
                entity.Property(x => x.Status).IsRequired();
                entity.Property(x => x.Condition)
                    .HasMaxLength(4000);
                
                entity.Property(x => x.CreateDate)
                    .IsRequired()
                    .HasColumnType("datetime");
                entity.Property(x => x.StartDate)
                    .IsRequired()
                    .HasColumnType("datetime");
                entity.Property(x => x.EndDate)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.HasOne(x => x.CreateUser)
                    .WithMany(y => y.JobProfiles)
                    .HasForeignKey(x => x.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobProfile_User");

                entity.HasOne(x => x.Profile)
                    .WithMany(y => y.JobProfiles)
                    .HasForeignKey(x => x.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobProfile_Profile");
            });

            modelBuilder.Entity<PermissionGroup>(entity =>
            {
                entity.ToTable("PermissionGroup");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();

                entity.Property(x => x.Status).IsRequired();
                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(x => x.CreateDate)
                    .IsRequired()
                    .HasColumnType("datetime");

                //entity.HasOne(x => x.CreateUser)
                //    .WithMany(y => y.PermissionGroups)
                //    .HasForeignKey(x => x.CreateUserId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_PermissionGroup_User");
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.ToTable("Position");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();

                entity.Property(x => x.Status).IsRequired();
                entity.Property(x => x.Note).HasMaxLength(4000);
                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(x => x.CreateDate)
                    .IsRequired()
                    .HasColumnType("datetime");

                //entity.HasOne(x => x.CreateUser)
                //    .WithMany(y => y.Positions)
                //    .HasForeignKey(x => x.CreateUserId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_Position_User");
            });

            modelBuilder.Entity<CategoryProfile>(entity =>
            {
                entity.ToTable("Profile");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();

                entity.Property(x => x.Symbol)
                    .IsRequired()
                    .HasMaxLength(400);
                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(400);
                entity.Property(x => x.Expiry)
                    .IsRequired()
                    .HasMaxLength(125);
                entity.Property(x => x.OrderBy).IsRequired();
                entity.Property(x => x.Status).IsRequired();
                
                entity.Property(x => x.CreateDate)
                    .IsRequired()
                    .HasColumnType("datetime");
                entity.Property(x => x.Deadline)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.HasOne(x => x.CreateUser)
                    .WithMany(y => y.Profiles)
                    .HasForeignKey(x => x.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Profile_User");
            });

            modelBuilder.Entity<ProfileDocument>(entity =>
            {
                entity.ToTable("ProfileDocument");
                entity.HasKey(x => new { x.JobProfileId, x.DocumentId });

                entity.HasOne(x => x.JobProfile)
                    .WithMany(y => y.ProfileDocuments)
                    .HasForeignKey(x => x.JobProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfileDocument_JobProfile");

                entity.HasOne(x => x.Document)
                    .WithMany(y => y.ProfileDocuments)
                    .HasForeignKey(x => x.DocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProfileDocument_Document");
            });

            modelBuilder.Entity<Rank>(entity =>
            {
                entity.ToTable("Rank");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();

                entity.Property(x => x.Status).IsRequired();
                entity.Property(x => x.Note).HasMaxLength(4000);
                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(x => x.CreateDate)
                    .IsRequired()
                    .HasColumnType("datetime");

                //entity.HasOne(x => x.CreateUser)
                //    .WithMany(y => y.Ranks)
                //    .HasForeignKey(x => x.CreateUserId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_Rank_User");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();

                entity.Property(x => x.FullName)
                    .IsRequired()
                    .HasMaxLength(400);
                entity.Property(x => x.Gender).IsRequired();
                entity.Property(x => x.UserName)
                    .IsRequired()
                    .HasMaxLength(400);
                entity.Property(x => x.PasswordHash)
                    .IsRequired();
                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(400);
                entity.Property(x => x.Note).HasMaxLength(4000);

                entity.Property(x => x.CreateDate)
                    .IsRequired()
                    .HasColumnType("datetime");
                entity.Property(x => x.Birthday)
                    .IsRequired()
                    .HasColumnType("datetime");

                //entity.HasOne(x => x.CreateUser)
                //    .WithMany(y => y.Users)
                //    .HasForeignKey(x => x.CreateUserId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_User_CreateUser");
                entity.HasOne(x => x.Position)
                    .WithMany(y => y.Users)
                    .HasForeignKey(x => x.PositionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Position");
                entity.HasOne(x => x.Department)
                    .WithMany(y => y.Users)
                    .HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Department");
                entity.HasOne(x => x.PermissionGroup)
                    .WithMany(y => y.Users)
                    .HasForeignKey(x => x.PermissionGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_PermissionGroup");
                entity.HasOne(x => x.Rank)
                    .WithMany(y => y.Users)
                    .HasForeignKey(x => x.RankId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Rank");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();

                entity.Property(x => x.Name).IsRequired();
            });

			modelBuilder.Entity<RolePermission>(entity =>
			{
				entity.ToTable("RolePermission");
				entity.HasKey(x => new { x.PermissionGroupId, x.RoleId });

				entity.HasOne(x => x.PermissionGroup)
					.WithMany(y => y.RolePermissions)
					.HasForeignKey(x => x.PermissionGroupId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_RolePermission_Permission");

				entity.HasOne(x => x.Role)
					.WithMany(y => y.RolePermissions)
					.HasForeignKey(x => x.RoleId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_RolePermission_Role");
			});

			modelBuilder.Entity<JobDocument>(entity =>
			{
				entity.ToTable("JobDocument");
				entity.HasKey(x => new { x.JobId, x.DocumentId });

				entity.HasOne(x => x.Job)
					.WithOne(y => y.JobDocument)
					.HasForeignKey<JobDocument>(x => x.JobId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_JobDocument_Job");

				entity.HasOne(x => x.Document)
					.WithOne(y => y.JobDocument)
					.HasForeignKey<JobDocument>(x => x.DocumentId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_JobDocument_Document");

			});

			modelBuilder.Entity<UserRole>(entity =>
            {
				entity.ToTable("UserRole");
				entity.HasKey(x => new { x.UserId, x.RoleId });

				entity.HasOne(x => x.User)
					.WithMany(y => y.UserRoles)
					.HasForeignKey(x => x.UserId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_UserRole_User");

				entity.HasOne(x => x.Role)
					.WithMany(y => y.UserRoles)
					.HasForeignKey(x => x.RoleId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_UserRole_Role");
			});

            modelBuilder.Entity<Density>(entity =>
            {
                entity.ToTable("Density");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Urgency>(entity =>
            {
                entity.ToTable("Urgency");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();
            });
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
