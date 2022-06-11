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
    private readonly ILogger _logger;

    public CharacterController(ICharacterService characterService, ILogger<CharacterController> logger)
    {
      _characterService = characterService;
      _logger = logger;
    }

    [HttpGet("all")]
    public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> GetAllCharacters()
    {
      var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
      return Ok(await _characterService.GetAllCharacters(userId));
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

    [HttpPut]
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
  }
}