using System.Collections.Generic;
using System.Threading.Tasks;
using dotnetrpg.Dtos.Character;
using dotnetrpg.models;

namespace dotnetrpg.Services.CharacterService
{
  public interface ICharacterService
  {
    Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters(int userId);
    Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int Id);
    Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter);
    Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacterDto);
    Task<ServiceResponse<GetCharacterDto>> DeleteCharacter(int Id);
  }
}