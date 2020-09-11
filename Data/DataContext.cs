using Microsoft.EntityFrameworkCore;
using rpg_api.Models;

namespace rpg_api.Data
{
    // instance of the DbContext represents a session with the database. 
    // This means we can use this instance to query the database and save all the changes to 
    // our RPG characters. The description also says that the DB context is a combination of 
    // the unit of work and the repository pattern.
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // Whenever you want to see a representation of your model in the database. 
        // You have to add a database set of this model that's how entity framework knows what 
        // tables it should create.
        // The name of the DbSet will be the name of the corresponding database table
        public DbSet<Character> Characters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<CharacterSkill> CharacterSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            // CharacterSkills is the result of joining Character id and Skill id entities
            // Character id and Skill id will act as a composite key
            modelBuilder.Entity<CharacterSkill>()
                .HasKey(cs => new { cs.CharacterId, cs.SkillId});

            // Set default value for User.Role to Player
            modelBuilder.Entity<User>()
                .Property(user => user.Role).HasDefaultValue(1);
        }
    }
}