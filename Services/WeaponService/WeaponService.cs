using System;
using System.Threading.Tasks;
using AutoMapper;
using dotnetrpg.Data;
using dotnetrpg.Dtos.Character;
using dotnetrpg.Dtos.Weapon;
using dotnetrpg.Helpers.UserContext;
using dotnetrpg.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace dotnetrpg.Services.WeaponService
{
  [Authorize]
  public class WeaponService : IWeaponService
  {
    private readonly IMapper _mapper;
    private readonly DataContext _dataContext;
    private readonly IUserContext _userContext;

    public WeaponService(DataContext dataContext, IUserContext userContext, IMapper mapper)
    {
      _mapper = mapper;
      _dataContext = dataContext;
      _userContext = userContext;
    }

    public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
    {
      var response = new ServiceResponse<GetCharacterDto>();
      try
      {
        var character = await _dataContext.Characters
          .FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId && c.UserId == _userContext.GetCurrentUserId());

        if (character == null)
        {
          response.Success = false;
          response.Message = "Character not found";
          return response;
        }

        var weapon = new Weapon
        {
          Name = newWeapon.Name,
          Damage = newWeapon.Damage,
          CharacterId = character.Id,
        };

        _dataContext.Weapons.Add(weapon);
        await _dataContext.SaveChangesAsync();

        var resolvedCharacter = _mapper.Map<GetCharacterDto>(character);
        resolvedCharacter.Weapon = _mapper.Map<GetWeaponDto>(weapon);

        response.Data = _mapper.Map<GetCharacterDto>(resolvedCharacter);
      }
      catch (Exception ex)
      {
        response.Success = false;
        response.Message = ex.ToString();
      }
      return response;
    }
  }
}