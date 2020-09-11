using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg_api.Dtos.Weapon;
using rpg_api.Services.WeaponService;

namespace rpg_api.Controllers
{
    // localhost:PORT/weapon
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeaponController : ControllerBase
    {
        private readonly IWeaponService _weaponService;
        public WeaponController(IWeaponService weaponService)
        {
            _weaponService = weaponService;
        }

        // POST: localhost:PORT/characterskill
        [HttpPost]
        public async Task<IActionResult> AddWeapon(AddWeaponDto newWeapon) 
        {
            return Ok(await _weaponService.AddWeapon(newWeapon));
        }
    }
}