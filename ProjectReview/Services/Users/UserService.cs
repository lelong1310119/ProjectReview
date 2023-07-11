using ProjectReview.Common;
using ProjectReview.DTO.Users;
using ProjectReview.Repositories;
using System.Security.Cryptography;

namespace ProjectReview.Services.Users
{
	public interface IUserService {
		Task<UserDTO> Create(CreateUserDTO userDTO);
		Task<UserDTO> Login(string name, string password);

	}

	public class UserService
	{
		private readonly IUnitOfWork _UOW;
		private readonly ICurrentUser _currentUser;
		public UserService(IUnitOfWork UOW, ICurrentUser currentUser)
		{
			_UOW = UOW;
			_currentUser = currentUser;
		}
		public async Task<UserDTO> Create(CreateUserDTO userDTO)
		{
			userDTO.UserName = userDTO.UserName.Trim();
			var user = await _UOW.UserRepository.GetUserByUserName(userDTO.UserName);
			if (user == true) throw new Exception("Tên người dụng đã tồn tại");
			userDTO.PasswordHash = HashPassword("123456");
			var result = await _UOW.UserRepository.Create(userDTO);
			if (result == null) return null;
			return result;
		}

		public async Task<UserDTO> Login(string name, string password)
		{
			name = name.Trim();
			password = HashPassword(password);
			var user = await _UOW.UserRepository.GetUser(name, password);
			if (user == null) throw new Exception("Tên đăng nhập hoặc mật khẩu không đúng");
			_currentUser.UserId = user.Id;
			_currentUser.UserName = user.UserName;
			_currentUser.FullName = user.FullName;
			_currentUser.Email = user.Email;
			return user;
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
