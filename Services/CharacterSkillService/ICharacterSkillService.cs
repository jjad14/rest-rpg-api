using System.Threading.Tasks;
using rpg_api.Dtos.Character;
using rpg_api.Dtos.CharacterSkill;
using rpg_api.Models;

namespace rpg_api.Services.CharacterSkillService
{
    public interface ICharacterSkillService
    {
        Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill);
    }
}