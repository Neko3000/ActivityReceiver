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
        public int QusetionID { get; set; }
        public int UserID { get; set; }
        public int? AnswserID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
