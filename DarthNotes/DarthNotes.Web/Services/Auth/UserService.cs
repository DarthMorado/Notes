using System.Security.Cryptography;
using System.Text;
using DarthNotes.Enums;
using DarthNotes.DB.Entities;
using DarthNotes.DB.Repositories;
using DarthNotes.Web.DTO;
using MapsterMapper;

namespace DarthNotes.Web.Services.Auth;

public interface IUserService
{
    public Task<int?> GetUserIdAsync(string email, UserTypeEnum userType);
    public Task<UserDto> GetUserAsync(int userId);
    public Task UpdateUserProfileAsync(int userId, bool isPasswordAuthEnabled, string? additionalPassword);
    Task<string?> GenerateAuthToken(int userId);
}

public class UserService : IUserService
{
    private readonly IBaseRepository<UserEntity> _usersRepository;
    private readonly IMapper _mapper;
    
    public UserService(IBaseRepository<UserEntity> usersRepository,
        IMapper mapper)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
    }
    
    public async Task<int?> GetUserIdAsync(string email, UserTypeEnum userType)
    {
        var dbUsers = await _usersRepository.FindAsync(x => x.Username == email && x.UserType == userType);
        if (dbUsers.Any())
        {
            if (dbUsers.Count > 1) return null;
            return dbUsers.First().Id;
        }
        else
        {
            //Create new User and return it's Id
            var newUser = new UserEntity()
            {
                Username = email,
                UserType = userType
            };
            await _usersRepository.AddAsync(newUser);
            await _usersRepository.SaveChangesAsync();
            return newUser.Id;
        }
        throw new NotImplementedException();
    }

    public async Task<UserDto> GetUserAsync(int userId)
    {
        var entity = await _usersRepository.GetByIdAsync(userId);
        var user = _mapper.Map<UserDto>(entity);
        return user;
    }

    public async Task UpdateUserProfileAsync(int userId, bool isPasswordAuthEnabled, string? additionalPassword)
    {
        var entity = await _usersRepository.GetByIdAsync(userId);
        entity.IsPasswordAuthEnabled = isPasswordAuthEnabled;

        if (!String.IsNullOrWhiteSpace(additionalPassword))
        {
            entity.AdditionalPasswordHash = Md5(additionalPassword);
        }
        else
        {
            entity.AdditionalPasswordHash = null;
        }

        await _usersRepository.UpdateAsync(entity);
        await _usersRepository.SaveChangesAsync();
    }

    public async Task<string?> GenerateAuthToken(int userId)
    {
        var user = await _usersRepository.GetByIdAsync(userId);
        user.Token = Guid.NewGuid().ToString();
        user.TokenDate = DateTime.UtcNow;
        await _usersRepository.UpdateAsync(user);
        await _usersRepository.SaveChangesAsync();
        return user.Token;
    }

    private string Md5(string input)
    {
        using var md5 = MD5.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);
        return Convert.ToHexString(hashBytes).ToLower();

    }
}