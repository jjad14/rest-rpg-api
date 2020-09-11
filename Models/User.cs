using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace rpg_api.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [StringLength(12, MinimumLength=4)]
        public string Username { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        public List<Character> Characters { get; set; }
        [Required]
        [Range(1, 2)]
        public int Role { get; set; }
    }
}