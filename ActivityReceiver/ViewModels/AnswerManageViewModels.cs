using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Models;

namespace ActivityReceiver.ViewModels.AnswerManage
{
    public class AnswerPresenter
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "アサインメント")]
        public int AssignmentID { get; set; }

        [Display(Name = "英文")]
        public string SentenceEN { get; set; }

        [Display(Name = "日本文")]
        public string SentenceJP { get; set; }

        [Display(Name = "問題の区切り")]
        public string Division { get; set; }

        [Display(Name = "正解の区切り")]
        public string AnswerDivision { get; set; }

        [Display(Name = "解答")]
        public string Content { get; set; }

        [Display(Name = "評定")]
        public bool IsCorrect { get; set; }

        [Display(Name = "迷い度")]
        public int? HesitationDegree { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "開始時間")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "開始時間")]
        public DateTime EndDate { get; set; }
    }

    #region Index
    public class AnswerManageIndexViewModel
    {
        public IList<AnswerPresenter> AnswerPresenterCollection { get;set; }
    }
    #endregion

    #region Details
    public class AnswerManageDetailsViewModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "アサインメント")]
        public int AssignmentID { get; set; }

        [Display(Name = "英文")]
        public string SentenceEN { get; set; }

        [Display(Name = "日本文")]
        public string SentenceJP { get; set; }

        [Display(Name = "問題の区切り")]
        public string Division { get; set; }

        [Display(Name = "正解の区切り")]
        public string AnswerDivision { get; set; }

        [Display(Name = "解答")]
        public string Content { get; set; }

        [Display(Name = "評定")]
        public bool IsCorrect { get; set; }

        [Display(Name = "迷い度")]
        public int? HesitationDegree { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "開始時間")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "完成時間")]
        public DateTime EndDate { get; set; }

        public IList<Movement> MovementCollection { get; set; }
        public IList<DeviceAcceleration> DeviceAccelerationCollection { get; set; }
    }
    #endregion
}
