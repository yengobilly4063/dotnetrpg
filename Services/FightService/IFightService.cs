using System.Threading.Tasks;
using dotnetrpg.Dtos.Fight;
using dotnetrpg.models;

namespace dotnetrpg.Services.FightService
{
  public interface IFightService
  {
    Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto requestDto);
    Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto requestDto);
    Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto requestDto);
  }
}