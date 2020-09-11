using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rpg_api.Data;
using rpg_api.Dtos.Character;
using rpg_api.Dtos.Weapon;
using rpg_api.Models;

namespace rpg_api.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public WeaponService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _context = context;

        }
        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            // wrapper
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();

            try 
            {
                // find character where the character id is equal to the newwepon character id
                // and where the user id of that character is equal to the userid in the token 
                Character character = await _context.Characters
                    .FirstOrDefaultAsync(u => u.Id == newWeapon.CharacterId && 
                    u.User.Id == int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));

                // if no user is found
                if (character == null) {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                // Create weapon obj
                Weapon weapon = new Weapon
                {
                    Name = newWeapon.Name,
                    Damage = newWeapon.Damage,
                    Character = character
                };

                // add weapon to db and save changes
                await _context.Weapons.AddAsync(weapon);
                await _context.SaveChangesAsync();

                // map character to GetCharacterDto
                response.Data = _mapper.Map<GetCharacterDto>(character);

            }
            catch(Exception ex) 
            {
                response.Success = false;
                response.Message = "Weapon could not be added: " + ex.Message;
            }

            return response;
        }
    }
}