using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using dotnetrpg.models;
using dotnetrpg.Data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace dotnetrpg.Repositories
{
  public class AuthRepository : IAuthRepository
  {
    private readonly DataContext _context;
    private readonly IConfiguration _config;
    public AuthRepository(DataContext context, IConfiguration config)
    {
      _context = context;
      _config = config;
    }

    public async Task<ServiceResponse<string>> Login(string username, string password)
    {
      var response = new ServiceResponse<string>();
      var user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));
      if (user == null)
      {
        response.Success = false;
        response.Message = "User not found";
      }
      else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
      {
        response.Success = false;
        response.Message = "Wrong password";
      }
      else
      {
        response.Data = CreateToken(user);
      }
      return response;
    }

    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
      var response = new ServiceResponse<int>();
      if (await UsertExists(user.Username))
      {
        response.Success = false;
        response.Message = "User already exists";
        return response;
      }
      CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
      user.PasswordHash = passwordHash;
      user.PasswordSalt = passwordSalt;

      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      response.Data = user.Id;
      return response;
    }

    public async Task<bool> UsertExists(string username)
    {
      if (await _context.Users.AnyAsync(user => user.Username.ToLower().Equals(username.ToLower())))
      {
        return true;
      }
      return false;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (var hmac = new System.Security.Cryptography.HMACSHA512())
      {
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
      }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
      using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
      {
        var computedPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < passwordHash.Length; i++)
        {
          if (computedPasswordHash[i] != passwordHash[i])
          {
            return false;
          }
        }
        return true;
      }
    }

    private string CreateToken(User user)
    {
      var claims = new List<Claim>
      {
        // new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        // new Claim(ClaimTypes.Name, user.Username)

        new Claim("UserId", user.Id.ToString()),
        new Claim("Username", user.Username)
      };

      var jwtTokenKey = _config.GetSection("AppSettings:JWTTokenKey").Value;
      var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtTokenKey));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = System.DateTime.Now.AddDays(1),
        SigningCredentials = creds
      };
      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
    }
  }
}