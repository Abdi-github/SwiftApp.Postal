using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftApp.Postal.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddKeycloakUserIdIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_employees_keycloak_user_id",
                table: "employees",
                column: "keycloak_user_id",
                unique: true,
                filter: "keycloak_user_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_customers_keycloak_user_id",
                table: "customers",
                column: "keycloak_user_id",
                unique: true,
                filter: "keycloak_user_id IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_employees_keycloak_user_id",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "IX_customers_keycloak_user_id",
                table: "customers");
        }
    }
}
