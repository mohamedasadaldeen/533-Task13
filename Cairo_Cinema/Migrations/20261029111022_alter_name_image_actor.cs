using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cairo_Cinema.Migrations
{
    /// <inheritdoc />
    public partial class alter_name_image_actor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "image",
                table: "Actors",
                newName: "Image");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Actors",
                newName: "image");
        }
    }
}
