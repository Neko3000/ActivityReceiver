using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.Functions
{
    public static class QuestionHandler
    {
        public static string ConvertDivisionToSentence(string division)
        {
            string[] splittedDivision = division.Split("|");

            string sentence = "";
            for (int i = 0; i < splittedDivision.Count(); i++)
            {
                if (i == 0)
                {
                    sentence = sentence + splittedDivision[i];
                }
                else
                {
                    sentence = sentence + " " + splittedDivision[i];
                }
            }

            sentence = sentence + ".";
            string captializedSentence = sentence.First().ToString().ToUpper() + sentence.Substring(1);

            return captializedSentence;
        }

        public static string ConvertSentenceToDivision(string sentence)
        {
            string[] splittedSentence = sentence.ToLower().Split(" ");
            splittedSentence = splittedSentence.Where(s => s != ".").ToArray();

            string division = "";
            for(int i = 0; i<splittedSentence.Count(); i++)
            {
                if(i==0)
                {
                    division = division + splittedSentence[i];
                }
                else
                {
                    division = division + "|" + splittedSentence[i];
                }
            }

            return division;
        }
    }
}
