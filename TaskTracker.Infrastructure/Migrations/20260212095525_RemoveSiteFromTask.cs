using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSiteFromTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_ProjectSites_SiteId",
                table: "ProjectTasks");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTasks_SiteId",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "ProjectTasks");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectSiteId",
                table: "ProjectTasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_ProjectSiteId",
                table: "ProjectTasks",
                column: "ProjectSiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_ProjectSites_ProjectSiteId",
                table: "ProjectTasks",
                column: "ProjectSiteId",
                principalTable: "ProjectSites",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_ProjectSites_ProjectSiteId",
                table: "ProjectTasks");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTasks_ProjectSiteId",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "ProjectSiteId",
                table: "ProjectTasks");

            migrationBuilder.AddColumn<Guid>(
                name: "SiteId",
                table: "ProjectTasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_SiteId",
                table: "ProjectTasks",
                column: "SiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_ProjectSites_SiteId",
                table: "ProjectTasks",
                column: "SiteId",
                principalTable: "ProjectSites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
