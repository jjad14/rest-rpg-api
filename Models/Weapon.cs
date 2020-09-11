using System.ComponentModel.DataAnnotations;

namespace rpg_api.Models
{
    public class Weapon
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Damage { get; set; }
        public Character Character { get; set; }
        public int CharacterId { get; set; }
    }
}