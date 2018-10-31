using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivityReceiver.Models
{
    public class AssignmentRecord
    {
        [Key]
        public int ID { get; set; }
        public string UserID { get; set; }

        public int ExerciseID { get; set; }
        public int CurrentQuestionIndex { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsFinished { get; set; }
        public float Grade { get; set; }

        public string Remark { get; set; }
    }
}
