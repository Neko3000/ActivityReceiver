using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ActivityReceiver.Migrations.ActivityReceiverDb
{
    public partial class AddExerciseAndAssignmentRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswserRecords");

            migrationBuilder.RenameColumn(
                name: "AnswerRecordID",
                table: "Movements",
                newName: "AnswerID");

            migrationBuilder.AlterColumn<string>(
                name: "EditorID",
                table: "Questions",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatDate",
                table: "Questions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Grammar",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignmentRecordID",
                table: "Answsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Answsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Answsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AssignmentRecords",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentQuestionIndex = table.Column<int>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ExerciseID = table.Column<int>(nullable: false),
                    Grade = table.Column<float>(nullable: false),
                    IsFinished = table.Column<bool>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentRecords", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseQuestionCollection",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExerciseID = table.Column<int>(nullable: false),
                    QuestionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseQuestionCollection", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EditorID = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentRecords");

            migrationBuilder.DropTable(
                name: "ExerciseQuestionCollection");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropColumn(
                name: "CreatDate",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Grammar",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AssignmentRecordID",
                table: "Answsers");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Answsers");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Answsers");

            migrationBuilder.RenameColumn(
                name: "AnswerID",
                table: "Movements",
                newName: "AnswerRecordID");

            migrationBuilder.AlterColumn<int>(
                name: "EditorID",
                table: "Questions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

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
        }
    }
}
