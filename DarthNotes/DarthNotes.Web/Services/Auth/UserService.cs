using DarthNotes.Core.Enums;
using DarthNotes.Db.Entities;
using DarthNotes.Db.Repositories;

namespace DarthNotes.Web.Services.Auth;

public interface IUserService
{
    public Task<int?> GetUserIdAsync(string email, UserTypeEnum userType);
}

public class UserService : IUserService
{
    private readonly IBaseRepository<UserEntity> _usersRepository;
    
    public UserService(IBaseRepository<UserEntity> usersRepository)
    {
        _usersRepository = usersRepository;
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
}