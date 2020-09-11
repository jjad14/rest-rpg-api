using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rpg_api.Dtos.Fight;
using rpg_api.Services.FightService;

namespace rpg_api.Controllers
{
    // localhost:PORT/fight
    [ApiController]
    [Route("[controller]")]
    public class FightController : ControllerBase
    {
        private readonly IFightService _fightService;
        public FightController(IFightService fightService)
        {
            _fightService = fightService;

        }
        // POST: localhost:PORT/fight/weapon
        [HttpPost("Weapon")]
        public async Task<IActionResult> WeaponAttack(WeaponAttackDto request)
        {
            return Ok(await _fightService.WeaponAttack(request));
        }

        // POST: localhost:PORT/fight/skill
        [HttpPost("Skill")]
        public async Task<IActionResult> SkillAttack(SkillAttackDto request)
        {
            return Ok(await _fightService.SkillAttack(request));
        }

        // POST: localhost:PORT/fight/
        [HttpPost]
        public async Task<IActionResult> Fight(FightRequestDto request)
        {
            return Ok(await _fightService.Fight(request));
        }

        // GET: localhost:PORT/fight/
        public async Task<IActionResult> GetHighscore() {
            return Ok(await _fightService.GetHighscore());
        }

    }
}