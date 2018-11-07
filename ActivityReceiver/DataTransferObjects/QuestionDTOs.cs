using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.DataTransferObjects
{
    public class ExerciseDetail
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public int CurrentNumber { get; set; }
        public int TotalNumber { get; set; }

        public bool IsFinished { get; set; }
    }

    public class AssignmentResult
    {
        public float AccuracyRate { get; set;  }

        public IList<AnswerDetail> AnswerDetails { get; set; }
    }

    public class AnswerDetail
    {
        public string SentenceJP { get; set; }
        public string SentenceEN { get; set; }

        public string Answer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
