using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivityReceiver.DataTransferObjects
{
    public class MovementSupervised
    {
        [Key]
        public int ID { get; set; }
        public int AnswerRecordID { get; set; }

        public int Index { get; set; }
        public int Time { get; set; }

        public int State { get; set; }
        public string TargetElement { get; set; }

        public int XPosition { get; set; }
        public int YPosition { get; set; }

        public float Force { get; set; }

        public bool IsAbnormal { get; set; }
    }
}
