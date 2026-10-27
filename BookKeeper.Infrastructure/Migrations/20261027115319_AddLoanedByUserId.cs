using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookKeeper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanedByUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLoaned",
                table: "Books");

            migrationBuilder.AddColumn<Guid>(
                name: "LoanedByUserId",
                table: "Books",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoanedByUserId",
                table: "Books");

            migrationBuilder.AddColumn<bool>(
                name: "IsLoaned",
                table: "Books",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
