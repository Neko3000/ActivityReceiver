using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Models;

namespace ActivityReceiver.ViewModels.AssignmentRecordManage
{

    public class AnswerPresenter
    {
        public int ID { get; set; }

        public string SentenceEN { get; set; }
        public string SentenceJP { get; set; }
        public string Division { get; set; }
        public string AnswerDivision { get; set; }

        public string Resolution { get; set; }

        public string Content { get; set; }
        public bool IsCorrect { get; set; }

        public int? HesitationDegree { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class AssignmentRecordPresenter
    {
        public int ID { get; set; }
        public string Username { get; set; }

        public string ExerciseName { get; set; }
        public string CurrentProgress { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsFinished { get; set; }
        public float Grade { get; set; }

        public string Remark { get; set; }

        public IList<AnswerPresenter> AnswerPresenterCollection { get; set; }
    }

    #region Index
    public class AssignmentRecordManageIndexViewModel
    {
        public IList<AssignmentRecordPresenter> AssignmentRecordPresenterCollection { get;set; }
    }
    #endregion

    #region Details
    public class AssignmentRecordManageDetailsViewModel
    {
        [Display(Name ="ID")]
        public int ID { get; set; }
        [Display(Name = "学習者")]
        public string Username { get; set; }

        [Display(Name = "問題集")]
        public string ExerciseName { get; set; }
        [Display(Name = "進度")]
        public string CurrentProgress { get; set; }

        [Display(Name = "開始時間")]
        public DateTime StartDate { get; set; }
        [Display(Name = "完成時間")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "完成度")]
        public bool IsFinished { get; set; }
        [Display(Name = "得点")]
        public float Grade { get; set; }

        [Display(Name = "リマーク")]
        public string Remark { get; set; }

        public IList<AnswerPresenter> AnswerPresenterCollection { get; set; }
    }
    #endregion

}
