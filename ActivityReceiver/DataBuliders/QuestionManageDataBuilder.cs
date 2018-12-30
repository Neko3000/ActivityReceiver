using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Data;
using ActivityReceiver.Models;
using ActivityReceiver.ViewModels.QuestionManage;
using AutoMapper.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ActivityReceiver.Functions;

namespace ActivityReceiver.DataBuilders
{
    public class QuestionManageDataBuilder
    {
        private readonly ActivityReceiverDbContext _arDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public QuestionManageDataBuilder(ActivityReceiverDbContext arDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _arDbContext = arDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // From Handler
        public static string ConvertGrammarIDStringToGrammarNameString(string grammarIDString, IList<Grammar> grammars)
        {
            var splittedGrammarIDs = grammarIDString.Split("#");
            splittedGrammarIDs = splittedGrammarIDs.Where(s => s != "").ToArray();

            var grammarNameString = "";
            for (int i = 0; i < splittedGrammarIDs.Count(); i++)
            {
                var grammar = grammars.Where(g => g.ID == Convert.ToInt32(splittedGrammarIDs[i])).SingleOrDefault();

                var grammarName = "";

                if (grammar != null)
                {
                    grammarName = grammar.Name;
                }

                if (i == 0)
                {
                    grammarNameString = grammarNameString + grammarName;
                }
                else
                {
                    grammarNameString = grammarNameString + " - " + grammarName;
                }
            }

            return grammarNameString;
        }

        public static IList<int> ConvertGrammarIDStringToGrammarIDList(string grammarIDString)
        {
            var splittedGrammarIDs = grammarIDString.Split("#");
            var grammarIDList = new List<int>();
            foreach (var grammarID in splittedGrammarIDs)
            {
                grammarIDList.Add(Convert.ToInt32(grammarID));
            }

            return grammarIDList;
        }

        public static string ConvertGrammarIDListToGrammarIDString(IList<int> grammarIDList)
        {
            var grammarIDString = "#";
            for (int i = 0; i < grammarIDList.Count; i++)
            {
                if (i == 0)
                {
                    grammarIDString = grammarIDString + grammarIDList[i].ToString();
                }
                else
                {
                    grammarIDString = grammarIDString + "#" + grammarIDList[i].ToString();
                }
            }
            grammarIDString = grammarIDString + "#";

            return grammarIDString;
        }

        public async Task<IList<QuestionPresenter>> BuildQuestionPresenterList()
        {
            var questionList = await _arDbContext.Questions.ToListAsync();

            var questionPresenterCollection = new List<QuestionPresenter>();
            foreach (var question in questionList)
            {
                var questionPresenter = Mapper.Map<Question, QuestionPresenter>(question);
                questionPresenter.GrammarNameString = ConvertGrammarIDStringToGrammarNameString(question.GrammarIDString, _arDbContext.Grammars.ToList());
                questionPresenter.EditorName = (await _userManager.FindByIdAsync(question.EditorID)).UserName;

                questionPresenterCollection.Add(questionPresenter);
            }

            return questionPresenterCollection;
        }
    }
}
