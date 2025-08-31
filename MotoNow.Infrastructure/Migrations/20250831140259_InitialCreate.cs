using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoNow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "motonow");

            migrationBuilder.CreateTable(
                name: "delivery_drivers",
                schema: "motonow",
                columns: table => new
                {
                    identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    birth_date = table.Column<DateTime>(type: "date", nullable: false),
                    driver_license_image_url = table.Column<string>(type: "text", nullable: true),
                    driver_license_number = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    driver_license_class = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_delivery_drivers", x => x.identifier);
                    table.CheckConstraint("ck_delivery_driver_cnh_class", "driver_license_class IN ('A','B','AB')");
                    table.CheckConstraint("ck_delivery_driver_cnh_digits", "driver_license_number ~ '^[0-9]{11}$'");
                    table.CheckConstraint("ck_delivery_driver_cnpj_digits", "cnpj ~ '^[0-9]{14}$'");
                });

            migrationBuilder.CreateTable(
                name: "motorcycle_notifications",
                schema: "motonow",
                columns: table => new
                {
                    identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    plate = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    model = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    received_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_motorcycle_notifications", x => x.identifier);
                });

            migrationBuilder.CreateTable(
                name: "motorcycles",
                schema: "motonow",
                columns: table => new
                {
                    identifier = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    model = table.Column<string>(type: "text", nullable: false),
                    plate = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_motorcycles", x => x.identifier);
                });

            migrationBuilder.CreateTable(
                name: "rentals",
                schema: "motonow",
                columns: table => new
                {
                    identifier = table.Column<string>(type: "text", nullable: false),
                    delivery_driver_id = table.Column<string>(type: "character varying(50)", nullable: false),
                    motorcycle_id = table.Column<string>(type: "character varying(64)", nullable: false),
                    start_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expected_end_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    plan_days = table.Column<int>(type: "integer", nullable: false),
                    return_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    daily_rate = table.Column<int>(type: "integer", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rentals", x => x.identifier);
                    table.CheckConstraint("ck_rental_plan_days", "plan_days > 0");
                    table.ForeignKey(
                        name: "fk_rentals_delivery_drivers_delivery_driver_id",
                        column: x => x.delivery_driver_id,
                        principalSchema: "motonow",
                        principalTable: "delivery_drivers",
                        principalColumn: "identifier",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rentals_motorcycles_motorcycle_id",
                        column: x => x.motorcycle_id,
                        principalSchema: "motonow",
                        principalTable: "motorcycles",
                        principalColumn: "identifier",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_delivery_drivers_cnpj",
                schema: "motonow",
                table: "delivery_drivers",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_delivery_drivers_driver_license_number",
                schema: "motonow",
                table: "delivery_drivers",
                column: "driver_license_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_delivery_drivers_identifier",
                schema: "motonow",
                table: "delivery_drivers",
                column: "identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_motorcycle_notifications_plate",
                schema: "motonow",
                table: "motorcycle_notifications",
                column: "plate");

            migrationBuilder.CreateIndex(
                name: "ix_motorcycle_notifications_year",
                schema: "motonow",
                table: "motorcycle_notifications",
                column: "year");

            migrationBuilder.CreateIndex(
                name: "ix_motorcycles_plate",
                schema: "motonow",
                table: "motorcycles",
                column: "plate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_rentals_delivery_driver_id_motorcycle_id_start_at_return_da",
                schema: "motonow",
                table: "rentals",
                columns: new[] { "delivery_driver_id", "motorcycle_id", "start_at", "return_date" });

            migrationBuilder.CreateIndex(
                name: "ix_rentals_motorcycle_id",
                schema: "motonow",
                table: "rentals",
                column: "motorcycle_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "motorcycle_notifications",
                schema: "motonow");

            migrationBuilder.DropTable(
                name: "rentals",
                schema: "motonow");

            migrationBuilder.DropTable(
                name: "delivery_drivers",
                schema: "motonow");

            migrationBuilder.DropTable(
                name: "motorcycles",
                schema: "motonow");
        }
    }
}
