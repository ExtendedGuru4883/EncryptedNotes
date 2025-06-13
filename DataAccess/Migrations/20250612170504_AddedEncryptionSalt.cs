using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedEncryptionSalt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Salt",
                table: "Users",
                newName: "SignatureSalt");

            migrationBuilder.AddColumn<string>(
                name: "EncryptionSalt",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptionSalt",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "SignatureSalt",
                table: "Users",
                newName: "Salt");
        }
    }
}
