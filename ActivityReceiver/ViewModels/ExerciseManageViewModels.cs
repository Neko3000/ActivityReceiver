using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Models;
using ActivityReceiver.ViewModels.UserManage;

namespace ActivityReceiver.ViewModels.ExerciseManage
{
    public class ExercisePresenter
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "名前")]
        public string Name { get; set; }

        [Display(Name = "説明")]
        public string Description { get; set; }

        [Display(Name = "レベル")]
        public int Level { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "作成日付")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "著者")]
        public string EditorName { get; set; }

        [Display(Name = "問題")]
        public IList<Question> QuestionCollection { get; set; }
    }

    #region Index
    public class ExerciseManageIndexViewModel
    {
        public IList<ExercisePresenter> ExercisePresenterCollection { get;set; }
    }
    #endregion

    #region Create
    public class ExerciseManageCreateGetViewModel
    {
        [Display(Name = "名前")]
        public string Name { get; set; }

        [Display(Name = "説明")]
        public string Description { get; set; }

        [Display(Name = "レベル")]
        public int Level { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "作成日付")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "問題")]
        public IList<int> SelectedQuestionIDCollection { get; set; }

        public IList<Question> EntireQuestionCollection{ get; set; }       
    }

    public class ExerciseManageCreatePostViewModel
    {
        [Display(Name = "名前")]
        public string Name { get; set; }

        [Display(Name = "説明")]
        public string Description { get; set; }

        [Display(Name = "レベル")]
        public int Level { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "作成日付")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "問題")]
        public IList<int> QuestionIDCollection { get; set; }

    }
    #endregion

    #region Edit
    public class ExerciseManageEditGetViewModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "名前")]
        public string Name { get; set; }

        [Display(Name = "説明")]
        public string Description { get; set; }

        [Display(Name = "レベル")]
        public int Level { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "作成日付")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "著者")]
        public string EditorID { get; set; }

        [Display(Name = "問題")]
        public IList<int> SelectedQuestionIDCollection { get; set; }

        public IList<ApplicationUser> ApplicationUserCollection { get; set; }
        public IList<Question> EntireQuestionCollection { get; set; }
    }
    public class ExerciseManageEditPostViewModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "名前")]
        public string Name { get; set; }

        [Display(Name = "説明")]
        public string Description { get; set; }

        [Display(Name = "レベル")]
        public int Level { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "作成日付")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "著者")]
        public string EditorID { get; set; }

        [Display(Name = "問題")]
        public IList<int> SelectedQuestionIDCollection { get; set; }
    }
    #endregion

    #region Details
    public class ExerciseManageDetailsViewModel
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
    public class ExerciseManageDeleteGetViewModel
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
    public class ExerciseManageDeletePostViewModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }
    }
    #endregion
}
