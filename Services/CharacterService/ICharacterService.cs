using System.Collections.Generic;
using System.Threading.Tasks;
using rpg_api.Dtos.Character;
using rpg_api.Models;

namespace rpg_api.Services.CharacterService
{
    public interface ICharacterService
    {
         Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters();
         Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id);
         Task<ServiceResponse<List<GetCharacterDto>>> CreateCharacter(CreateCharacterDto newCharacter);
        Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter);
        Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id);
    }
}