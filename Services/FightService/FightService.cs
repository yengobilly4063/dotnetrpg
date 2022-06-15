using System;
using System.Collections.Generic;
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
        int damage = DoWeaponAttack(attacker, opponent);

        UpdateAtackDefeatVictoryHistory(serviceResponse, attacker, opponent);
        UpdateCharacterFightInfo(attacker, opponent);

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

        int damage = DoSkillAttack(attacker, opponent, attackerSkill);

        UpdateAtackDefeatVictoryHistory(serviceResponse, attacker, opponent);
        UpdateCharacterFightInfo(attacker, opponent);

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

    public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto requestDto)
    {
      var serviceResponse = new ServiceResponse<FightResultDto>
      {
        Data = new FightResultDto()
      };

      try
      {
        var characters = await _dataContext.Characters
          .Include(c => c.Weapon)
          .Include(c => c.Skills)
          .Where(c => requestDto.CharacterIds.Contains(c.Id)).ToListAsync();

        bool defeated = false;
        while (!defeated)
        {
          foreach (var attacker in characters)
          {
            var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
            var opponent = opponents[new Random().Next(opponents.Count)];

            var damage = 0;
            string attackUsed = string.Empty;
            var useWeapon = new Random().Next(2) == 0;
            if (useWeapon)
            {
              attackUsed = attacker.Weapon.Name;
              damage = DoWeaponAttack(attacker, opponent);
              UpdateCharacterFightInfo(attacker, opponent);
              UpdateAtackDefeatVictoryHistory(serviceResponse, attacker, opponent, ref defeated);
              UpdateServiceResponseDataLogs(serviceResponse, attacker, opponent, damage, attackUsed);
            }
            else
            {
              var attackerSkill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
              attackUsed = attackerSkill.Name;
              damage = DoSkillAttack(attacker, opponent, attackerSkill);
              UpdateCharacterFightInfo(attacker, opponent);
              UpdateAtackDefeatVictoryHistory(serviceResponse, attacker, opponent, ref defeated);
              UpdateServiceResponseDataLogs(serviceResponse, attacker, opponent, damage, attackUsed);

            }

            if (defeated) break;
          }
        }
        ResetCharactersHitPoints(characters);

        await _dataContext.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Message = ex.Message;
      }
      return serviceResponse;
    }

    private static void UpdateServiceResponseDataLogs(ServiceResponse<FightResultDto> serviceResponse, Character attacker, Character opponent, int damage, string attackUsed)
    {
      serviceResponse.Data.Logs.Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage");
      serviceResponse.Data.Logs.Add($"{opponent.Name} has been defeated by {attacker.Name}");
      serviceResponse.Data.Logs.Add($"{attacker.Name} wins attack with {attacker.HitPoints} HP left!");
    }

    private static void ResetCharactersHitPoints(List<Character> characters)
    {
      characters.ForEach(c =>
      {
        c.HitPoints = 200;
      });
    }

    private static int DoWeaponAttack(Character attacker, Character opponent)
    {
      int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
      damage -= new Random().Next(opponent.Defense);

      if (damage > 0)
        opponent.HitPoints -= damage;
      return damage;
    }

    private static int DoSkillAttack(Character attacker, Character opponent, Skill attackerSkill)
    {
      int damage = attackerSkill.Damage + (new Random().Next(attacker.Intelligence));
      damage -= new Random().Next(opponent.Defense);

      if (damage > 0)
        opponent.HitPoints -= damage;
      return damage;
    }

    private static void UpdateCharacterFightInfo(Character attacker, Character opponent)
    {
      opponent.Fights++;
      attacker.Fights++;
    }

    private static void UpdateAtackDefeatVictoryHistory(ServiceResponse<AttackResultDto> serviceResponse, Character attacker, Character opponent)
    {
      if (opponent.HitPoints <= 0)
      {
        serviceResponse.Message = $"{opponent.Name} has been defeated by {attacker.Name}";
        attacker.Victories++;
        opponent.Defeats++;
      }
    }

    private static void UpdateAtackDefeatVictoryHistory(ServiceResponse<FightResultDto> serviceResponse, Character attacker, Character opponent, ref bool defeated)
    {
      if (opponent.HitPoints <= 0)
      {
        defeated = true;
        serviceResponse.Message = $"{opponent.Name} has been defeated by {attacker.Name}";
        attacker.Victories++;
        opponent.Defeats++;
      }
    }
  }
}