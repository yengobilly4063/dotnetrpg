using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnetrpg.Dtos.Character;
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
    }
  }
}