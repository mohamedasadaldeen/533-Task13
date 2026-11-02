using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cairo_Cinema.Migrations
{
    /// <inheritdoc />
    public partial class insert_data_to_actors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Actors (Name, image) VALUES ('Tom Hardy', 'a.jpg'), ('Emma Stone', 'b.jpg'), ('Chris Evans', 'e.jpg'), ('Sarah Miller', 'd.jpg'), ('Daniel Smith', 'c.jpg');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from actors");
        }
    }
}
