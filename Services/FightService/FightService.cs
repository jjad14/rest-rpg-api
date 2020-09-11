using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using rpg_api.Data;
using rpg_api.Dtos.Fight;
using rpg_api.Models;

namespace rpg_api.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public FightService(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }

        // Weapon Attacks
        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            // wrapper
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();

            try
            {
                // get attacker by the request id, includes the weapon entity
                Character attacker = await _context.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                // get opponent by request id
                Character opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                // attack with weapon algorithm
                int damage = DoWeaponAttack(attacker, opponent);

                // if health falls to/or below 0
                if (opponent.HitPoints <= 0)
                {
                    response.Message = $"You have defeated {opponent.Name} in combat!";
                }

                // update opponent stats and save changes
                _context.Characters.Update(opponent);
                await _context.SaveChangesAsync();

                // wrapper data consists of the attack/opponent Name, attacker/opponent health and damage inflicted
                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHp = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHp = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to execute attack: " + ex.Message;
            }
            return response;
        }

        // Skill attack
        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();

            try
            {
                // get attacker by request id, include character skills and skills entities
                Character attacker = await _context.Characters
                    .Include(c => c.CharacterSkills)
                    .ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                // get opponent by request id
                Character opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);
                // get attackers skill by the request id
                CharacterSkill characterSkill =
                    attacker.CharacterSkills.FirstOrDefault(cs => cs.Skill.Id == request.SkillId);

                // if attacker has no skills
                if (characterSkill == null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know that skill.";
                    return response;
                }
                // attack with skill algorithm
                int damage = DoSkillAttack(attacker, opponent, characterSkill);

                // if health falls to/or below 0
                if (opponent.HitPoints <= 0)
                {
                    response.Message = $"You have defeated {opponent.Name} in combat!";
                }

                // update opponent stats and save changes
                _context.Characters.Update(opponent);
                await _context.SaveChangesAsync();

                // wrapper data to send
                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHp = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHp = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to execute attack: " + ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            ServiceResponse<FightResultDto> response = new ServiceResponse<FightResultDto>
            {
                Data = new FightResultDto()
            };

            try
            {
                // get all combatants, include the weapon,character skills, and skills entities
                List<Character> characters = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkills)
                    .ThenInclude(cs => cs.Skill)
                    .Where(c => request.CharacterIds.Contains(c.Id))
                    .ToListAsync();

                bool defeated = false;

                while (!defeated)
                {
                    // each combatant takes their turn attacking
                    foreach (Character attacker in characters)
                    {
                        // get all the opponents of the attacker
                        List<Character> opponents = characters.Where(c => c.Id != attacker.Id).ToList();

                        // randomly select an opponent, pass opponent count to use as an index for the opponents list
                        Character opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        // attack with either weapon or skill - if 0 we use weapon else skill
                        bool useWeapon = new Random().Next(2) == 0;

                        if (useWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);
                        }
                        else
                        {
                            int randomSkill = new Random().Next(attacker.CharacterSkills.Count);
                            attackUsed = attacker.CharacterSkills[randomSkill].Skill.Name;
                            damage = DoSkillAttack(attacker, opponent, attacker.CharacterSkills[randomSkill]);
                        }

                        response.Data.Log.Add($"{attacker.Name} has attacked {opponent.Name} using {attackUsed} inflicting {(damage >= 0 ? damage : 0)} damage.");

                        if (opponent.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated! {attacker.Name} has {attacker.HitPoints} left.");
                            break;
                        }
                    }
                }
                // increment fight count and reset hp of all combatants
                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HitPoints = 100;
                });

                // update characters and save changes
                _context.Characters.UpdateRange(characters);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Fighting execution failed: " + ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<HighscoreDto>>> GetHighscore()
        {
            // get all characters whos fight property is greater then 0
            // order in decending order by victories, and by defeats
            List<Character> characters = await _context.Characters
                .Where(c => c.Fights > 0)
                .OrderByDescending(c => c.Victories)
                .ThenBy(c => c.Defeats)
                .ToListAsync();

            // wrapper data, map Character to HighscoreDto
            var response = new ServiceResponse<List<HighscoreDto>>
            {
                Data = characters.Select(c => _mapper.Map<HighscoreDto>(c)).ToList()
            };

            return response;
        }

        private static int DoWeaponAttack(Character attacker, Character opponent)
        {
            
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }

        private static int DoSkillAttack(Character attacker, Character opponent, CharacterSkill characterSkill)
        {
            
            int damage = characterSkill.Skill.Damage + (new Random().Next(attacker.Intelligence));
            
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }


    }
}