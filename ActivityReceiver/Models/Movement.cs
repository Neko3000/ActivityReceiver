using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivityReceiver.Models
{
    public class Movement
    {
        [Key]
        public int ID { get; set; }
        public int AnswerRecordID { get; set; }
        public int Index { get; set; }
        public int State { get; set; }
        public int Time { get; set; }
        public int XPosition { get; set; }
        public int YPostion { get; set; }
        public bool IsFinished { get; set; }
    }
}
