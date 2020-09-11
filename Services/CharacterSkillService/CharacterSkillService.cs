using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rpg_api.Data;
using rpg_api.Dtos.Character;
using rpg_api.Dtos.CharacterSkill;
using rpg_api.Models;

namespace rpg_api.Services.CharacterSkillService
{
    public class CharacterSkillService : ICharacterSkillService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _context;
        public CharacterSkillService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;

        }
        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            // wrapper
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();

            try 
            {
                // find character but first include the weapon, characterSkills and Skill entities
                // find character where id is equal to characterSkill id 
                // and where character user id is equal to id found in token
                Character character = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkills)
                    .ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId &&
                    c.User.Id == int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));

                // if no character is found
                if (character == null) {
                    response.Success = false;
                    response.Message = "Character not found";
                    return response;
                }

                // find skill where id is equal to new characterskill skill id
                Skill skill = await _context.Skills 
                    .FirstOrDefaultAsync(S => S.Id == newCharacterSkill.SkillId);

                // if no skill is found
                if (skill == null) {
                    response.Success = false;
                    response.Message = "Skill not found";
                    return response;
                }

                // create characterSkill obj
                CharacterSkill characterSkill = new CharacterSkill
                {
                    Character = character,
                    Skill = skill
                };

                // add characterskill to db and save changes
                await _context.CharacterSkills.AddAsync(characterSkill);
                await _context.SaveChangesAsync();

                // map character to GetCharacterDto
                response.Data = _mapper.Map<GetCharacterDto>(character);

            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = "Unable to add Character Skill: " + ex.Message;
            }
            return response;
        }
    }
}