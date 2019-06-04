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
                await _userManager.AddToRoleAsync(applicationUser7, "Student");

                var applicationUser8 = new ApplicationUser
                {
                    UserName = "Banno",
                };
                var applicationUserPWD8 = "b123456";
                await _userManager.CreateAsync(applicationUser8, applicationUserPWD8);
                await _userManager.AddToRoleAsync(applicationUser8, "Student");

                var applicationUser9 = new ApplicationUser
                {
                    UserName = "Ando",
                };
                var applicationUserPWD9 = "a123456";
                await _userManager.CreateAsync(applicationUser9, applicationUserPWD9);
                await _userManager.AddToRoleAsync(applicationUser9, "Student");

                var applicationUser10 = new ApplicationUser
                {
                    UserName = "Waki",
                };
                var applicationUserPWD10 = "w123456";
                await _userManager.CreateAsync(applicationUser10, applicationUserPWD10);
                await _userManager.AddToRoleAsync(applicationUser10, "Student");

                var applicationUser11 = new ApplicationUser
                {
                    UserName = "Shirasu",
                };
                var applicationUserPWD11 = "s123456";
                await _userManager.CreateAsync(applicationUser11, applicationUserPWD11);
                await _userManager.AddToRoleAsync(applicationUser11, "Student");

                string[] studentIDCollection = { "1518350090", "1718310007", "1818310009", "1818310018", "1818310032", "1818310035", "1818310041", "1818310042", "1818320067", "1818320086",
                    "1818380016", "1818380092", "1818380100", "1918310003", "1718310007", "1918310009", "1918310029", "1918310050", "1918310085", "1918310096",
                    "1918310099", "1918380003", "1918380005", "1918380010", "1918380012", "1918380019", "1918380033", "1918380038", "1918380046", "1918380047",
                    "1918380064", "1918380087", "1918380094", "1918320003", "1918320005", "1918320009", "1918320038", "1918320042", "1918320045", "1918320066",
                    "1918320079", "1918320081", "1918320086", "1918360003", "1918360008", "1918360026", "1918360036", "1918360042", "1918360066"};
                studentIDCollection = studentIDCollection.Distinct().ToArray();
                for (int i = 0; i < studentIDCollection.Count(); i++)
                {
                    var applicationUserTemp = new ApplicationUser
                    {
                        UserName = studentIDCollection[i],
                    };
                    var applicationUserPWDTemp = studentIDCollection[i];
                    await _userManager.CreateAsync(applicationUserTemp, applicationUserPWDTemp);
                    await _userManager.AddToRoleAsync(applicationUserTemp, "Student");
                }

                var applicationUser12 = new ApplicationUser
                {
                    UserName = "Test1",
                };
                var applicationUserPWD12 = "Test1123456";
                await _userManager.CreateAsync(applicationUser12, applicationUserPWD12);
                await _userManager.AddToRoleAsync(applicationUser12, "Student");

                var applicationUser13 = new ApplicationUser
                {
                    UserName = "Test2",
                };
                var applicationUserPWD13 = "Test2123456";
                await _userManager.CreateAsync(applicationUser13, applicationUserPWD13);
                await _userManager.AddToRoleAsync(applicationUser13, "Student");

                var applicationUser14 = new ApplicationUser
                {
                    UserName = "Test3",
                };
                var applicationUserPWD14 = "Test3123456";
                await _userManager.CreateAsync(applicationUser14, applicationUserPWD14);
                await _userManager.AddToRoleAsync(applicationUser14, "Student");

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
                    SentenceEN = "His explanation was far from satisfactory.",
                    SentenceJP = "彼の話は本当のようには思えない。",
                    Level = 5,
                    Division = "explanation|far|from|his|satisfactory|was",
                    StandardAnswerDivision = "his|explanation|was|far|from|satisfactory",
                    GrammarIDString = "-1",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "He is anything but a scholar.",
                    SentenceJP = "彼が学者などということはない。",
                    Level = 5,
                    Division = "a|anything|but|he|is|scholar",
                    StandardAnswerDivision = "he|is|anything|but|a|scholar",
                    GrammarIDString = "-1",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "He is recovering from his illness.",
                    SentenceJP = "彼の病気は快方に向かっている。",
                    Level = 5,
                    Division = "from|he|his|illness|is|recovering",
                    StandardAnswerDivision = "he|is|recovering|from|his|illness",
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
                    GrammarIDString = "-1",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "Help yourself to anything you like.",
                    SentenceJP = "何でもお好きなものを召し上がってください。",
                    Level = 5,
                    Division = "anything|help|like|to|you|yourself",
                    StandardAnswerDivision = "help|yourself|to|anything|you|like",
                    GrammarIDString = "-1",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "There is something refined about him.",
                    SentenceJP = "彼にはどこか上品なところがある。",
                    Level = 5,
                    Division = "about|him|is|refined|something|there",
                    StandardAnswerDivision = "there|is|something|refined|about|him",
                    GrammarIDString = "-1",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "I had my fingers caught in the train doors.",
                    SentenceJP = "私は電車のドアに指をはさまれた。",
                    Level = 5,
                    Division = "caught|doors|had|I|in|my fingers|the train",
                    StandardAnswerDivision = "I|had|my fingers|caught|in|the train|doors",
                    GrammarIDString = "#20#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "We hope for his success in business.",
                    SentenceJP = "私たちは彼が事業で成功することを願っている。",
                    Level = 5,
                    Division = "business|for|his|hope|in|success|we",
                    StandardAnswerDivision = "we|hope|for|his|success|in|business",
                    GrammarIDString = "#8#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "Hardly had the game started when it began to rain.",
                    SentenceJP = "試合が始まるとすぐに雨が降り出した。",
                    Level = 5,
                    Division = "began to|game|had the|hardly|it|rain|started when",
                    StandardAnswerDivision = "hardly|had the|game|started when|it|began to|rain",
                    GrammarIDString = "#4#5#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "All of them voted against the proposal.",
                    SentenceJP = "彼らは全員、その提案に反対の票を入れた。",
                    Level = 5,
                    Division = "against|all|of|proposal|the|them|voted",
                    StandardAnswerDivision = "all|of|them|voted|against|the|proposal",
                    GrammarIDString = "#8#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "Japan is often compared to an over-crowded bus.",
                    SentenceJP = "日本はよく混雑したバスに例えられる。",
                    Level = 5,
                    Division = "an|compared|is|Japan|often|to|over-crowded bus",
                    StandardAnswerDivision = "Japan|is|often|compared|to|an|over-crowded bus",
                    GrammarIDString = "#12#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "They were promised higher wages by their boss.",
                    SentenceJP = "彼らは上司から昇給を約束された。",
                    Level = 5,
                    Division = "by|higher|promised|they|wages|were|their boss",
                    StandardAnswerDivision = "they|were|promised|higher|wages|by|their boss",
                    GrammarIDString = "#12#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "He insisted on my paying the debt.",
                    SentenceJP = "彼は私が借金を支払うようにと言い張った。",
                    Level = 5,
                    Division = "debt|he|insisted|my|on|paying|the",
                    StandardAnswerDivision = "he|insisted|on|my|paying|the|debt",
                    GrammarIDString = "#10#18#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "This house needs repairing before winter comes.",
                    SentenceJP = "冬がやってくるまでにこの家を改修する必要がある。",
                    Level = 5,
                    Division = "before|house|needs|repairing|this|winter|comes",
                    StandardAnswerDivision = "this|house|needs|repairing|before|winter |comes",
                    GrammarIDString = "#10#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "They went out of the room unobserved.",
                    SentenceJP = "彼らは気づかれずに部屋を出て行った。",
                    Level = 5,
                    Division = "of|out|room|the|they|unobserved|went",
                    StandardAnswerDivision = "they|went|out|of|the|room|unobserved",
                    GrammarIDString = "#9#21#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "The last ten years have seen a remarkable development of computers.",
                    SentenceJP = "過去10年の間にコンピュータは目覚ましい発展を遂げた。",
                    Level = 5,
                    Division = "a remarkable|development|of computers|seen|ten|the last|years have",
                    StandardAnswerDivision = "the last|ten|years have|seen|a remarkable|development|of computers",
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
                    SentenceEN = "Would you mind waiting another ten minutes?",
                    SentenceJP = "もう10分ほどお待ちくださいませんか?",
                    Level = 5,
                    Division = "another|mind|minutes|ten|waiting|would|you",
                    StandardAnswerDivision = "would|you|mind|waiting|another|ten|minutes",
                    GrammarIDString = "-1",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "She told her children to eat more vegetables.",
                    SentenceJP = "彼女は子どもたちにもっと野菜を食べるように言った。",
                    Level = 5,
                    Division = "children|eat|her|more|she|to|told|vegetables",
                    StandardAnswerDivision = "she|told|her|children|to|eat|more|vegetables",
                    GrammarIDString = "#11#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "I want you to clean up this mess.",
                    SentenceJP = "あなたにこの散らかりを片付けてもらいたい。",
                    Level = 5,
                    Division = "clean|I|mess|this|to|up|want|you",
                    StandardAnswerDivision = "I|want|you|to|clean|up|this|mess",
                    GrammarIDString = "#11#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "You did more work than I had expected.",
                    SentenceJP = "君は私たちが期待した以上の仕事をした。。",
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
                    SentenceEN = "Keeping early hours is good for the health.",
                    SentenceJP = "早寝早起きをすることは健康に良いことだ。",
                    Level = 5,
                    Division = "early|for|good|health|hours|is|keeping|the",
                    StandardAnswerDivision = "keeping|early|hours|is|good|for|the|health",
                    GrammarIDString = "#10#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "I'm sure of it being fine tomorrow morning.",
                    SentenceJP = "明日の朝はきっと天気がよくなると思う。",
                    Level = 5,
                    Division = "being|fine|it|I'm|morning|of|sure|tomorrow",
                    StandardAnswerDivision = "I'm|sure|of|it|being|fine|tomorrow|morning",
                    GrammarIDString = "#10#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "I cannot stand being talked about by others.",
                    SentenceJP = "私は他人にとやかく言われるのには我慢がならない。",
                    Level = 5,
                    Division = "about|being|by|cannot|I|others|stand|talked",
                    StandardAnswerDivision = "I|cannot|stand|being|talked|about|by|others",
                    GrammarIDString = "#10#12#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "The story reminds me of his late father.",
                    SentenceJP = "その話を聞くと彼の死んだお父さんのことを思い出す。",
                    Level = 5,
                    Division = "father|his|late|me|of|reminds|story|the",
                    StandardAnswerDivision = "the|story|reminds|me|of|his|late|father",
                    GrammarIDString = "-1",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "She told her children to eat more vegetables.",
                    SentenceJP = "彼女は子どもたちにもっと野菜を食べるように言った。",
                    Level = 5,
                    Division = "children|eat|her|more|she|to|told|vegetables",
                    StandardAnswerDivision = "she|told|her|children|to|eat|more|vegetables",
                    GrammarIDString = "#11#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "You must have been thinking of something else.",
                    SentenceJP = "君は何か他のことを考えていたに違いない。",
                    Level = 5,
                    Division = "been|else|have|must|of|something|thinking|you",
                    StandardAnswerDivision = "you|must|have|been|thinking|of|something|else",
                    GrammarIDString = "#13#17#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "Everybody was anxious to know what had happened.",
                    SentenceJP = "何事が起ったのか誰もが知りたがっていた。",
                    Level = 5,
                    Division = "anxious|everybody|had|happened|know|to|was|what",
                    StandardAnswerDivision = "everybody|was|anxious|to|know|what|had|happened",
                    GrammarIDString = "#6#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "The fact is that I don't know anything about him.",
                    SentenceJP = "実は、彼のことを私は何も知らないのです。",
                    Level = 5,
                    Division = "about|anything|fact|him|I don't know|is|that|the",
                    StandardAnswerDivision = "the|fact|is|that|I don't know|anything|about|him",
                    GrammarIDString = "#4#",
                    CreateDate = DateTime.Now,
                    Remark = "",
                },
                new Question()
                {
                    EditorID = applicationUser1.Id,
                    SentenceEN = "He was advised by his lawyer to sell the land.",
                    SentenceJP = "彼は、その土地を売るように弁護士から忠告された。",
                    Level = 5,
                    Division = "advised|by|he|land|his lawyer|sell|the|to|was",
                    StandardAnswerDivision = "he|was|advised|by|his lawyer|to|sell|the|land",
                    GrammarIDString = "#11#12#",
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
                    Description = "比較的に簡単な30問が含まれている",
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

                // Exercise 5
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
