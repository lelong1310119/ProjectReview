using Microsoft.EntityFrameworkCore;
using ProjectReview.Enums;
using ProjectReview.Models.Entities;
using System.Security.Cryptography;

namespace ProjectReview.Models
{
    public class DbSeeder
    {
        public static async Task Migrate(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<DataContext>();
            await context.Database.MigrateAsync();
            await DepartmentInitialize(serviceProvider);
            await PositionInitialize(serviceProvider);
            await RankInitialize(serviceProvider);
            await RoleInitialize(serviceProvider);
            await PermissionGroupInitialize(serviceProvider);
            await RolePermissionInitialize(serviceProvider);
            await UserInitialize(serviceProvider);
            await DocumentTypeInitialize(serviceProvider);
            await CategoryProfileInitialize(serviceProvider);
            await JobProfileInitialize(serviceProvider);
            await UrgencyInitialize(serviceProvider);
            await DensityInitialize(serviceProvider);
        }

        public static async Task DepartmentInitialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<DataContext>();
            if (context != null)
            {
                var departmentSet = context.Departments;
                if (await departmentSet.AnyAsync()) return;

                List<Department> departments = new List<Department>();
                departments.Add(new Department { Id = 1, Name = "Ban lãnh đạo", Address = "144 Xuân Thủy, Cầu Giấy, Hà Nội", Phone = "", Status = 1, CreateDate = DateTime.Now});
                departments.Add(new Department { Id = 2, Name = "Hành chính - Tổ chức", Address = "144 Xuân Thủy, Cầu Giấy, Hà Nội", Phone = "", Status = 0, CreateDate = DateTime.Now });
                departments.Add(new Department { Id = 3, Name = "Kế toán - Tài vụ", Address = "144 Xuân Thủy, Cầu Giấy, Hà Nội", Phone = "", Status = 0, CreateDate = DateTime.Now });
                departments.Add(new Department { Id = 4, Name = "Tổ chuyên môn", Address = "144 Xuân Thủy, Cầu Giấy, Hà Nội", Phone = "", Status = 0, CreateDate = DateTime.Now });
                departments.Add(new Department { Id = 5, Name = "Tổ kỹ thuật", Address = "144 Xuân Thủy, Cầu Giấy, Hà Nội", Phone = "", Status = 0, CreateDate = DateTime.Now });
                departments.Add(new Department { Id = 6, Name = "Quản trị hệ thống", Address = "144 Xuân Thủy, Cầu Giấy, Hà Nội", Phone = "", Status = 0, CreateDate = DateTime.Now });


                foreach (var item in departments)
                {
                    await departmentSet.AddAsync(item);
                }

                await context.SaveChangesAsync();
            }
        }

