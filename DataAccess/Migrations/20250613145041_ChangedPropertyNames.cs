using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPropertyNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SignatureSalt",
                table: "Users",
                newName: "SignatureSaltBase64");

            migrationBuilder.RenameColumn(
                name: "PublicKey",
                table: "Users",
                newName: "PublicKeyBase64");

            migrationBuilder.RenameColumn(
                name: "EncryptionSalt",
                table: "Users",
                newName: "EncryptionSaltBase64");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Notes",
                newName: "EncryptedTitleBase64");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Notes",
                newName: "EncryptedContentBase64");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SignatureSaltBase64",
                table: "Users",
                newName: "SignatureSalt");

            migrationBuilder.RenameColumn(
                name: "PublicKeyBase64",
                table: "Users",
                newName: "PublicKey");

            migrationBuilder.RenameColumn(
                name: "EncryptionSaltBase64",
                table: "Users",
                newName: "EncryptionSalt");

            migrationBuilder.RenameColumn(
                name: "EncryptedTitleBase64",
                table: "Notes",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "EncryptedContentBase64",
                table: "Notes",
                newName: "Content");
        }
    }
}
