using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnetrpg.Data;
using dotnetrpg.Dtos.Character;
using dotnetrpg.models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace dotnetrpg.Services.CharacterService
{
  public class CharacterService : ICharacterService
  {
    private readonly IMapper _mapper;
    private readonly DataContext _dataContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CharacterService(IMapper mapper, DataContext dataContext, IHttpContextAccessor httpContextAccessor)
    {
      _mapper = mapper;
      _dataContext = dataContext;
      _httpContextAccessor = httpContextAccessor;
    }

    private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue("UserId"));

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
      var dbCharacters = await _dataContext.Characters
        .Include(c => c.Skills)
        .Include(c => c.Weapon)
        .Where(c => c.User.Id == GetUserId()).ToListAsync();
      var mappedCharacterResponse = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
      return new ServiceResponse<List<GetCharacterDto>>(mappedCharacterResponse);
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int Id)
    {
      var dbCharacter = await _dataContext.Characters
        .Include(c => c.Skills)
        .Include(c => c.Weapon)
        .FirstOrDefaultAsync(c => c.Id == Id && c.User.Id == GetUserId());
      var characterFound = _mapper.Map<GetCharacterDto>(dbCharacter);
      return new ServiceResponse<GetCharacterDto>(characterFound);
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
      var resolvedCharacter = _mapper.Map<Character>(newCharacter);
      resolvedCharacter.User = await _dataContext.Users.FirstOrDefaultAsync(c => c.Id == GetUserId());
      _dataContext.Characters.Add(resolvedCharacter);
      await _dataContext.SaveChangesAsync();
      var mappedCharacterResponse = await _dataContext.Characters
        .Include(c => c.Weapon)
        .Where(c => c.User.Id == GetUserId())
        .Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
      return new ServiceResponse<List<GetCharacterDto>>(mappedCharacterResponse);
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();
      try
      {
        var character = await _dataContext.Characters
          .Include(c => c.Weapon)
          .Include(c => c.User)
          .Where(c => c.User.Id == GetUserId())
          .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);

        if (character != null)
        {
          character.Name = updatedCharacter.Name;
          character.HitPoints = updatedCharacter.HitPoints;
          character.Strength = updatedCharacter.Strength;
          character.Defense = updatedCharacter.Defense;
          character.Intelligence = updatedCharacter.Intelligence;
          character.Class = updatedCharacter.Class;

          await _dataContext.SaveChangesAsync();
          serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
        }
        else
        {
          serviceResponse.Success = false;
          serviceResponse.Message = $"Character not found";
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        serviceResponse.Success = false;
        serviceResponse.Message = ex.ToString();
      }
      return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> DeleteCharacter(int Id)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();
      try
      {
        var character = await _dataContext.Characters
          .Include(c => c.Weapon)
          .FirstOrDefaultAsync(c => c.Id == Id && c.User.Id == GetUserId());
        if (character != null)
        {
          _dataContext.Remove(character);
          await _dataContext.SaveChangesAsync();
          serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
        }
        else
        {
          serviceResponse.Success = false;
          serviceResponse.Message = $"Character not found";
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        serviceResponse.Success = false;
        serviceResponse.Message = ex.ToString();
      }
      return serviceResponse;
    }

    public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
    {
      var serviceResponse = new ServiceResponse<GetCharacterDto>();
      try
      {
        var character = await _dataContext.Characters
          .Include(c => c.Weapon)
          .Include(c => c.Skills)
          .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId && c.User.Id == GetUserId());

        if (character == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Message = "Character not found";
          return serviceResponse;
        }
        var skill = await _dataContext.Skills.FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);
        if (skill == null)
        {
          serviceResponse.Success = false;
          serviceResponse.Message = "Skill not found";
          return serviceResponse;
        }

        character.Skills.Add(skill);
        await _dataContext.SaveChangesAsync();

        serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
      }
      catch (Exception ex)
      {
        serviceResponse.Success = false;
        serviceResponse.Message = ex.Message;
        throw;
      }
      return serviceResponse;
    }
  }
}