        public static async Task PositionInitialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<DataContext>();
            if (context != null)
            {
                var positionSet = context.Positions;
                if (await positionSet.AnyAsync()) return;

                List<Position> positions = new List<Position>();
                positions.Add(new Position { Id = 1, Name = "Quản trị hệ thống", Note = "", Status = 1, CreateDate = DateTime.Now });
                positions.Add(new Position { Id = 2, Name = "Trưởng phòng", Note = "", Status = 0, CreateDate = DateTime.Now });
                positions.Add(new Position { Id = 3, Name = "Phó trưởng phòng", Note = "", Status = 0, CreateDate = DateTime.Now });
                positions.Add(new Position { Id = 4, Name = "Cán bộ", Note = "", Status = 0, CreateDate = DateTime.Now });
                positions.Add(new Position { Id = 5, Name = "Nhân viên văn thư", Note = "", Status = 0, CreateDate = DateTime.Now });
                positions.Add(new Position { Id = 6, Name = "Kế toán", Note = "", Status = 0, CreateDate = DateTime.Now });


                foreach (var item in positions)
                {
                    await positionSet.AddAsync(item);
                }

                await context.SaveChangesAsync();
            }
        }

        public static async Task RankInitialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<DataContext>();
            if (context != null)
            {
                var rankSet = context.Ranks;
                if (await rankSet.AnyAsync()) return;

                List<Rank> ranks = new List<Rank>();
                ranks.Add(new Rank { Id = 1, Name = "Thiếu úy", Note = "", Status = 0, CreateDate = DateTime.Now });
                ranks.Add(new Rank { Id = 2, Name = "Trung úy", Note = "", Status = 0, CreateDate = DateTime.Now });
                ranks.Add(new Rank { Id = 3, Name = "Thượng úy", Note = "", Status = 0, CreateDate = DateTime.Now });
                ranks.Add(new Rank { Id = 4, Name = "Đại úy", Note = "", Status = 0, CreateDate = DateTime.Now });
                ranks.Add(new Rank { Id = 5, Name = "Thiếu tá", Note = "", Status = 0, CreateDate = DateTime.Now });
                ranks.Add(new Rank { Id = 6, Name = "Trung tá", Note = "", Status = 0, CreateDate = DateTime.Now });
                ranks.Add(new Rank { Id = 7, Name = "Thượng tá", Note = "", Status = 0, CreateDate = DateTime.Now });
                ranks.Add(new Rank { Id = 8, Name = "Đại tá", Note = "", Status = 1, CreateDate = DateTime.Now });
                ranks.Add(new Rank { Id = 9, Name = "Thiếu tướng", Note = "", Status = 0, CreateDate = DateTime.Now });
                ranks.Add(new Rank { Id = 10, Name = "Trung tướng", Note = "", Status = 0, CreateDate = DateTime.Now });

                foreach (var item in ranks)
                {
                    await rankSet.AddAsync(item);
                }

                await context.SaveChangesAsync();
            }
        }

        public static async Task RoleInitialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<DataContext>();
            if (context != null)
            {
                var roleSet = context.Roles;
                if (await roleSet.AnyAsync()) return;

                var seed = RoleEnum.RoleEnumList;
                foreach (var item in seed)
                {
                    await roleSet.AddAsync(new Role
                    {
                        Id = item.Id,
                        Name = item.Name
                    });
                }
                await context.SaveChangesAsync();
            }
        }

        public static async Task DensityInitialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<DataContext>();
            if (context != null)
            {
                var densitySet = context.Densities;
                if (await densitySet.AnyAsync()) return;

                var seed = DensityEnum.DensityEnumList;
                foreach (var item in seed)
                {
                    await densitySet.AddAsync(new Density
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Detail = item.Detail
                    });
                }
                await context.SaveChangesAsync();
            }
        }

        public static async Task UrgencyInitialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<DataContext>();
            if (context != null)
            {
                var urgencySet = context.Urgencies;
                if (await urgencySet.AnyAsync()) return;

                var seed = UrgencyEnum.UrgencyEnumList;
                foreach (var item in seed)
                {
                    await urgencySet.AddAsync(new Urgency
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Detail = item.Detail
                    });
                }
                await context.SaveChangesAsync();
            }
        }

        public static async Task PermissionGroupInitialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<DataContext>();
            if (context != null)
            {
                var permissionSet = context.PermissionGroups;
                if (await permissionSet.AnyAsync()) return;

                List<PermissionGroup> permissions = new List<PermissionGroup>();
                permissions.Add(new PermissionGroup { Id = 1, Name = "Quản trị hệ thống", Status = 1, CreateDate = DateTime.Now });
                permissions.Add(new PermissionGroup { Id = 2, Name = "Trưởng phòng", Status = 0, CreateDate = DateTime.Now });
                permissions.Add(new PermissionGroup { Id = 3, Name = "Phó trưởng phòng", Status = 0, CreateDate = DateTime.Now });
                permissions.Add(new PermissionGroup { Id = 4, Name = "Cán bộ", Status = 0, CreateDate = DateTime.Now });
                permissions.Add(new PermissionGroup { Id = 5, Name = "Văn thư", Status = 0, CreateDate = DateTime.Now });

                foreach (var item in permissions)
                {
                    await permissionSet.AddAsync(item);
                }

                await context.SaveChangesAsync();
            }
        }

        public static async Task RolePermissionInitialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<DataContext>();
            if (context != null)
            {
                var rolePermissionSet = context.RolePermissions;
                if (await rolePermissionSet.AnyAsync()) return;

                List<RolePermission> rolePermissions = new List<RolePermission>();
                rolePermissions.Add(new RolePermission { PermissionGroupId = 1, RoleId = 1 });
                rolePermissions.Add(new RolePermission { PermissionGroupId = 1, RoleId = 2 });
                rolePermissions.Add(new RolePermission { PermissionGroupId = 1, RoleId = 3 });
                rolePermissions.Add(new RolePermission { PermissionGroupId = 1, RoleId = 4 });
                rolePermissions.Add(new RolePermission { PermissionGroupId = 1, RoleId = 5 });
                rolePermissions.Add(new RolePermission { PermissionGroupId = 1, RoleId = 6 });
                rolePermissions.Add(new RolePermission { PermissionGroupId = 1, RoleId = 7 });
                rolePermissions.Add(new RolePermission { PermissionGroupId = 1, RoleId = 8 });
                rolePermissions.Add(new RolePermission { PermissionGroupId = 1, RoleId = 9 });

                foreach (var item in rolePermissions)
                {
                    await rolePermissionSet.AddAsync(item);
                }

                await context.SaveChangesAsync();
            }
        }

        public static async Task UserInitialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<DataContext>();
            if (context != null)
            {
                var userSet = context.Users;
                if (await userSet.AnyAsync()) return;

                User user = new User
                {
                    Id = 1,
                    FullName = "SuperAdmin",
                    Birthday = DateTime.Now,
                    CreateDate = DateTime.Now,
                    Gender = "Nam",
                    PermissionGroupId = 1,
                    RankId = 8,
                    PositionId = 1,
                    DepartmentId = 1,
                    UserName = "superadmin",
                    Note = "",
                    Status = 0,
                    PasswordHash = HashPassword("123456"),
                    Email = "superadmin@gmail.com"
                };
                await userSet.AddAsync(user);
                await context.SaveChangesAsync();

                var userRoleSet = context.UserRoles;
                if (await userRoleSet.AnyAsync()) return;
                List<UserRole> userRoles = new List<UserRole>();
                userRoles.Add(new UserRole { UserId = 1, RoleId = 1 });
                userRoles.Add(new UserRole { UserId = 1, RoleId = 2 });
                userRoles.Add(new UserRole { UserId = 1, RoleId = 3 });
                userRoles.Add(new UserRole { UserId = 1, RoleId = 4 });
                userRoles.Add(new UserRole { UserId = 1, RoleId = 5 });
                userRoles.Add(new UserRole { UserId = 1, RoleId = 6 });
                userRoles.Add(new UserRole { UserId = 1, RoleId = 7 });
                userRoles.Add(new UserRole { UserId = 1, RoleId = 8 });
                userRoles.Add(new UserRole { UserId = 1, RoleId = 9 });

                foreach (var item in userRoles)
                {
                    await userRoleSet.AddAsync(item);
                }

                await context.SaveChangesAsync();
            }
        }

        public static async Task DocumentTypeInitialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<DataContext>();
            if (context != null)
            {
                var documentTypeSet = context.DocumentTypes;
                if (await documentTypeSet.AnyAsync()) return;

                List<DocumentType> documentTypes = new List<DocumentType>();
                documentTypes.Add(new DocumentType { Id = 1, Name = "Công văn", Status = 1, Note = "", CreateDate = DateTime.Now, CreateUserId = 1 });
                documentTypes.Add(new DocumentType { Id = 2, Name = "Báo cáo", Status = 1, Note = "", CreateDate = DateTime.Now, CreateUserId = 1 });
                documentTypes.Add(new DocumentType { Id = 3, Name = "Giấy mời", Status = 0, Note = "", CreateDate = DateTime.Now, CreateUserId = 1 });
                documentTypes.Add(new DocumentType { Id = 4, Name = "Quyết định", Status = 0, Note = "", CreateDate = DateTime.Now, CreateUserId = 1 });
                documentTypes.Add(new DocumentType { Id = 5, Name = "Nghị định", Status = 0, Note = "", CreateDate = DateTime.Now, CreateUserId = 1 });
                documentTypes.Add(new DocumentType { Id = 6, Name = "Thông báo", Status = 0, Note = "", CreateDate = DateTime.Now, CreateUserId = 1 });
                documentTypes.Add(new DocumentType { Id = 7, Name = "Tờ trình", Status = 0, Note = "", CreateDate = DateTime.Now, CreateUserId = 1 });
                documentTypes.Add(new DocumentType { Id = 8, Name = "Thông tư", Status = 0, Note = "", CreateDate = DateTime.Now, CreateUserId = 1 });

                foreach (var item in documentTypes)
                {
                    await documentTypeSet.AddAsync(item);
                }
                await context.SaveChangesAsync();
            }
        }

        public static async Task CategoryProfileInitialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<DataContext>();
            if (context != null)
            {
                var categoryProfileSet = context.Profiles;
                if (await categoryProfileSet.AnyAsync()) return;

                CategoryProfile profile =  new CategoryProfile {Id = 1, Symbol = "123/NĐ", Title = "Hồ sơ nguyên tắc", Expiry = "Lâu dài", CreateUserId = 1, CreateDate = DateTime.Now, Status = 1, Deadline = DateTime.Now.AddMonths(1), OrderBy = 1};
                await categoryProfileSet.AddAsync(profile);
                await context.SaveChangesAsync();
            }
        }

        public static async Task JobProfileInitialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<DataContext>();
            if (context != null)
            {
                var jobProfileSet = context.JobProfiles;
                if (await jobProfileSet.AnyAsync()) return;

                JobProfile profile = new JobProfile {Id = 1, ProfileId = 1, Condition = "", Name = "Hồ sơ quản lý nghiệp vụ", CreateUserId = 1, CreateDate = DateTime.Now, Status = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), NumberPaper = 5 };
                await jobProfileSet.AddAsync(profile);
                await context.SaveChangesAsync();
            }
        }

        private static string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
    }
}
