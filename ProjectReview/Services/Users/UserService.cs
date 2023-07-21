using Microsoft.AspNetCore.Identity;
using ProjectReview.Common;
using ProjectReview.DTO.Departments;
using ProjectReview.DTO.PermissionGroups;
using ProjectReview.DTO.Positions;
using ProjectReview.DTO.Ranks;
using ProjectReview.DTO.Users;
using ProjectReview.Models.Entities;
using ProjectReview.Paging;
using ProjectReview.Repositories;
using System.Security.Cryptography;

namespace ProjectReview.Services.Users
{
	public interface IUserService {
        Task Delete(long id);
        Task Active(long id);
        Task<UserDTO> Create(CreateUserDTO userDTO);
		Task<UserDTO> Login(string name, string password);
        Task<CustomPaging<UserDTO>> GetCustomPaging(string? filter, int page, int pageSize);
        Task<UpdateUserDTO> GetById(long id);
        Task<UserDTO> Update(UpdateUserDTO userDTO);
        Task<List<RankDTO>> GetRank();
        Task<List<DepartmentDTO>> GetDepartment();
        Task<List<PermissionGroupDTO>> GetPermissionGroup();
        Task<List<PositionDTO>> GetPosition();
    }

	public class UserService : IUserService
	{
		private readonly IUnitOfWork _UOW;
		private readonly ICurrentUser _currentUser;
        public UserService(IUnitOfWork UOW, ICurrentUser currentUser)
		{
			_UOW = UOW;
			_currentUser = currentUser;
		}

        public async Task Delete(long id)
        {
            await _UOW.UserRepository.Delete(id);
        }

        public async Task Active(long id)
        {
            await _UOW.UserRepository.Active(id);
        }

        public async Task<UserDTO> Create(CreateUserDTO userDTO)
		{
			userDTO.UserName = userDTO.UserName.Trim();
			var user = await _UOW.UserRepository.GetUserByUserName(userDTO.UserName);
			if (user != null) throw new Exception("Tên người dụng đã tồn tại");
			userDTO.PasswordHash = HashPassword("123456");
			var result = await _UOW.UserRepository.Create(userDTO);
			if (result == null) return null;
            result.PasswordHash = "";
            return result;
		}

        public async Task<UserDTO> Update(UpdateUserDTO userDTO)
        {
            userDTO.UserName = userDTO.UserName.Trim();
            var user = await _UOW.UserRepository.GetUserByUserName(userDTO.UserName);
            if (user != null && user.Id != userDTO.Id)
			{
                    throw new Exception("Tên người dụng đã tồn tại");
            }
            var result = await _UOW.UserRepository.Update(userDTO);
            if (result == null) return null;
            result.PasswordHash = "";
            return result;
        }

        public async Task<CustomPaging<UserDTO>> GetCustomPaging(string? filter, int page, int pageSize)
        {
            filter = (filter ?? "");
            var result = await _UOW.UserRepository.GetCustomPaging(filter, page, pageSize);
			if (result.Data.Count > 0)
			{
				foreach(var  item in result.Data)
				{
					item.PasswordHash = "";
				}
			}
			return result;
        }

        public async Task<UserDTO> Login(string name, string password)
		{
			name = name.Trim();
			var user = await _UOW.UserRepository.GetUserByUserName(name);
			if (user == null) throw new Exception("Tên người dùng không tồn tại");
			if(VerifyPassword(user.PasswordHash, password))
			{
                _currentUser.UserId = user.Id;
                _currentUser.UserName = user.UserName;
                _currentUser.FullName = user.FullName;
                _currentUser.Email = user.Email;
                _currentUser.Roles = await _UOW.UserRepository.GetRole(user.Id);
				user.PasswordHash = "";
                return user;
            } else
			{
				throw new Exception("Mật khẩu không chính xác");
			}
			
		}

        public async Task<UpdateUserDTO> GetById(long id)
        {
            return await _UOW.UserRepository.GetById(id);
        }

        public async Task<List<RankDTO>> GetRank()
        {
            return await _UOW.RankRepository.GetAllActive();
        }

        public async Task<List<DepartmentDTO>> GetDepartment()
        {
            return await _UOW.DepartmentRepository.GetAllActive();
        }

        public async Task<List<PermissionGroupDTO>> GetPermissionGroup()
        {
            return await _UOW.PermissionGroupRepository.GetAllActive();
        }

        public async Task<List<PositionDTO>> GetPosition()
        {
            return await _UOW.PositionRepository.GetAllActive();
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

        private bool VerifyPassword(string oldPassword, string newPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(oldPassword);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(newPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }
    }
}
