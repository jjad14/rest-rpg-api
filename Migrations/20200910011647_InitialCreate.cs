using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace rpg_api.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    HitPoints = table.Column<int>(nullable: false),
                    Mana = table.Column<int>(nullable: false),
                    Strength = table.Column<int>(nullable: false),
                    Defense = table.Column<int>(nullable: false),
                    Perception = table.Column<int>(nullable: false),
                    Endurance = table.Column<int>(nullable: false),
                    Charisma = table.Column<int>(nullable: false),
                    Intelligence = table.Column<int>(nullable: false),
                    Agility = table.Column<int>(nullable: false),
                    Luck = table.Column<int>(nullable: false),
                    Class = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
