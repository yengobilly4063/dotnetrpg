using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnetrpg.Data;
using dotnetrpg.Dtos.Character;
using dotnetrpg.models;
using Microsoft.EntityFrameworkCore;

namespace dotnetrpg.Services.CharacterService
{
  public class CharacterService : ICharacterService
  {
    private readonly IMapper _mapper;
    private readonly DataContext _dataContext;

    public CharacterService(IMapper mapper, DataContext dataContext)
    {
      _mapper = mapper;
      _dataContext = dataContext;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
      var dbCharacters = await _dataContext.Characters.ToListAsync();
      var mappedCharacterResponse = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
      return new ServiceResponse<List<GetCharacterDto>>(mappedCharacterResponse);
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int Id)
    {
      var dbCharacter = await _dataContext.Characters.FirstOrDefaultAsync(c => c.Id == Id);
      var characterFound = _mapper.Map<GetCharacterDto>(dbCharacter);
      return new ServiceResponse<GetCharacterDto>(characterFound);
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
      var resolvedCharacter = _mapper.Map<Character>(newCharacter);
      _dataContext.Characters.Add(resolvedCharacter);
      await _dataContext.SaveChangesAsync();
      var mappedCharacterResponse = await _dataContext.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
      return new ServiceResponse<List<GetCharacterDto>>(mappedCharacterResponse);
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
      try
      {
        var character = await _dataContext.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
        character.Name = updatedCharacter.Name;
        character.HitPoints = updatedCharacter.HitPoints;
        character.Strength = updatedCharacter.Strength;
        character.Defense = updatedCharacter.Defense;
        character.Intelligence = updatedCharacter.Intelligence;
        character.Class = updatedCharacter.Class;

        await _dataContext.SaveChangesAsync();

        return new ServiceResponse<GetCharacterDto>(_mapper.Map<GetCharacterDto>(character));
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        return new ServiceResponse<GetCharacterDto>(null, false, ex.Message);
      }
    }

    public async Task<ServiceResponse<GetCharacterDto>> DeleteCharacter(int Id)
    {
      try
      {
        var character = await _dataContext.Characters.FirstAsync(c => c.Id == Id);
        _dataContext.Remove(character);
        await _dataContext.SaveChangesAsync();
        return new ServiceResponse<GetCharacterDto>(_mapper.Map<GetCharacterDto>(character), true, $"{character.Name} was deleted successfully!");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        return new ServiceResponse<GetCharacterDto>(null, false, $"Character not deleted => {ex.Message}");
      }
    }
  }
}