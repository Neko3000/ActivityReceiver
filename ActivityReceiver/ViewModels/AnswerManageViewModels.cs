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

        [Display(Name = "Drop->Drag平均時間")]
        public float DDIntervalAVG { get; set; }

        [Display(Name = "Drop->Drag最大時間")]
        public float DDIntervalMAX { get; set; }

        [Display(Name = "Drop->Drag最小時間")]
        public float DDIntervalMIN { get; set; }

        [Display(Name = "Drag->Drop平均時間")]
        public float DDProcessAVG { get; set; }

        [Display(Name = "Drag->Drop最大時間")]
        public float DDProcessMAX { get; set; }

        [Display(Name = "Drag->Drop最小時間")]
        public float DDProcessMIN { get; set; }

        [Display(Name = "総移動距離")]
        public float TotalDistance { get; set; }

        [Display(Name = "Drag->Drop平均速度")]
        public float DDSpeedAVG { get; set; }

        [Display(Name = "Drag->Drop最大速度")]
        public float DDSpeedMAX { get; set; }

        [Display(Name = "Drag->Drop最小速度")]
        public float DDSpeedMIN { get; set; }

        [Display(Name = "初動時間")]
        public float DDFirstTime { get; set; }

        [Display(Name = "D＆D回数")]
        public int DDCount { get; set; }
    }
    #endregion
}
