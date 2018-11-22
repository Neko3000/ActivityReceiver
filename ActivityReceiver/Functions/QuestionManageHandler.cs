using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.Functions
{

    public class QuestionManageHandler
    {
        // 
        public static string ConvertGrammarIDStringToGrammarNameString(string grammarIDString,IList<Grammar> grammars)
        {
            var splittedGrammarIDs = grammarIDString.Split("#");

            var grammarNameString = "";
            for(int i = 0;i < splittedGrammarIDs.Count;i++)
            {
                if(i == 0){
                    grammarNameString = grammarNameString + grammargrammars.Where(g=>g.ID == Convert.ToInt32(splittedGrammarIDs[i])).SingleOrDefault().Name;
                }
                else{
                    grammarNameString = grammarNameString + "," + grammargrammars.Where(g=>g.ID == Convert.ToInt32(splittedGrammarIDs[i])).SingleOrDefault().Name;
                }
            }

            return grammarNameString;
        }

        public static IList<int> ConvertGrammarIDStringToGrammarIDList(string grammarIDString)
        {
            var splittedGrammarIDs = grammarIDString.Split("#");
            var grammarIDList = new List<int>();
            foreach(var grammarID in splittedGrammarIDs)
            {
                grammarIDList.Add(Convert.ToInt32(grammarID));
            }

            return grammarIDList;
        }

        public static IList<int> ConvertGrammarIDListToGrammarIDString(IList<int> grammarIDList)
        {
            var grammarIDString = "";
            for(int i = 0;i < grammarIDList.Count;i++)
            {
                if(i == 0){
                    grammarIDString = grammarIDString + grammarIDList[i].ToString();
                }
                else{
                    grammarIDString = grammarIDString + "," + grammarIDList[i].ToString();
                }
            }

            return grammarIDString;
        }

        /// <summary>
        /// It injects ItemCategory into each ItemDTO by its ItemCategory's ID which located in Item.
        /// </summary>
        /// <param name="items">the list of items need to be converted</param>
        /// <param name="itemCategories">all ItemCategories got from database</param>
        /// <returns>A list of ItemDTO, it has one-to-one relationship with items. </returns>
        public static IList<QuestionDTO>  ConvertToQuestionDTOForEachQuestion(IList<Question> questions,IList<Grammar> grammars,IList<ApplicationUser> applicationUsers)
        {
            var questionDTOs = new List<QuestionDTO>();

            foreach(var question in questions)
            {
                var questionDTO = Mapper.Map<Question,QuestionDTO>(question);
                
                questionDTO.GrammarNameString =ConvertGrammarIDStringToGrammarNameString(question.Grammar,grammars);
                questionDTO.EditorName = await applicationUsers.FindByIdAsync(question.EditorID).UserName;

                questionDTOs.Add(questionDTO);
            }
            return questionDTOs;
        }
    }
}
