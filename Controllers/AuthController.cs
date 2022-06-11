using System.Threading.Tasks;
using dotnetrpg.Dtos.User;
using dotnetrpg.models;
using dotnetrpg.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace dotnetrpg.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly IAuthRepository _authRepository;
    public AuthController(IAuthRepository authRepository)
    {
      _authRepository = authRepository;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto userRequest)
    {
      var response = await _authRepository.Register(
          new User { Username = userRequest.Username }, userRequest.Password
        );
      if (!response.Success)
      {
        return BadRequest(response);
      }
      return Ok(response);
    }

    [HttpPost("Login")]
    public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDto userRequest)
    {
      var response = await _authRepository.Login(userRequest.Username, userRequest.Password);
      if (!response.Success)
      {
        return BadRequest(response);
      }
      return Ok(response);
    }
  }
}