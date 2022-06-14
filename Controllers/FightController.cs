using System.Threading.Tasks;
using dotnetrpg.Dtos.Fight;
using dotnetrpg.models;
using dotnetrpg.Services.FightService;
using Microsoft.AspNetCore.Mvc;

namespace dotnetrpg.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class FightController : ControllerBase
  {
    private readonly IFightService _fightService;

    public FightController(IFightService fightService)
    {
      _fightService = fightService;
    }

    [HttpPost("WeaponAttack")]
    public async Task<ActionResult<ServiceResponse<AttackResultDto>>> WeaponAttack(WeaponAttackDto weaponAttackDto)
    {
      var response = await _fightService.WeaponAttack(weaponAttackDto);
      if (!response.Success)
      {
        return BadRequest(response);
      }
      return Ok(response);
    }
  }
}