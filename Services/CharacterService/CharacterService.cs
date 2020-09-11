using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rpg_api.Data;
using rpg_api.Dtos.Character;
using rpg_api.Models;

namespace rpg_api.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
            _context = context;
            _mapper = mapper;
        }

        // Access token and get userId
        private int GetUserId() => int.Parse(_httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        // Access token and get user Role
        private string GetUserRole() => _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.Role);

        public async Task<ServiceResponse<List<GetCharacterDto>>> CreateCharacter(CreateCharacterDto newCharacter)
        {
            // Wrapper
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                // map CreateCharacterDto to character 
                Character character = _mapper.Map<Character>(newCharacter);
                // set character property User to user with the userId from the IHttpContext 
                character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
                // add character to database and save changes
                await _context.Characters.AddAsync(character);
                await _context.SaveChangesAsync();
                // after adding new character to db, return all characters from db
                serviceResponse.Data = (_context.Characters.Where(u => u.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
                serviceResponse.Message = "Character has been created.";
            }
            catch (Exception ex)
            {
                // creation failed
                serviceResponse.Success = false;
                serviceResponse.Message = "Character could not be created: " + ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            // wrapper
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                // get character from database that matches both id params
                Character character = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());

                if (character != null) {
                    // remove character from Characters table
                    _context.Characters.Remove(character);

                    // save changes
                    await _context.SaveChangesAsync();

                    // return list of characters, map Character to GetCharacterDto
                    serviceResponse.Data = (_context.Characters.Where(c => c.User.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
                    serviceResponse.Message = "Character has been deleted.";
                }
                else 
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character was not deleted.";
                }

            }
            catch (Exception ex)
            {
                // deletion failes
                serviceResponse.Success = false;
                serviceResponse.Message = "Character deletion failed: " + ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            // wrapper
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            // return all characters that match userId
            List<Character> dbCharacters = 
                GetUserRole().Equals("2") ?
                await _context.Characters.ToListAsync() :
                await _context.Characters.Where(c => c.User.Id == GetUserId()).ToListAsync();
            // map Character to getCharacterDTo
            serviceResponse.Data = (dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c))).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            // wrapper
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();
            // get character that matches id
            Character dbCharacter = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.CharacterSkills)
                .ThenInclude(cs => cs.Skill)
                .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
            // map character to GetCharacterDto
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            // wrapper
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();

            try
            {
                // find character in database by id
                Character character = await _context.Characters.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                if (character.User.Id == GetUserId()) {
                    // overwrite all character properties
                    character.Name = updatedCharacter.Name;
                    character.Class = updatedCharacter.Class;
                    character.HitPoints = updatedCharacter.HitPoints;
                    character.Mana = updatedCharacter.Mana;
                    character.Strength = updatedCharacter.Strength;
                    character.Defense = updatedCharacter.Defense;
                    character.Perception = updatedCharacter.Perception;
                    character.Endurance = updatedCharacter.Endurance;
                    character.Charisma = updatedCharacter.Charisma;
                    character.Intelligence = updatedCharacter.Intelligence;
                    character.Agility = updatedCharacter.Agility;
                    character.Luck = updatedCharacter.Luck;

                    // update character
                    _context.Characters.Update(character);

                    // save changes
                    await _context.SaveChangesAsync();

                    // map Character obj to GetCharacterDto
                    serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
                    serviceResponse.Message = "Character has been updated.";
                }
                else {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character was not updated.";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Character update failed: " + ex.Message;
            }

            return serviceResponse;
        }
    }
}