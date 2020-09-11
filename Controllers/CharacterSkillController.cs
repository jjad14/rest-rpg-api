using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg_api.Dtos.CharacterSkill;
using rpg_api.Services.CharacterSkillService;

namespace rpg_api.Controllers
{
    // localhost:PORT/characterskill
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CharacterSkillController : ControllerBase
    {
        private readonly ICharacterSkillService _characterSkillService;
        public CharacterSkillController(ICharacterSkillService characterSkillService)
        {
            _characterSkillService = characterSkillService;

        }

        // POST: localhost:PORT/characterskill
        [HttpPost]
        public async Task<IActionResult> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill) 
        {
            return Ok(await _characterSkillService.AddCharacterSkill(newCharacterSkill));
        }
    }
}