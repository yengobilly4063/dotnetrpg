using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetrpg.Dtos.Character;
using dotnetrpg.models;
using dotnetrpg.Services.CharacterService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dotnetrpg.Controllers
{
  [Authorize]
  [ApiController()]
  [Route("character")]
  public class CharacterController : ControllerBase
  {

    private readonly ICharacterService _characterService;

    public CharacterController(ICharacterService characterService)
    {
      _characterService = characterService;
    }

    [HttpGet("all")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> GetAllCharacters()
    {
      return Ok(await _characterService.GetAllCharacters());
    }

    [HttpGet("{Id}")]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetCharacter(int Id)
    {
      return Ok(await _characterService.GetCharacterById(Id));
    }

    [HttpPost()]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter(AddCharacterDto newCharacter)
    {
      return Ok(await _characterService.AddCharacter(newCharacter));
    }

    [HttpPut()]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
    {
      var response = await _characterService.UpdateCharacter(updatedCharacter);
      if (!response.Success)
      {
        return NotFound(response);
      }
      return Ok(response);
    }

    [HttpDelete("{Id}")]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> DeleteCharacter(int Id)
    {
      var response = await _characterService.DeleteCharacter(Id);
      if (!response.Success)
      {
        return NotFound(response);
      }
      return Ok(response);
    }

    [HttpPost("Skill")]
    public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
    {
      var response = await _characterService.AddCharacterSkill(newCharacterSkill);
      if (!response.Success)
      {
        return NotFound(response);
      }
      return Ok(response);
    }
  }
}