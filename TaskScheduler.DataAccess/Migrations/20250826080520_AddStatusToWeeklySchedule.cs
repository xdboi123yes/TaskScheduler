using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskScheduler.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToWeeklySchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScheduleName",
                table: "WeeklySchedules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "WeeklySchedules",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduleName",
                table: "WeeklySchedules");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "WeeklySchedules");
        }
    }
}
