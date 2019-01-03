﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Models;
using ActivityReceiver.ViewModels;
using ActivityReceiver.Data;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace ActivityReceiver.Functions
{
    public class QuestionManageHandler
    {
        public static string ConvertGrammarIDStringToGrammarNameString(string grammarIDString,IList<Grammar> grammars)
        {
            var splittedGrammarIDs = grammarIDString.Split("#");
            splittedGrammarIDs = splittedGrammarIDs.Where(s => s != "").ToArray();

            var grammarNameString = "";
            for(int i = 0;i < splittedGrammarIDs.Count();i++)
            {
                var grammar= grammars.Where(g => g.ID == Convert.ToInt32(splittedGrammarIDs[i])).SingleOrDefault();

                var grammarName = "";

                if (grammar != null)
                {
                    grammarName = grammar.Name;
                }
                    
                if (i == 0){
                    grammarNameString = grammarNameString + grammarName;
                }
                else{
                    grammarNameString = grammarNameString + " - " + grammarName;
                }
            }

            return grammarNameString;
        }

        public static IList<int> ConvertGrammarIDStringToGrammarIDList(string grammarIDString)
        {
            var splittedGrammarIDs = grammarIDString.Split("#");
            splittedGrammarIDs = splittedGrammarIDs.Where(s => s != "").ToArray();

            var grammarIDList = new List<int>();
            foreach(var grammarID in splittedGrammarIDs)
            {
                grammarIDList.Add(Convert.ToInt32(grammarID));
            }

            return grammarIDList;
        }

        public static string ConvertGrammarIDListToGrammarIDString(IList<int> grammarIDList)
        {
            var grammarIDString = "#";
            for(int i = 0;i < grammarIDList.Count;i++)
            {
                if(i == 0){
                    grammarIDString = grammarIDString + grammarIDList[i].ToString();
                }
                else{
                    grammarIDString = grammarIDString + "#" + grammarIDList[i].ToString();
                }
            }
            grammarIDString = grammarIDString + "#";

            return grammarIDString;
        }
    }
}
