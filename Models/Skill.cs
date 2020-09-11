using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace rpg_api.Models
{
    public class Skill
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Damage { get; set; }
        public List<CharacterSkill> CharacterSkills { get; set; }
    }
}