using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cairo_Cinema.Migrations
{
    /// <inheritdoc />
    public partial class insert_data_to_movies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Movies (Name, Description, Price, Status, DateTime, MainImage, ActorId, CategoryId, CinemaId) VALUES ('John Wick', 'A retired hitman returns to seek revenge for his beloved dog.', 200, 1, '2026-11-01T19:00:00', '1.jpg', 1, 1, 1), ('The Green Mile', 'A prison guard discovers extraordinary events on death row.', 275, 1, '2026-11-02T19:00:00', '2.jpg', 2, 2, 2), ('The Mask', 'A shy bank clerk discovers a magical mask that changes his life.', 350, 1, '2026-11-03T19:00:00', '3.jpg', 3, 3, 3), ('The Conjuring', 'A family faces terrifying supernatural events inside their new home.', 425, 0, '2026-11-04T19:00:00', '4.jpg', 4, 4, 4), ('The Notebook', 'Two young lovers struggle to stay together despite many obstacles.', 500, 0, '2026-11-05T19:00:00', '5.jpg', 5, 5, 5);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from movies");

        }
    }
}
