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

            var grammars = new List<Grammar>()
            {
                new Grammar()
                {
                    Name = "仮定法，命令法"
                },
                new Grammar()
                {
                    Name = "It，There"
                },
                new Grammar()
                {
                    Name = "無生物主語"
                },
                new Grammar()
                {
                    Name = "接続詞"
                },
                new Grammar()
                {
                    Name = "接続詞"
                },
                new Grammar()
                {
                    Name = "関係詞"
                },
                new Grammar()
                {
                    Name = "間接話法"
                },
                new Grammar()
                {
                    Name = "前置詞(句)"
                },
                new Grammar()
                {
                    Name = "分詞"
                },
                new Grammar()
                {
                    Name = "動名詞"
                },
                new Grammar()
                {
                    Name = "不定詞"
                },
                new Grammar()
                {
                    Name = "受動態"
                },
                new Grammar()
                {
                    Name = "助動詞"
                },
                new Grammar()
                {
                    Name = "比較"
                },
                new Grammar()
                {
                    Name = "否定"
                },
                new Grammar()
                {
                    Name = "後置修飾"
                },
                new Grammar()
                {
                    Name = "完了形、時制"
                },
                new Grammar()
                {
                    Name = "句動詞(群動詞)"
                },
                new Grammar()
                {
                    Name = "挿入"
                },
                new Grammar()
                {
                    Name = "使役"
                },
                new Grammar()
                {
                    Name = "補語/二重目的語"
                },
            };
            for(int i = 0; i < grammars.Count; i++)
            {
                _arDbContext.Grammars.Add(grammars[i]);
                _arDbContext.SaveChanges();
            }

            var questions = new List<Question>()
            {
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "The spread of personal computers enables us to enjoy global communication.",
                    SentenceJP = "パソコンが普及したおかげで、地球規模でのコミュニケーションを楽しむことができる。",
                    Level = 1,
                    Division = "computers|enables|enjoy|of|personal|spread|the|to|us|global communication",
                    AnswerDivision = "the|spread|of|personal|computers|enables|us|to|enjoy|global communication",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,30,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "I would have somebody sweep this room clean.",
                    SentenceJP = "誰かにこの部屋をきれいに掃除してもらいたい。",
                    Level = 1,
                    Division = "clean|have|I|room|somebody|sweep|this|would",
                    AnswerDivision = "I|would|have|somebody|sweep|this|room|clean",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,30,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "I'll have you all speaking fluent English within a year.",
                    SentenceJP = "1年以内にあなた方がみな、流暢な英語を話しているようにしてあげます。",
                    Level = 1,
                    Division = "a|all|English|fluent|have|I'll|speaking|within|year|you",
                    AnswerDivision = "I'll|have|you|all|speaking|fluent|English|within|a|year",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,30,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "This picture reminds me of the good old days.",
                    SentenceJP = "この写真を見ると楽しかった昔のことを思い出す。",
                    Level = 1,
                    Division = "days|good|me|of|old|picture|reminds|the|this",
                    AnswerDivision = "this|picture|reminds|me|of|the|good|old|days",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,30,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "Seeing me , they suddently stopped talking.",
                    SentenceJP = "僕の姿を見ると、彼らは急に話をするのをやめた。",
                    Level = 1,
                    Division = ",|me|seeing|stopped|suddently|talking|they",
                    AnswerDivision = "seeing|me|,|they|suddently|stopped|talking",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,30,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "A slip of the tongue often brings about unexpected results.",
                    SentenceJP = "うっかり口をすべらせると思わぬ結果を招くことが多い。",
                    Level = 2,
                    Division = "a|about|brings|of|often|results|slip|the|tongue|unexpected",
                    AnswerDivision = "a|slip|of|the|tongue|often|brings|about|unexpected|results",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,40,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "Nothing was to be heard except the sound of the waves.",
                    SentenceJP = "波の音のほかは何一つ聞こえなかった。",
                    Level = 2,
                    Division = "be|except|heard|nothing|of|sound|the|the|to|was|waves",
                    AnswerDivision = "nothing|was|to|be|heard|except|the|sound|of|the|waves",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,40,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "He behaved himself so as not to give offence to others.",
                    SentenceJP = "彼は他人の感情を害さないように振舞った。",
                    Level = 2,
                    Division = "as|behaved|give|he|himself|not|offence|others|so|to|to",
                    AnswerDivision = "he|behaved|himself|so|as|not|to|give|offence|to|others",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,40,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "Bad books do us as much harm as bad friends.",
                    SentenceJP = "悪書は悪友と同じように私たちに害を与えるものだ。",
                    Level = 2,
                    Division = "a|beauty|by|good|is|means|no|of|personality|sign",
                    AnswerDivision = "bad|books|do|us|as|much|harm|as|bad|friends",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,40,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "Beauty is by no means a sign of good personality.",
                    SentenceJP = "美しいということが善良な人である印になることなどは絶対にない。",
                    Level = 2,
                    Division = "as|behaved|give|he|himself|not|offence|others|so|to|to",
                    AnswerDivision = "beauty|is|by|no|means|a|sign|of|good|personality",
                    GrammarIDString = "#8#15#",
                    CreateDate = new DateTime(2015,7,1,12,40,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser2.Id,
                    SentenceEN = "You cannot be too busy to come to see us now and then.",
                    SentenceJP = "時折会いに来ることができないほど忙しいということはあるまい。",
                    Level = 3,
                    Division = "be|busy|cannot|come|see|to|to|too|us|you|now and then",
                    AnswerDivision = "you|cannot|be|too|busy|to|come|to|see|us|now|and|then",
                    GrammarIDString = "#11#13#15#",
                    CreateDate = new DateTime(2015,7,1,12,50,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser2.Id,
                    SentenceEN = "A true friend would have done otherwise.",
                    SentenceJP = "本当の友人であったなら違った行動をとっていただろうに。",
                    Level = 3,
                    Division = "a|done|friend|have|otherwise|true|would",
                    AnswerDivision = "a|true|friend|would|have|done|otherwise",
                    GrammarIDString = "#8#15#",
                    CreateDate = new DateTime(2015,7,1,12,50,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser2.Id,
                    SentenceEN = "We felt happy as if we were still dreaming.",
                    SentenceJP = "私たちはまだ夢をみているような幸せな気分だった。",
                    Level = 3,
                    Division = "as|dreaming|felt|happy|if|still|we|we|were",
                    AnswerDivision = "we|felt|happy|as|if|we|were|still|dreaming",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,50,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser2.Id,
                    SentenceEN = "Judging from the sky , it looks like snow this afternoon.",
                    SentenceJP = "空模様から察すると、今日の午後は雪になりそうだ。。",
                    Level = 3,
                    Division = ",|afternoon|from|it|judging|like|looks|sky|snow|the|this",
                    AnswerDivision = "judging|from|the|sky|,|it|looks|like|snow|this|afternoon",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,50,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser2.Id,
                    SentenceEN = "There were some stars seen in the sky last night.",
                    SentenceJP = "昨夜は空に星が出ていた。",
                    Level = 3,
                    Division = "in|seen|sky|some|stars|the|there|were|last night",
                    AnswerDivision = "there|were|some|stars|seen|in|the|sky|last night",
                    GrammarIDString = "#2#9#",
                    CreateDate = new DateTime(2015,7,1,12,50,0),
                    Remark = "",
                },
            };
            _arDbContext.Questions.AddRange(questions);
            _arDbContext.SaveChanges();

            var exercises = new List<Exercise>()
            {
                new Exercise
                {
                    Name = "初心者ための問題集",
                    Description = "短い5問をやってみよう",
                    Level = 1,
                    CreateDate = DateTime.Now,
                    EditorID = applicationUser1.Id
                },
                new Exercise
                {
                    Name = "入門！英語学習者！",
                    Description = "「初心者ための問題集」を含めた計10問で自分の英語能力を検定する",
                    Level = 2,
                    CreateDate = DateTime.Now,
                    EditorID = applicationUser2.Id
                },
                new Exercise
                {
                    Name = "伝説の道",
                    Description = "基礎の問題で腕を磨いた君が、伝説なヒーローになれるか",
                    Level = 3,
                    CreateDate = DateTime.Now,
                    EditorID = applicationUser2.Id
                },
            };
            _arDbContext.Exercises.AddRange(exercises);
            _arDbContext.SaveChanges();

            for(int i = 0;i<exercises.Count;i++)
            {
                // Exercise 1
                if(exercises[i].Level == 1)
                {
                    for(int j = 0; j< questions.Count; j++)
                    {
                        if(questions[j].Level == 1)
                        {
                            var exerciseQuestion = new ExerciseQuestion()
                            {
                                ExerciseID = exercises[i].ID,
                                QuestionID = questions[j].ID,
                                SerialNumber = j
                            };

                            _arDbContext.ExerciseQuestionCollection.Add(exerciseQuestion);
                            _arDbContext.SaveChanges();
                        }
                    }
                }

                // Exercise 2
                if (exercises[i].Level == 2)
                {
                    for (int j = 0; j < questions.Count; j++)
                    {
                        if (questions[j].Level == 1 || questions[j].Level == 2)
                        {
                            var exerciseQuestion = new ExerciseQuestion()
                            {
                                ExerciseID = exercises[i].ID,
                                QuestionID = questions[j].ID,
                                SerialNumber = j
                            };

                            _arDbContext.ExerciseQuestionCollection.Add(exerciseQuestion);
                            _arDbContext.SaveChanges();
                        }
                    }
                }

                // Exercise 1
                if (exercises[i].Level == 3)
                {
                    for (int j = 0; j < questions.Count; j++)
                    {
                        if (questions[j].Level == 1 || questions[j].Level == 2 || questions[j].Level == 3)
                        {
                            var exerciseQuestion = new ExerciseQuestion()
                            {
                                ExerciseID = exercises[i].ID,
                                QuestionID = questions[j].ID,
                                SerialNumber = j
                            };

                            _arDbContext.ExerciseQuestionCollection.Add(exerciseQuestion);
                            _arDbContext.SaveChanges();
                        }
                    }
                }

            }

        }
    }
}
