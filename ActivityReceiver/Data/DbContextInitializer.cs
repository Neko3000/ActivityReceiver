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
        void Initialize();
    }
    public class DbContextInitializer:IDbContextInitializer
    {
        ApplicationDbContext _appDbContext;
        ActivityReceiverDbContext _arDbContext;
        UserManager<ApplicationUser> _userManager;
        RoleManager<IdentityRole> _roleManager;

        public DbContextInitializer(ApplicationDbContext appDbContext, ActivityReceiverDbContext arDbCotnext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _appDbContext = appDbContext;
            _arDbContext = arDbCotnext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            ApplicationDbContextInitialize();
            AcitvityReceiverDbCotextInitialize();
        }

        public void ApplicationDbContextInitialize()
        {
            _appDbContext.Database.EnsureCreated();

            if(_appDbContext.Users.Any())
            {
                return;
            }
        }

        public void AcitvityReceiverDbCotextInitialize()
        {
            _arDbContext.Database.EnsureCreated();
            
            if(_arDbContext.Movements.Any())
            {
                return;
            }

            var questions = new List<Question>()
            {
                new Question()
                {
                    EditorID = 1,
                    SentenceEN = "There are many ways to solve this problem.",
                    SentenceJP = "この問題を解決する方法はたくさんあります。",
                    Level = 2,
                    Division = "are|many|problem|solve|there|this|to|ways",
                    Remark = "",
                },
                new Question()
                {
                    EditorID = 1,
                    SentenceEN = "you may have heard this joke before.",
                    SentenceJP = "その冗談は前に聞いたことがあるかもしれませんね。",
                    Level = 5,
                    Division = "before|have|heard|joke|may|this|you",
                    Remark = "",
                }
            };
            _arDbContext.Questions.AddRange(questions);
            _arDbContext.SaveChanges();

            var answers = new List<Answer>()
            {
                new Answer()
                {
                    Content = "are|many|problem|solve|there|this|to|ways",
                    HesitationDegree = 1
                },
                new Answer()
                {
                    Content = "you|before|have|heard|joke|may|this",
                    HesitationDegree = 2
                }
            };
            _arDbContext.Answsers.AddRange(answers);
            _arDbContext.SaveChanges();

            var answerRecords = new List<AnswerRecord>()
            {
                new AnswerRecord()
                {
                    QusetionID = 1,
                    UserID = 2,
                    AnswserID = 1,
                    StartDate = new DateTime(2018,1,2,16,0,0),
                    EndDate = new DateTime(2018,1,2,16,0,20)
                },
                new AnswerRecord()
                {
                    QusetionID = 2,
                    UserID = 2,
                    AnswserID = 2,
                    StartDate = new DateTime(2018,1,2,16,0,30),
                    EndDate = new DateTime(2018,1,2,16,0,55)
                },
            };
            _arDbContext.AnswserRecords.AddRange(answerRecords);
            _arDbContext.SaveChanges();

            var movements = new List<Movement>()
            {
                new Movement()
                {
                    AnswerRecordID = 1,
                    Index = 0,
                    State = 0,
                    Time = 1000,
                    XPosition = 200,
                    YPostion = 300,
                    IsFinished = false,

                },
                new Movement()
                {
                    AnswerRecordID = 1,
                    Index = 1,
                    State = 1,
                    Time = 2500,
                    XPosition = 100,
                    YPostion = 100,
                    IsFinished = false,
                },
                new Movement()
                {
                    AnswerRecordID = 1,
                    Index = 2,
                    State = 2,
                    Time = 5000,
                    XPosition = 100,
                    YPostion = 100,
                    IsFinished = true,
                }
            };
            _arDbContext.Movements.AddRange(movements);
            _arDbContext.SaveChanges();
        }
    }
}
