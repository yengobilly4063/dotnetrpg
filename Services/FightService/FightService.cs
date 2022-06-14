using System;
using System.Linq;
using System.Threading.Tasks;
using dotnetrpg.Data;
using dotnetrpg.Dtos.Fight;
using dotnetrpg.models;
using Microsoft.EntityFrameworkCore;

namespace dotnetrpg.Services.FightService
{
  public class FightService : IFightService
  {
    private readonly DataContext _dataContext;
    public FightService(DataContext dataContext)
    {
      _dataContext = dataContext;
    }



    public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto requestDto)
    {
      var serviceResponse = new ServiceResponse<AttackResultDto>();
      try
      {
        var attacker = await _dataContext.Characters
          .Include(c => c.Weapon)
          .FirstOrDefaultAsync(c => c.Id == requestDto.AttackerId);

        var opponent = await _dataContext.Characters.FirstOrDefaultAsync(c => c.Id == requestDto.OponentId);

        int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
        damage -= new Random().Next(opponent.Defense);

        if (damage > 0)
          opponent.HitPoints -= damage;
        if (opponent.HitPoints <= 0)
        {
          serviceResponse.Message = $"{opponent.Name} has been defeated";
          attacker.Victories++;
          opponent.Defeats++;
        }

        // Update fight information for each character
        opponent.Fights++;
        attacker.Fights++;

        await _dataContext.SaveChangesAsync();

        serviceResponse.Data = new AttackResultDto
        {
          Attacker = attacker.Name,
          Opponent = opponent.Name,
          AttackerHP = attacker.HitPoints,
          OpponentHP = opponent.HitPoints,
          Damage = damage
        };
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Message = ex.ToString();
      }
      return serviceResponse;
    }

    public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto requestDto)
    {
      var serviceResponse = new ServiceResponse<AttackResultDto>();
      try
      {
        var attacker = await _dataContext.Characters
          .Include(c => c.Skills)
          .FirstOrDefaultAsync(c => c.Id == requestDto.AttackerId);

        var opponent = await _dataContext.Characters.FirstOrDefaultAsync(c => c.Id == requestDto.OponentId);

        var attackerSkill = attacker.Skills.FirstOrDefault(s => s.Id == requestDto.AttackerSkillId);

        if (attackerSkill == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Message = $"Attacker {attacker.Name} does not have this skill";
          return serviceResponse;
        }

        int damage = attackerSkill.Damage + (new Random().Next(attacker.Intelligence));
        damage -= new Random().Next(opponent.Defense);

        if (damage > 0)
          opponent.HitPoints -= damage;
        if (opponent.HitPoints <= 0)
        {
          serviceResponse.Message = $"{opponent.Name} has been defeated";
          attacker.Victories++;
          opponent.Defeats++;
        }

        // Update fight information for each character
        opponent.Fights++;
        attacker.Fights++;

        await _dataContext.SaveChangesAsync();

        serviceResponse.Data = new AttackResultDto
        {
          Attacker = attacker.Name,
          Opponent = opponent.Name,
          AttackerHP = attacker.HitPoints,
          OpponentHP = opponent.HitPoints,
          Damage = damage
        };
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Message = ex.ToString();
      }
      return serviceResponse;
    }
  }
}