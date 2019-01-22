using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivityReceiver.Models
{
    public class AnswerRecord
    {
        [Key]
        public int ID { get; set; }
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
    }
}
