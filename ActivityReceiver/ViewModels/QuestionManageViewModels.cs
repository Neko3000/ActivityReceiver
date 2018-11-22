using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Models;

namespace ActivityReceiver.ViewModels
{
    public class QuestionDTO
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "英文")]
        public string SentenceEN { get; set; }

        [Display(Name = "日本文")]
        public string SentenceJP { get; set; }

        [Display(Name = "レベル")]
        public int Level { get; set; }

        [Display(Name = "文法")]
        public string GrammarNameString { get; set; }

        [Display(Name = "問題の区切り")]
        public string Division { get; set; }

        [Display(Name = "正解の区切り")]
        public string AnswerDivision { get; set; }

        [Display(Name = "コメント")]
        public string Remark { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "作成日付")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "著者")]
        public string EditorName { get; set; }
    }

    #region Index
    public class QuestionManageIndexViewModel
    {
        public IList<QuestionDTO> QuestionDTOs { get;set; }
    }
    #endregion

    #region Create
    public class QuestionManageCreateGetViewModel
    {
        [Display(Name = "英文")]
        public string SentenceEN { get; set; }

        [Display(Name = "日本文")]
        public string SentenceJP { get; set; }

        [Display(Name = "レベル")]
        public int Level { get; set; }

        [Display(Name = "文法")]
        public IList<int> GrammarIDs { get; set; }

        [Display(Name = "問題の区切り")]
        public string Division { get; set; }

        [Display(Name = "正解の区切り")]
        public string AnswerDivision { get; set; }

        [Display(Name = "コメント")]
        public string Remark { get; set; }

        public IList<Grammar> Grammars{ get; set; }    
        
    }

    public class QuestionManageCreatePostViewModel
    {
        [Display(Name = "英文")]
        public string SentenceEN { get; set; }

        [Display(Name = "日本文")]
        public string SentenceJP { get; set; }

        [Display(Name = "レベル")]
        public int Level { get; set; }

        [Display(Name = "文法")]
        public IList<int> GrammarIDs { get; set; }

        [Display(Name = "問題の区切り")]
        public string Division { get; set; }

        [Display(Name = "正解の区切り")]
        public string AnswerDivision { get; set; }

        [Display(Name = "コメント")]
        public string Remark { get; set; }  
        
    }
    #endregion

    #region Edit
    public class QuestionManageEditGetViewModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "英文")]
        public string SentenceEN { get; set; }

        [Display(Name = "日本文")]
        public string SentenceJP { get; set; }

        [Display(Name = "レベル")]
        public int Level { get; set; }

        [Display(Name = "文法")]
        public IList<int> GrammarIDs { get; set; }

        [Display(Name = "問題の区切り")]
        public string Division { get; set; }

        [Display(Name = "正解の区切り")]
        public string AnswerDivision { get; set; }

        [Display(Name = "コメント")]
        public string Remark { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "作成日付")]
        public DateTime CreateDate { get; set; }


        [Display(Name = "著者")]
        public string EditorID { get; set; }

        public IList<Grammar> Grammars{ get; set; }
        public IList<ApplicationUserDTO>  ApplicationUserDTOs { get;set; }
        
    }
    public class QuestionManageEditPostViewModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "英文")]
        public string SentenceEN { get; set; }

        [Display(Name = "日本文")]
        public string SentenceJP { get; set; }

        [Display(Name = "レベル")]
        public int Level { get; set; }

        [Display(Name = "文法")]
        public IList<int> GrammarIDs { get; set; }

        [Display(Name = "問題の区切り")]
        public string Division { get; set; }

        [Display(Name = "正解の区切り")]
        public string AnswerDivision { get; set; }

        [Display(Name = "コメント")]
        public string Remark { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "作成日付")]
        public DateTime CreateDate { get; set; }


        [Display(Name = "著者")]
        public string EditorID { get; set; }   
    }
    #endregion

    #region Details
    public class QuestionManageDetailsViewModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "英文")]
        public string SentenceEN { get; set; }

        [Display(Name = "日本文")]
        public string SentenceJP { get; set; }

        [Display(Name = "レベル")]
        public int Level { get; set; }

        [Display(Name = "文法")]
        public string GrammarNameString { get; set; }

        [Display(Name = "問題の区切り")]
        public string Division { get; set; }

        [Display(Name = "正解の区切り")]
        public string AnswerDivision { get; set; }

        [Display(Name = "コメント")]
        public string Remark { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "作成日付")]
        public DateTime CreateDate { get; set; }


        [Display(Name = "著者")]
        public string EditorName { get; set; }      
    }
    #endregion

    #region Delete
    public class QuestionManageDeleteGetViewModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "英文")]
        public string SentenceEN { get; set; }

        [Display(Name = "日本文")]
        public string SentenceJP { get; set; }

        [Display(Name = "レベル")]
        public int Level { get; set; }

        [Display(Name = "文法")]
        public string GrammarNameString { get; set; }

        [Display(Name = "問題の区切り")]
        public string Division { get; set; }

        [Display(Name = "正解の区切り")]
        public string AnswerDivision { get; set; }

        [Display(Name = "コメント")]
        public string Remark { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "作成日付")]
        public DateTime CreateDate { get; set; }


        [Display(Name = "著者")]
        public string EditorName { get; set; }

        public IList<GrammarDTO> GrammarDTOs{ get; set; }        
    }
    public class QuestionManageDeletePostViewModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }
    }
    #endregion
}
