using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cairo_Cinema.Migrations
{
    /// <inheritdoc />
    public partial class insert_data_to_cinemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Cinemas (Name, Description, Status, Image) VALUES ('Cairo Cinema', 'The Best Cinema', 1, 'cairo.jpg'), ('Egypt Cinema', 'Modern Movie Experience', 1, 'egypt.jpg'), ('Zamalek Cinema', 'Premium Film Experience', 1, 'zamalek.jpg'), ('Rehab Cinema', 'Family Movie Destination', 1, 'rehab.jpg'), ('Madenty Cinema', 'Luxury Movie Experience', 1, 'madenty.jpg');");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from cinemas");

        }
    }
}
