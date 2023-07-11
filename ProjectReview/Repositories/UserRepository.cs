using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectReview.DTO.Users;
using ProjectReview.Models;
using ProjectReview.Models.Entities;

namespace ProjectReview.Repositories
{
	public interface IUserRepository
	{
		Task<UserDTO> Create(CreateUserDTO userDTO);
		Task<UserDTO> GetUser(string name, string password);
		Task<bool> GetUserByEmail(string email);
		Task<bool> GetUserByUserName(string userName);
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
			await _dataContext.AddAsync(user);
			await _dataContext.SaveChangesAsync();
			return _mapper.Map<User, UserDTO>(user);
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
							.Where(x => x.Email == email)
							.FirstOrDefaultAsync();
			if (result == null) return false;
			return true;
		}

		public async Task<bool> GetUserByUserName(string userName)
		{
			var result = await _dataContext.Users
							.Where(x => x.UserName == userName)
							.FirstOrDefaultAsync();
			if (result == null) return false;
			return true;
		}
	}
}
