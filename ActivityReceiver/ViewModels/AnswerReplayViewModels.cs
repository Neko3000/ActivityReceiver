using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Models;

namespace ActivityReceiver.ViewModels
{

    #region GetAnswer
    public class GetAnswerGetViewModel
    {
        public int ID { get; set; }

        public int QuestionID { get; set; }
        public int AssignmentRecordID { get; set; }

        public string SentenceEN { get; set; }
        public string SentenceJP { get; set; }
        public string Division { get; set; }
        public string AnswerDivision { get; set; }

        public string Content { get; set; }
        public bool IsCorrect { get; set; }

        public int? HesitationDegree { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public IList<MovementDTO> MovementDTOs { get; set; }
    }
    #endregion
}
