﻿// <auto-generated />
using ActivityReceiver.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace ActivityReceiver.Migrations.ActivityReceiverDb
{
    [DbContext(typeof(ActivityReceiverDbContext))]
    partial class ActivityReceiverDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ActivityReceiver.Models.Answer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AssignmentRecordID");

                    b.Property<string>("Content");

                    b.Property<DateTime>("EndDate");

                    b.Property<int?>("HesitationDegree");

                    b.Property<bool>("IsCorrect");

                    b.Property<int>("QuestionID");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("ID");

                    b.ToTable("Answsers");
                });

            modelBuilder.Entity("ActivityReceiver.Models.AssignmentRecord", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CurrentQuestionIndex");

                    b.Property<DateTime?>("EndDate");

                    b.Property<int>("ExerciseID");

                    b.Property<float>("Grade");

                    b.Property<bool>("IsFinished");

                    b.Property<string>("Remark");

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("UserID");

                    b.HasKey("ID");

                    b.ToTable("AssignmentRecords");
                });

            modelBuilder.Entity("ActivityReceiver.Models.Exercise", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Description");

                    b.Property<string>("EditorID");

                    b.Property<int>("Level");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Exercises");
                });

            modelBuilder.Entity("ActivityReceiver.Models.ExerciseQuestion", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ExerciseID");

                    b.Property<int>("QuestionID");

                    b.Property<int>("SerialNumber");

                    b.HasKey("ID");

                    b.ToTable("ExerciseQuestionCollection");
                });

            modelBuilder.Entity("ActivityReceiver.Models.Movement", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AnswerID");

                    b.Property<int>("Index");

                    b.Property<int>("State");

                    b.Property<int>("Time");

                    b.Property<int>("XPosition");

                    b.Property<int>("YPosition");

                    b.HasKey("ID");

                    b.ToTable("Movements");
                });

            modelBuilder.Entity("ActivityReceiver.Models.Question", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Division");

                    b.Property<string>("EditorID");

                    b.Property<string>("Grammar");

                    b.Property<int>("Level");

                    b.Property<string>("Remark");

                    b.Property<string>("SentenceEN");

                    b.Property<string>("SentenceJP");

                    b.HasKey("ID");

                    b.ToTable("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}