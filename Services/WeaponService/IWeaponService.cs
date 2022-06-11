using System.Threading.Tasks;
using dotnetrpg.Dtos.Character;
using dotnetrpg.Dtos.Weapon;
using dotnetrpg.models;

namespace dotnetrpg.Services.WeaponService
{
  public interface IWeaponService
  {
    Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
  }
}