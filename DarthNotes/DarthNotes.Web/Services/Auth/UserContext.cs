namespace DarthNotes.Web.Services.Auth;

public interface IUserContext
{
    int? GetUserId();
    string GetUserName();
    bool IsLoggedIn();
}

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public int? GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user is null) return null;
        
        var userId = user.FindFirst("Id")?.Value;
        if (String.IsNullOrWhiteSpace(userId)) return null;

        if (!int.TryParse(userId, out var userIdValue)) return null;

        return userIdValue;
    }

    public string GetUserName()
    {
        throw new NotImplementedException();
    }

    public bool IsLoggedIn()
        => GetUserId().HasValue;
}