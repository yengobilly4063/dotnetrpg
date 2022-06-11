using System.Threading.Tasks;
using dotnetrpg.models;

namespace dotnetrpg.Repositories
{
  public interface IAuthRepository
  {
    Task<ServiceResponse<int>> Register(User user, string password);
    Task<ServiceResponse<string>> Login(string username, string password);
    Task<bool> UsertExists(string username);
  }
}