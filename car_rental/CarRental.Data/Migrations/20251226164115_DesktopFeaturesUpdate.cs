using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Data.Migrations
{
    /// <inheritdoc />
    public partial class DesktopFeaturesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RenewalCycleMonths",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedReturnDate",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsClockedIn",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnLeave",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSick",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastClockIn",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastClockOut",
                table: "Employees",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenewalCycleMonths",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ExpectedReturnDate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsClockedIn",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsOnLeave",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsSick",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LastClockIn",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LastClockOut",
                table: "Employees");
        }
    }
}
