using System.Threading.Tasks;
using rpg_api.Dtos.Character;
using rpg_api.Dtos.Weapon;
using rpg_api.Models;

namespace rpg_api.Services.WeaponService
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
    }
}