using rpg_api.Models;

namespace rpg_api.Dtos.Character
{
    // Dto for creating a character
    public class CreateCharacterDto
    {
        public string Name { get; set; } = "Lone Wanderer";
        public int HitPoints { get; set; } = 100;
        public int Mana { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Perception { get; set; } = 10;
        public int Endurance { get; set; } = 10;
        public int Charisma { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Agility { get; set; } = 10;
        public int Luck { get; set; } = 10;
        public RpgClass Class { get; set; } = RpgClass.Knight;
    }
}