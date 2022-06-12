using AutoMapper;
using dotnetrpg.Dtos.Character;
using dotnetrpg.Dtos.Skill;
using dotnetrpg.Dtos.Weapon;
using dotnetrpg.models;

namespace dotnetrpg
{
  public class AutoMapperProfile : Profile
  {
    public AutoMapperProfile()
    {
      CreateMap<Character, GetCharacterDto>();
      CreateMap<AddCharacterDto, Character>();
      CreateMap<Weapon, GetWeaponDto>();
      CreateMap<Skill, GetSkillDto>();
    }
  }
}