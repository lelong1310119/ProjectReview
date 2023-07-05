using Microsoft.EntityFrameworkCore;
using ProjectReview.Models.Entities;

namespace ProjectReview.Models
{
    public class DataContext : DbContext
    {
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<DocumentType> DocumentTypes { get; set; }
        public virtual DbSet<Handler> Handlers { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobProfile> JobProfiles { get; set; }
        public virtual DbSet<Opinion> Opinions { get; set; }
        public virtual DbSet<PermissionGroup> PermissionGroups { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<ProfileDocument> ProfileDocuments { get; set; }
        public virtual DbSet<Rank> Ranks { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(x => x.Phone).HasMaxLength(400);
                entity.Property(x => x.Address).HasMaxLength(400);
                entity.Property(x => x.CreateDate)
                    .IsRequired()
                    .HasColumnType("datetime");

                entity.HasOne(x => x.CreateUser)
                    .WithMany(y => y.Departments)
                    .HasForeignKey(x => x.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Department_User");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Documnet");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.Property(x => x.Number).IsRequired();
                entity.Property(x => x.Author)
                    .IsRequired()
                    .HasMaxLength(400);
                entity.Property(x => x.Symbol)
                    .IsRequired()
                    .HasMaxLength(400);
                entity.Property(x => x.Content).IsRequired(); 
                entity.Property(x => x.Receiver)
                    .IsRequired()
                    .HasMaxLength(400);
                entity.Property(x => x.FileName)
                    .IsRequired()
                    .HasMaxLength(400); 
                entity.Property(x => x.Density).IsRequired();
                entity.Property(x => x.Urgency).IsRequired(); 
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
                    .HasConstraintName("FK_Documnent_User");

                entity.HasOne(x => x.DocumentType)
                    .WithMany(y => y.Documents)
                    .HasForeignKey(x => x.DocumentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Documnent_DocumentType");
            });

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.ToTable("DocumentType");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedOnAdd();

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

            modelBuilder.Entity<Handler>(entity =>
            {
                entity.ToTable("Handler");
                entity.HasKey(x => new {x.UserId, x.JobId});

                entity.HasOne(x => x.User)
                    .WithMany(y => y.Handlers)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Handler_User");

                entity.HasOne(x => x.Job)
                    .WithMany(y => y.Handlers)
                    .HasForeignKey(x => x.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Handler_Job");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.Property(x => x.Request).IsRequired();
                entity.Property(x => x.Content).IsRequired();
                entity.Property(x => x.FileName)
                    .IsRequired()
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

                //entity.HasOne(x => x.Document)
                //    .WithMany(y => y.Job)
                //    .HasForeignKey(x => x.DocumnentId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_Job_Document");
            });
        }
    }
}
