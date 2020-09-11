using System.ComponentModel.DataAnnotations;

namespace rpg_api.Models
{
    public class CharacterSkill
    {
        public int CharacterId { get; set; }
        [Required]
        public Character Character { get; set; }
        public int SkillId { get; set; }
        [Required]
        public Skill Skill { get; set; }
    }
}