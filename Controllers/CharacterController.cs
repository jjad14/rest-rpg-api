using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg_api.Dtos.Character;
using rpg_api.Models;
using rpg_api.Services.CharacterService;

namespace rpg_api.Controllers
{
    // access controller on localhost:PORT/character
    [Authorize(Roles = "1,2")]
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        // Inject services via dependency injection
        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        // access controller method on GET localhost:PORT/character/getall
        // get all characters of a given userId
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetCharacters()
        {
            // pass Id to characterService get all Characters
            return Ok(await _characterService.GetAllCharacters());
        }

        // access controller method on GET localhost:PORT/character/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleCharacter(int id)
        {
            // get a single charcter by the id paramater
            return Ok(await _characterService.GetCharacterById(id));
        }

        // access controller method on POST localhost:PORT/character/
        [HttpPost]
        public async Task<IActionResult> CreateCharacter(CreateCharacterDto newCharacter)
        {
            // return Ok(await _characterService.CreateCharacter(newCharacter));

            // call createCharacter and wrap object with service response object to get additonal info (success/message)
            ServiceResponse<List<GetCharacterDto>> response = await _characterService.CreateCharacter(newCharacter);

            // if success is false then creating a character failed
            if (response.Success == false) {
                return Conflict(response);
            }
            // creation successful
            return Ok(response);

        }

        // access controller method on PUT localhost:PORT/character/
        [HttpPut]
        public async Task<IActionResult> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            // call UpdateCharacter and wrap object with service response object to get additonal info (success/message)
            ServiceResponse<GetCharacterDto> response = await _characterService.UpdateCharacter(updatedCharacter);

            // if success is false then updating a character failed
            if (response.Success == false) {
                return NotFound(response);
            }
            // update successful
            return Ok(response);
        }

        // access controller method on DELETE localhost:PORT/character/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            // call DeleteCharacter with id param and wrap object with service response object to get additonal info (success/message)
            ServiceResponse<List<GetCharacterDto>> response = await _characterService.DeleteCharacter(id);

            // deletion failed
            if (response.Success == false) {
                return NotFound(response);
            }
            // delete successful
            return Ok(response);
        }

    }

}