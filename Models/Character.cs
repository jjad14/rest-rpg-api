using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace rpg_api.Models
{
    public class Character
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "Unknown Hero";
        [Required]
        [Range(100, 1000)]
        public int HitPoints { get; set; } = 100;
        [Required]
        [Range(100, 1000)]
        public int Mana { get; set; } = 100;
        [Required]
        [Range(1, 10)]
        public int Strength { get; set; } = 10;
        [Required]
        [Range(1, 10)]
        public int Defense { get; set; } = 10;
        [Required]
        [Range(1, 10)]
        public int Perception { get; set; } = 10;
        [Required]
        [Range(1, 10)]
        public int Endurance { get; set; } = 10;
        [Required]
        [Range(1, 10)]
        public int Charisma { get; set; } = 10;
        [Required]
        [Range(1, 10)]
        public int Intelligence { get; set; } = 10;
        [Required]
        [Range(1, 10)]
        public int Agility { get; set; } = 10;
        [Required]
        [Range(1, 10)]
        public int Luck { get; set; } = 10;
        public RpgClass Class { get; set; } = RpgClass.Knight;
        [Required]
        public User User { get; set; }
        public Weapon Weapon { get; set; }
        public List<CharacterSkill> CharacterSkills { get; set; }
        [Required]
        public int Fights { get; set; } = 0;
        [Required]
        public int Victories { get; set; } = 0;
        [Required]
        public int Defeats { get; set; } = 0;
    }
}