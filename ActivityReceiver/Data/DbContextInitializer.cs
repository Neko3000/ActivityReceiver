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
                    UserName = "Alex",
                };
                var applicationUserPWD2 = "a123456";
                await _userManager.CreateAsync(applicationUser2, applicationUserPWD2);
                await _userManager.AddToRoleAsync(applicationUser2, "Admin");

                var applicationUser3 = new ApplicationUser
                {
                    UserName = "Jackson",
                };
                var applicationUserPWD3 = "j123456";
                await _userManager.CreateAsync(applicationUser3, applicationUserPWD3);
                await _userManager.AddToRoleAsync(applicationUser3, "Student");

                var applicationUser4 = new ApplicationUser
                {
                    UserName = "Nakamura",
                };
                var applicationUserPWD4 = "n123456";
                await _userManager.CreateAsync(applicationUser4, applicationUserPWD4);
                await _userManager.AddToRoleAsync(applicationUser4, "Student");

                var applicationUser5 = new ApplicationUser
                {
                    UserName = "Inoue",
                };
                var applicationUserPWD5 = "i123456";
                await _userManager.CreateAsync(applicationUser5, applicationUserPWD5);
                await _userManager.AddToRoleAsync(applicationUser5, "Student");

                var applicationUser6 = new ApplicationUser
                {
                    UserName = "Tanaka",
                };
                var applicationUserPWD6 = "t123456";
                await _userManager.CreateAsync(applicationUser6, applicationUserPWD6);
                await _userManager.AddToRoleAsync(applicationUser6, "Student");

                var applicationUser7 = new ApplicationUser
                {
                    UserName = "Ikeda",
                };
                var applicationUserPWD7 = "i123456";
                await _userManager.CreateAsync(applicationUser7, applicationUserPWD7);
                await _userManager.AddToRoleAsync(applicationUser5, "Student");

                var applicationUser8 = new ApplicationUser
                {
                    UserName = "Banno",
                };
                var applicationUserPWD8 = "b123456";
                await _userManager.CreateAsync(applicationUser8, applicationUserPWD8);
                await _userManager.AddToRoleAsync(applicationUser5, "Student");

                var applicationUser9 = new ApplicationUser
                {
                    UserName = "Ando",
                };
                var applicationUserPWD9 = "a123456";
                await _userManager.CreateAsync(applicationUser9, applicationUserPWD9);
                await _userManager.AddToRoleAsync(applicationUser5, "Student");

                var applicationUser10 = new ApplicationUser
                {
                    UserName = "Waki",
                };
                var applicationUserPWD10 = "w123456";
                await _userManager.CreateAsync(applicationUser10, applicationUserPWD10);
                await _userManager.AddToRoleAsync(applicationUser5, "Student");

                var applicationUser11 = new ApplicationUser
                {
                    UserName = "Shirasu",
                };
                var applicationUserPWD11 = "s123456";
                await _userManager.CreateAsync(applicationUser11, applicationUserPWD11);
                await _userManager.AddToRoleAsync(applicationUser5, "Student");
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
            var applicationUser2 = await _userManager.FindByNameAsync("Alex");
            var applicationUser3 = await _userManager.FindByNameAsync("Jackson");

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
                    Name = "倒置"
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
                    StandardAnswerDivision = "the|spread|of|personal|computers|enables|us|to|enjoy|global communication",
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
                    StandardAnswerDivision = "I|would|have|somebody|sweep|this|room|clean",
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
                    StandardAnswerDivision = "I'll|have|you|all|speaking|fluent|English|within|a|year",
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
                    StandardAnswerDivision = "this|picture|reminds|me|of|the|good|old|days",
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
                    StandardAnswerDivision = "seeing|me|,|they|suddently|stopped|talking",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,30,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser2.Id,
                    SentenceEN = "A slip of the tongue often brings about unexpected results.",
                    SentenceJP = "うっかり口をすべらせると思わぬ結果を招くことが多い。",
                    Level = 2,
                    Division = "a|about|brings|of|often|results|slip|the|tongue|unexpected",
                    StandardAnswerDivision = "a|slip|of|the|tongue|often|brings|about|unexpected|results",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,40,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser2.Id,
                    SentenceEN = "Nothing was to be heard except the sound of the waves.",
                    SentenceJP = "波の音のほかは何一つ聞こえなかった。",
                    Level = 2,
                    Division = "be|except|heard|nothing|of|sound|the|the|to|was|waves",
                    StandardAnswerDivision = "nothing|was|to|be|heard|except|the|sound|of|the|waves",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,40,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser2.Id,
                    SentenceEN = "He behaved himself so as not to give offence to others.",
                    SentenceJP = "彼は他人の感情を害さないように振舞った。",
                    Level = 2,
                    Division = "as|behaved|give|he|himself|not|offence|others|so|to|to",
                    StandardAnswerDivision = "he|behaved|himself|so|as|not|to|give|offence|to|others",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,40,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser2.Id,
                    SentenceEN = "Bad books do us as much harm as bad friends.",
                    SentenceJP = "悪書は悪友と同じように私たちに害を与えるものだ。",
                    Level = 2,
                    Division = "a|beauty|by|good|is|means|no|of|personality|sign",
                    StandardAnswerDivision = "bad|books|do|us|as|much|harm|as|bad|friends",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,40,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser2.Id,
                    SentenceEN = "Beauty is by no means a sign of good personality.",
                    SentenceJP = "美しいということが善良な人である印になることなどは絶対にない。",
                    Level = 2,
                    Division = "as|behaved|give|he|himself|not|offence|others|so|to|to",
                    StandardAnswerDivision = "beauty|is|by|no|means|a|sign|of|good|personality",
                    GrammarIDString = "#8#15#",
                    CreateDate = new DateTime(2015,7,1,12,40,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser3.Id,
                    SentenceEN = "You cannot be too busy to come to see us now and then.",
                    SentenceJP = "時折会いに来ることができないほど忙しいということはあるまい。",
                    Level = 3,
                    Division = "be|busy|cannot|come|see|to|to|too|us|you|now and then",
                    StandardAnswerDivision = "you|cannot|be|too|busy|to|come|to|see|us|now|and|then",
                    GrammarIDString = "#11#13#15#",
                    CreateDate = new DateTime(2015,7,1,12,50,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser3.Id,
                    SentenceEN = "A true friend would have done otherwise.",
                    SentenceJP = "本当の友人であったなら違った行動をとっていただろうに。",
                    Level = 3,
                    Division = "a|done|friend|have|otherwise|true|would",
                    StandardAnswerDivision = "a|true|friend|would|have|done|otherwise",
                    GrammarIDString = "#8#15#",
                    CreateDate = new DateTime(2015,7,1,12,50,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser3.Id,
                    SentenceEN = "We felt happy as if we were still dreaming.",
                    SentenceJP = "私たちはまだ夢をみているような幸せな気分だった。",
                    Level = 3,
                    Division = "as|dreaming|felt|happy|if|still|we|we|were",
                    StandardAnswerDivision = "we|felt|happy|as|if|we|were|still|dreaming",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,50,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser3.Id,
                    SentenceEN = "Judging from the sky , it looks like snow this afternoon.",
                    SentenceJP = "空模様から察すると、今日の午後は雪になりそうだ。。",
                    Level = 3,
                    Division = ",|afternoon|from|it|judging|like|looks|sky|snow|the|this",
                    StandardAnswerDivision = "judging|from|the|sky|,|it|looks|like|snow|this|afternoon",
                    GrammarIDString = "-1",
                    CreateDate = new DateTime(2015,7,1,12,50,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser3.Id,
                    SentenceEN = "There were some stars seen in the sky last night.",
                    SentenceJP = "昨夜は空に星が出ていた。",
                    Level = 3,
                    Division = "in|seen|sky|some|stars|the|there|were|last night",
                    StandardAnswerDivision = "there|were|some|stars|seen|in|the|sky|last night",
                    GrammarIDString = "#2#9#",
                    CreateDate = new DateTime(2015,7,1,12,50,0),
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "The police came as quickly as possible.",
                    SentenceJP = "警察はできるだけ急いでやってきた。",
                    Level = 4,
                    Division = "as|as|came|police|possible|quickly|the",
                    StandardAnswerDivision = "the|police|came|as|quickly|as|possible",
                    GrammarIDString = "#14#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "My young sister does not sing as well as I.",
                    SentenceJP = "私の妹は私ほど歌がうまくない。",
                    Level = 4,
                    Division = "as|as|does|I|my|not|sing|sister|well|young",
                    StandardAnswerDivision = "my|young|sister|does|not|sing|as|well|as|I",
                    GrammarIDString = "#14#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "I love the Koiwai Coffee.",
                    SentenceJP = "私は小岩井珈琲が好きです。",
                    Level = 5,
                    Division = "coffee|I|Koiwai|love|the",
                    StandardAnswerDivision = "I|love|the|Koiwai|coffee",
                    GrammarIDString = "#8#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "His story does not sound true.",
                    SentenceJP = "彼の話は本当のようには思えない。",
                    Level = 5,
                    Division = "does|his|not|sound|story|true",
                    StandardAnswerDivision = "his|story|does|not|sound|true",
                    GrammarIDString = "#21#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "He often fails to keep his promises.",
                    SentenceJP = "彼は約束を守らないことがよくある。",
                    Level = 5,
                    Division = "fails|often|he|his|keep|promises|to",
                    StandardAnswerDivision = "he|often|fails|to|keep|his|promises",
                    GrammarIDString = "-1",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "You may have heard this joke before.",
                    SentenceJP = "その冗談は前に聞いたことがあるかもしれませんね。",
                    Level = 5,
                    Division = "before|have|heard|joke|may|this|you",
                    StandardAnswerDivision = "you|may|have|heard|this|joke|before",
                    GrammarIDString = "#13#17#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "I found the knife covered with dust.",
                    SentenceJP = "そのナイフがホコリだらけなのに私は気づいた。",
                    Level = 5,
                    Division = "covered|dust|found|I|knife|the|with",
                    StandardAnswerDivision = "I|found|the|knife|covered|with|dust",
                    GrammarIDString = "#16#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "She is too young to buy alcohol.",
                    SentenceJP = "彼女は、お酒を買うには若すぎる。",
                    Level = 5,
                    Division = "alcohol|buy|is|she|to|too|young",
                    StandardAnswerDivision = "she|is|too|young|to|buy|alcohol",
                    GrammarIDString = "#11#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "He is very much interested in Japanese history.",
                    SentenceJP = "彼は日本の歴史に非常に興味を持っている。",
                    Level = 5,
                    Division = "he|history|very much|in|interested|is|Japanese",
                    StandardAnswerDivision = "he|is|very much|interested|in|Japanese|history",
                    GrammarIDString = "-1",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "Can I have something cold to drink?",
                    SentenceJP = "何か冷たい飲み物をくれないか？",
                    Level = 5,
                    Division = "can|cold|drink|have|I|something|to",
                    StandardAnswerDivision = "can|I|have|something|cold|to|drink",
                    GrammarIDString = "#11#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "You did more work than I had expected.",
                    SentenceJP = "君は私たちが期待した以上の仕事をした。",
                    Level = 5,
                    Division = "did|expected|had|I|more|than|work|you",
                    StandardAnswerDivision = "you|did|more|work|than|I|had|expected",
                    GrammarIDString = "#14#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "Neither road will lead you to the destination.",
                    SentenceJP = "どちらの道を行ってもその目的地には着かないだろう。",
                    Level = 5,
                    Division = "destination|lead|neither|road|the|to|will|you",
                    StandardAnswerDivision = "neither|road|will|lead|you|to|the|destination",
                    GrammarIDString = "#3#15#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "Remembering the names of flowers is difficult for me.",
                    SentenceJP = "花の名前を覚えることは、私は苦手だ。",
                    Level = 5,
                    Division = "difficult|for|names of flowers|is|me|remembering|the",
                    StandardAnswerDivision = "difficult|for|names of flowers|is|me|remembering|the",
                    GrammarIDString = "#10#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "This is how he solved the difficult problem.",
                    SentenceJP = "このようにして彼はその難問を解いた。",
                    Level = 5,
                    Division = "this|difficult|he|how|is|problem|solved|the",
                    StandardAnswerDivision = "this|is|how|he|solved|the|difficult|problem",
                    GrammarIDString = "#6#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "He had to give up his trip to France.",
                    SentenceJP = "彼はフランスへの旅行を断念しなければならなかった。",
                    Level = 5,
                    Division = "France|give|had|he|his|to|to|trip|up",
                    StandardAnswerDivision = "he|had|to|give|up|his|trip|to|France",
                    GrammarIDString = "-1",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "She was pleased with this score on the test.",
                    SentenceJP = "彼女はテストの点に喜んだ。",
                    Level = 5,
                    Division = "on|pleased|score|she|this|was|with|the test",
                    StandardAnswerDivision = "she|was|pleased|with|this|score|on|the test",
                    GrammarIDString = "#12#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "He decided not to side with the stronger party.",
                    SentenceJP = "彼は強い方には味方しないと決心した。",
                    Level = 5,
                    Division = "decided|he|not|party|side|stronger|the|to|with",
                    StandardAnswerDivision = "he|decided|not|to|side|with|the|stronger|party",
                    GrammarIDString = "#11#15#22#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "This medicine will make you feel a bit better.",
                    SentenceJP = "この薬を飲めばすこしは気分が良くなるだろう。",
                    Level = 5,
                    Division = "a|better|bit|feel|make|medicine|this|will|you",
                    StandardAnswerDivision = "this|medicine|will|make|you|feel|a|bit|better",
                    GrammarIDString = "#3#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "The top of the mountain is covered with snow.",
                    SentenceJP = "その山の頂上は雪で覆われている。",
                    Level = 5,
                    Division = "covered|is|mountain|of|snow|the|the|top|with",
                    StandardAnswerDivision = "the|top|of|the|mountain|is|covered|with|snow",
                    GrammarIDString = "#12#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "When does your school break up for the summer?",
                    SentenceJP = "あなたの学校はいつ夏休みになりますか。",
                    Level = 5,
                    Division = "break|does|for|school|summer|the|up|when|your",
                    StandardAnswerDivision = "when|does|your|school|break|up|for|the|summer",
                    GrammarIDString = "-1",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "I'm sorry to have kept you waiting so long.",
                    SentenceJP = "長いことお待たせしてすみませんでした。",
                    Level = 5,
                    Division = "have|I'm|kept|long|so|sorry|to|waiting|you",
                    StandardAnswerDivision = "I'm|sorry|to|have|kept|you|waiting|so|long",
                    GrammarIDString = "#11#21#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "We must find out how to deal with the matter.",
                    SentenceJP = "私たちはその問題の処理方法を見つけ出さねばならない。",
                    Level = 5,
                    Division = "deal|find|how|matter|must|out|the|to|we|with",
                    StandardAnswerDivision = "we|must|find|out|how|to|deal|with|the|matter",
                    GrammarIDString = "#11#",
                    CreateDate = DateTime.Now,
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
                    EditorID = applicationUser3.Id
                },
                new Exercise
                {
                    Name = "テスト用問題集",
                    Description = "基本の操作を練習する",
                    Level = 4,
                    CreateDate = DateTime.Now,
                    EditorID = applicationUser1.Id
                },
                new Exercise
                {
                    Name = "問題集-実験①",
                    Description = "比較的に簡単な20問が含まれている",
                    Level = 5,
                    CreateDate = DateTime.Now,
                    EditorID = applicationUser1.Id
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
                            var exerciseQuestionRelation = new ExerciseQuestionRelation()
                            {
                                ExerciseID = exercises[i].ID,
                                QuestionID = questions[j].ID,
                                SerialNumber = j
                            };

                            _arDbContext.ExerciseQuestionRelationMap.Add(exerciseQuestionRelation);
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
                            var exerciseQuestion = new ExerciseQuestionRelation()
                            {
                                ExerciseID = exercises[i].ID,
                                QuestionID = questions[j].ID,
                                SerialNumber = j
                            };

                            _arDbContext.ExerciseQuestionRelationMap.Add(exerciseQuestion);
                            _arDbContext.SaveChanges();
                        }
                    }
                }

                // Exercise 3
                if (exercises[i].Level == 3)
                {
                    for (int j = 0; j < questions.Count; j++)
                    {
                        if (questions[j].Level == 1 || questions[j].Level == 2 || questions[j].Level == 3)
                        {
                            var exerciseQuestion = new ExerciseQuestionRelation()
                            {
                                ExerciseID = exercises[i].ID,
                                QuestionID = questions[j].ID,
                                SerialNumber = j
                            };

                            _arDbContext.ExerciseQuestionRelationMap.Add(exerciseQuestion);
                            _arDbContext.SaveChanges();
                        }
                    }
                }

                // Exercise 4
                if (exercises[i].Level == 4)
                {
                    for (int j = 0; j < questions.Count; j++)
                    {
                        if (questions[j].Level == 4)
                        {
                            var exerciseQuestion = new ExerciseQuestionRelation()
                            {
                                ExerciseID = exercises[i].ID,
                                QuestionID = questions[j].ID,
                                SerialNumber = j
                            };

                            _arDbContext.ExerciseQuestionRelationMap.Add(exerciseQuestion);
                            _arDbContext.SaveChanges();
                        }
                    }
                }

                // Exercise 4
                if (exercises[i].Level == 5)
                {
                    for (int j = 0; j < questions.Count; j++)
                    {
                        if (questions[j].Level == 5)
                        {
                            var exerciseQuestion = new ExerciseQuestionRelation()
                            {
                                ExerciseID = exercises[i].ID,
                                QuestionID = questions[j].ID,
                                SerialNumber = j
                            };

                            _arDbContext.ExerciseQuestionRelationMap.Add(exerciseQuestion);
                            _arDbContext.SaveChanges();
                        }
                    }
                }

            }

        }
    }
}
