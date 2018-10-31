using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivityReceiver.Models
{
    public class Answer
    {
        [Key]
        public int ID { get; set; }
        public int AssignmentRecordID { get; set; }
        public string Content { get; set; }

        public int? HesitationDegree { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
