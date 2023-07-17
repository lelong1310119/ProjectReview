using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.DTO.Users;
using ProjectReview.Models;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;
using System.Net.WebSockets;

namespace ProjectReview.Repositories
{
	public interface IUserRepository
	{
        Task Delete(long id);
        Task Active(long id);
        Task<UserDTO> Create(CreateUserDTO userDTO);
		Task<UserDTO> GetUser(string name, string password);
		Task<bool> GetUserByEmail(string email);
		Task<UserDTO> GetUserByUserName(string userName);
        Task<UserDTO> Update(UpdateUserDTO updateUser);
        Task<List<UserDTO>> GetAllActive();
        Task<CustomPaging<UserDTO>> GetCustomPaging(string filter, int page, int pageSize);
        Task<UpdateUserDTO> GetById(long id);
        Task<List<UserDTO>> GetAllUser();
        Task<List<UserDTO>> GetUserByBirthday(DateTime date);
        Task<List<UserDTO>> GetHostUser();
	}

	public class UserRepository : IUserRepository
	{
		private readonly DataContext _dataContext;
		private readonly IMapper _mapper;

		public UserRepository(DataContext dataContext, IMapper mapper)
		{
			_dataContext = dataContext;
			_mapper = mapper;
		}

		public async Task<UserDTO> Create(CreateUserDTO userDTO)
		{
			var user = _mapper.Map<CreateUserDTO, User>(userDTO);
            long maxId = await _dataContext.Users.MaxAsync(x => x.Id);
            user.Id = maxId + 1;
            user.Status = 0;
            user.CreateDate = DateTime.Now;
			await _dataContext.AddAsync(user);
            await _dataContext.SaveChangesAsync();
			await UpdateRole(user.Id, user.PermissionGroupId);
			return _mapper.Map<User, UserDTO>(user);
		}

        public async Task<List<UserDTO>> GetUserByBirthday(DateTime date)
        {
            var result = await _dataContext.Users
                                    .Where(x => (x.Birthday.Day == date.Day && x.Birthday.Month == date.Month && x.Status == 1))
                                    .ToListAsync();
            return _mapper.Map<List<User>, List<UserDTO>>(result);   
        }

        public async Task<UserDTO> Update(UpdateUserDTO updateUser)
        {
            var user = await _dataContext.Users
                                    .Where(x => x.Id == updateUser.Id)
                                    .FirstOrDefaultAsync();
            if (user == null) return null;
			user.UserName = updateUser.UserName.Trim();
			user.FullName = updateUser.FullName;
			user.Email = updateUser.Email;
			user.Gender = updateUser.Gender;
			user.Birthday = updateUser.Birthday;
			user.Note = updateUser.Note;
			user.PositionId = updateUser.PositionId;
			user.PermissionGroupId = updateUser.PermissionGroupId;
			user.RankId = updateUser.RankId;
			user.DepartmentId = updateUser.DepartmentId;

            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
			await UpdateRole(user.Id, user.PermissionGroupId);
			return _mapper.Map<User, UserDTO>(user);
        }

        public async Task Delete(long id)
        {
            var result = await _dataContext.Users
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
            if (result == null) return;
			await _dataContext.UserRoles
				.Where(x => x.UserId == result.Id)
				.ExecuteDeleteAsync();
			_dataContext.Remove(result);
			await _dataContext.SaveChangesAsync();
        }

        public async Task Active(long id)
        {
            var result = await _dataContext.Users
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
            if (result == null) return;
            result.Status = 1;
            _dataContext.Users.Update(result);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<UserDTO> GetUser(string name, string password)
		{
			var result = await _dataContext.Users
							.Where(x => (x.UserName == name) && (x.PasswordHash == password))
							.FirstOrDefaultAsync();
			if (result == null) return null;
			return _mapper.Map<User, UserDTO>(result);
		}

		public async Task<bool> GetUserByEmail(string email)
		{
			var result = await _dataContext.Users
							.Where(x => (x.Email == email && x.Status == 1))
							.FirstOrDefaultAsync();
			if (result == null) return false;
			return true;
		}

		public async Task<UserDTO> GetUserByUserName(string userName)
		{
			var result = await _dataContext.Users
							.Where(x => x.UserName == userName)
							.FirstOrDefaultAsync();
			if (result == null) return null;
			return _mapper.Map<User, UserDTO>(result);
		}

        public async Task<List<UserDTO>> GetAllActive()
        {
            var result = await _dataContext.Users
                                    .Where(x => ((x.Status == 1) && (x.UserName != "superadmin")))
                                    .ToListAsync();
            return _mapper.Map<List<User>, List<UserDTO>>(result);
        }

        public async Task<List<UserDTO>> GetAllUser()
        {
            var result = await _dataContext.Users
                                    .Where(x => x.Status == 1)
                                    .ToListAsync();
            return _mapper.Map<List<User>, List<UserDTO>>(result);
        }

        public async Task<List<UserDTO>> GetHostUser()
        {
			var result = await _dataContext.Users
									.Where(x => (x.Status == 1 && x.DepartmentId == 1))
									.ToListAsync();
			return _mapper.Map<List<User>, List<UserDTO>>(result);
		}

        public async Task<CustomPaging<UserDTO>> GetCustomPaging(string filter, int page, int pageSize)
        {
            int count = await _dataContext.Users
                                        .Where(x => ((x.FullName.Contains(filter)) && (x.UserName != "superadmin")))
                                        .CountAsync();
            var result = await _dataContext.Users
                                        .Where(x => ((x.FullName.Contains(filter)) && (x.UserName != "superadmin")))
                                        .Include(x => x.Position)
                                        .Include(x => x.PermissionGroup)
                                        .Include(x => x.Rank)
                                        .Include(x => x.Department)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
            int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
            var users = _mapper.Map<List<User>, List<UserDTO>>(result);
            CustomPaging<UserDTO> paging = new CustomPaging<UserDTO>
            {
                TotalPage = totalPage,
                PageSize = pageSize,
                Data = users
            };
            return paging;
        }

        public async Task<UpdateUserDTO> GetById(long id)
        {
            var result = await _dataContext.Users
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
            if (result == null) return null;
            return _mapper.Map<User, UpdateUserDTO>(result);
        }

        public async Task UpdateRole(long userId, long permissionId)
        {
            await _dataContext.UserRoles
                        .Where(x => x.UserId == userId)
                        .ExecuteDeleteAsync();
            await _dataContext.SaveChangesAsync();
            var result = await _dataContext.RolePermissions
                            .Where(x => x.PermissionGroupId == permissionId)
                            .ToListAsync();
            if (result.Count > 0)
            {
                foreach(var role in result)
                {
                    await _dataContext.UserRoles.AddAsync(new UserRole { UserId =  userId, RoleId = role.RoleId });
                }
            }
            await _dataContext.SaveChangesAsync();
        }
    }
}
