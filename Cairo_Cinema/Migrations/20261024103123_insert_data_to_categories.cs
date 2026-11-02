using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cairo_Cinema.Migrations
{
    /// <inheritdoc />
    public partial class insert_data_to_categories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Categories (Name, Description, Status) VALUES ('Action', 'Exciting stories filled with fights and adventure.', 1), ('Drama', 'Emotional stories about life and loss.', 1), ('Comedy', 'Funny stories full of laughter and joy.', 1), ('Horror', 'Terrifying stories that create fear and suspense.', 0), ('Romance', 'Beautiful stories about love and relationships.', 0);");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from categories");

        }
    }
}
