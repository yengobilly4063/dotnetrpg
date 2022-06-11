using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace dotnetrpg.Helpers.UserContext
{
  public class UserContext : IUserContext
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    /**
    @Note only used in [Authorized] based controllers calling services
    */
    public int GetCurrentUserId()
    {
      return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue("UserId"));
    }
  }
}