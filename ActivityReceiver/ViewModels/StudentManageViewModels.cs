using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Models;
using ActivityReceiver.ViewModels.UserManage;

namespace ActivityReceiver.ViewModels.StudentManage
{
    public class StudentPresenter
    {
        [Display(Name = "ID")]
        public string ID { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Display(Name = "正解率")]
        public float AccuracyRate { get; set; }

        [Display(Name = "エクササイズの完成数")]
        public int FinishedExerciseCount { get; set; }
    }

    #region Index
    public class StudentManageIndexViewModel
    {
        public IList<StudentPresenter> StudentPresenterCollection { get;set; }
    }
    #endregion

    #region Edit
    public class StudentManageEditViewModel
    {
        [Display(Name = "ID")]
        public string ID { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Display(Name = "正解率")]
        public float AccuracyRate { get; set; }

        [Display(Name = "エクササイズの完成数")]
        public int FinishedExerciseCount { get; set; }
    }
    #endregion
}
