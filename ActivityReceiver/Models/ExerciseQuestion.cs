using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.Models
{
    public class ExerciseQuestion
    {
        [Key]
        public int ID { get; set; }

        public int ExerciseID { get; set; }
        public int QuestionID { get; set; }

        public int SerialNumber { get; set; }
    }
}
