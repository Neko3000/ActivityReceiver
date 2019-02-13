using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Models;

namespace ActivityReceiver.ViewModels.AnswerReplay
{
    #region Replayer
    public class AnswerReplayReplayerViewModel
    {
        public int AnswerRecordID { get; set; }
    }
    #endregion

    #region GetAnswer
    public class AnswerReplayGetAnswerViewModel
    {
        public int ID { get; set; }

        public int QuestionID { get; set; }
        public int AssignmentRecordID { get; set; }

        public string SentenceEN { get; set; }
        public string SentenceJP { get; set; }
        public string Division { get; set; }
        public string StandardAnswerDivision { get; set; }

        public string Resolution { get; set; }

        public string AnswerDivision { get; set; }
        public bool IsCorrect { get; set; }

        public int ConfusionDegree { get; set; }
        public string ConfusionElement { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public IList<Movement> MovementCollection { get; set; }
        public IList<DeviceAcceleration> DeviceAccelerationCollection { get; set; }
    }
    #endregion


}
