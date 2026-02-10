using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwiftApp.Postal.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class FixDataTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "width_cm",
                table: "parcels",
                type: "numeric(10,3)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "weight_kg",
                table: "parcels",
                type: "numeric(10,3)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "length_cm",
                table: "parcels",
                type: "numeric(10,3)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "height_cm",
                table: "parcels",
                type: "numeric(10,3)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "longitude",
                table: "branches",
                type: "numeric(10,7)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "latitude",
                table: "branches",
                type: "numeric(10,7)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "width_cm",
                table: "parcels",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "weight_kg",
                table: "parcels",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "length_cm",
                table: "parcels",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "height_cm",
                table: "parcels",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "longitude",
                table: "branches",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,7)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "latitude",
                table: "branches",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,7)",
                oldNullable: true);
        }
    }
}
