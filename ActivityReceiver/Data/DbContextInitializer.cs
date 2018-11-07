using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Models;
using Microsoft.AspNetCore.Identity;

namespace ActivityReceiver.Data
{
    public interface IDbContextInitializer
    {
        Task MainDbContextInitialize();
        Task ApplicationDbContextInitialize();
    }
    public class DbContextInitializer:IDbContextInitializer
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbContextInitializer(ApplicationDbContext appDbContext, ActivityReceiverDbContext arDbCotnext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _appDbContext = appDbContext;
            _arDbContext = arDbCotnext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task ApplicationDbContextInitialize()
        {
            _appDbContext.Database.EnsureCreated();

            if (!_appDbContext.Roles.Any())
            {
                var identityRole1 = new IdentityRole("SuperAdmin");
                var identityRole2 = new IdentityRole("Admin");
                var identityRole3 = new IdentityRole("Student");

                await _roleManager.CreateAsync(identityRole1);
                await _roleManager.CreateAsync(identityRole2);
                await _roleManager.CreateAsync(identityRole3);
            }

            if(!_appDbContext.Users.Any())
            {
                var applicationUser1 = new ApplicationUser
                {
                    UserName = "Dolores",
                };
                var applicationUserPWD1 = "d123456";
                await _userManager.CreateAsync(applicationUser1, applicationUserPWD1);
                await _userManager.AddToRoleAsync(applicationUser1, "SuperAdmin");


                var applicationUser2 = new ApplicationUser
                {
                    UserName = "alex",
                };
                var applicationUserPWD2 = "a123456";
                await _userManager.CreateAsync(applicationUser2, applicationUserPWD2);
                await _userManager.AddToRoleAsync(applicationUser2, "Admin");

                var applicationUser3 = new ApplicationUser
                {
                    UserName = "jackson",
                };
                var applicationUserPWD3 = "j123456";
                await _userManager.CreateAsync(applicationUser3, applicationUserPWD3);
                await _userManager.AddToRoleAsync(applicationUser3, "Student");
            }
        }

        public async Task MainDbContextInitialize()
        {
            _arDbContext.Database.EnsureCreated();
            
            if(_arDbContext.Questions.Any())
            {
                return;
            }

            var applicationUser1 = await _userManager.FindByNameAsync("Dolores");
            var applicationUser2 = await _userManager.FindByNameAsync("alex");
            var applicationUser3 = await _userManager.FindByNameAsync("jackson");

            var questions = new List<Question>()
            {
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "There are many ways to solve this problem.",
                    SentenceJP = "この問題を解決する方法はたくさんあります。",
                    Level = 2,
                    Division = "are|many|problem|solve|there|this|to|ways",
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "You may have heard this joke before.",
                    SentenceJP = "その冗談は前に聞いたことがあるかもしれませんね。",
                    Level = 5,
                    Division = "before|have|heard|joke|may|this|you",
                    Remark = "",
                }
            };
            _arDbContext.Questions.AddRange(questions);
            _arDbContext.SaveChanges();

            var exercises = new List<Exercise>()
            {
                new Exercise
                {
                    Name = "For Beginners",
                    Description = "A short exercise for newcomer.",
                    Level = 1,
                    CreateDate = new DateTime(2015,7,1,12,30,0),
                    EditorID = applicationUser2.Id
                },
                new Exercise
                {
                    Name = "Verbs",
                    Description = "This exercise contains so many questions related verb.",
                    Level = 1,
                    CreateDate = new DateTime(2015,7,1,12,30,0),
                    EditorID = applicationUser2.Id
                },
                new Exercise
                {
                    Name = "For Heros",
                    Description = "A legendary exercise, only two people passed it.",
                    Level = 1,
                    CreateDate = new DateTime(2015,7,1,12,30,0),
                    EditorID = applicationUser2.Id
                },
            };
            _arDbContext.Exercises.AddRange(exercises);
            _arDbContext.SaveChanges();

            var exerciseQuestionCollection = new List<ExerciseQuestion>
            {
                new ExerciseQuestion()
                {
                    ExerciseID = exercises[0].ID,
                    QuestionID = questions[0].ID
                },
                new ExerciseQuestion()
                {
                    ExerciseID = exercises[1].ID,
                    QuestionID =questions[1].ID
                },
            };
            _arDbContext.ExerciseQuestionCollection.AddRange(exerciseQuestionCollection);
            _arDbContext.SaveChanges();

            var assignmentRecords = new List<AssignmentRecord>()
            {
                new AssignmentRecord()
                {
                    UserID = applicationUser3.Id,
                    ExerciseID = exercises[0].ID,
                    CurrentQuestionIndex = 0,
                    StartDate = new DateTime(2018,1,2,16,0,0),
                    EndDate = null,
                    IsFinished = false,
                    Grade = (float)0.0,
                    Remark = ""
                },
                new AssignmentRecord()
                {
                    UserID = applicationUser1.Id,
                    ExerciseID = exercises[2].ID,
                    CurrentQuestionIndex = 1,
                    StartDate = new DateTime(2018,1,2,16,0,0),
                    EndDate = new DateTime(2018,1,2,16,0,20),
                    IsFinished = true,
                    Grade = (float)100.0,
                    Remark = "it was difficult"
                },
            };
            _arDbContext.AssignmentRecords.AddRange(assignmentRecords);
            _arDbContext.SaveChanges();

            var answers = new List<Answer>()
            {
                new Answer()
                {
                    AssignmentRecordID = assignmentRecords[0].ID,
                    QuestionID = questions[0].ID,
                    Content = "there|are|many|ways|to|solve|this|problem",
                    HesitationDegree = 1,
                    IsCorrect = true,
                    StartDate = new DateTime(2015,7,1,13,20,10),
                    EndDate = new DateTime(2015,7,1,13,21,50),
                },
                new Answer()
                {
                    AssignmentRecordID = assignmentRecords[1].ID,
                    QuestionID = questions[1].ID,
                    Content = "you|before|have|heard|joke|may|this",
                    HesitationDegree = 2,
                    IsCorrect = false,
                    StartDate = new DateTime(2015,7,1,14,15,20),
                    EndDate = new DateTime(2015,7,1,14,17,30),
                }
            };
            _arDbContext.Answsers.AddRange(answers);
            _arDbContext.SaveChanges();
            var movements = new List<Movement>()
            {
                new Movement()
                {
                    AnswerID = 1,
                    Index = 0,
                    State = 0,
                    Time = 1000,
                    XPosition = 200,
                    YPosition = 300,

                },
                new Movement()
                {
                    AnswerID = 1,
                    Index = 1,
                    State = 1,
                    Time = 2500,
                    XPosition = 100,
                    YPosition = 100,
                },
                new Movement()
                {
                    AnswerID = 1,
                    Index = 2,
                    State = 2,
                    Time = 5000,
                    XPosition = 100,
                    YPosition = 100,
                }
            };
            _arDbContext.Movements.AddRange(movements);
            _arDbContext.SaveChanges();
        }
    }
}
