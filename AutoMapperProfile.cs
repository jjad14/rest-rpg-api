using System.Linq;
using AutoMapper;
using rpg_api.Dtos.Character;
using rpg_api.Dtos.Fight;
using rpg_api.Dtos.Skill;
using rpg_api.Dtos.Weapon;
using rpg_api.Models;

namespace rpg_api
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // means you want to map from Character to GetCharacterDto
            CreateMap<Character, GetCharacterDto>()
                .ForMember(dto => dto.Skills, c => c.MapFrom(c => c.CharacterSkills.Select(cs => cs.Skill)));
            // means you want to map from CreateCharacterDto to Character
            CreateMap<CreateCharacterDto, Character>();
            // means you want to map from Weapon to GetWeaponDto
            CreateMap<Weapon, GetWeaponDto>();
            // means you want to map from Skill to GetSkillDto
            CreateMap<Skill, GetSkillDto>();
            // means you want to map from Character to HighscoreDto
            CreateMap<Character, HighscoreDto>();
        }
    }
}