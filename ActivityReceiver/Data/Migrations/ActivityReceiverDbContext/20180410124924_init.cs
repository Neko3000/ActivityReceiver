using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ActivityReceiver.Migrations.ActivityReceiverDb
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnswserRecords",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnswserID = table.Column<int>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    QusetionID = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswserRecords", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Answsers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(nullable: true),
                    HesitationDegree = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answsers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Movements",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnswerRecordID = table.Column<int>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    IsFinished = table.Column<bool>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    Time = table.Column<int>(nullable: false),
                    XPosition = table.Column<int>(nullable: false),
                    YPostion = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movements", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Division = table.Column<string>(nullable: true),
                    EditorID = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    SentenceEN = table.Column<string>(nullable: true),
                    SentenceJP = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswserRecords");

            migrationBuilder.DropTable(
                name: "Answsers");

            migrationBuilder.DropTable(
                name: "Movements");

            migrationBuilder.DropTable(
                name: "Questions");
        }
    }
}
