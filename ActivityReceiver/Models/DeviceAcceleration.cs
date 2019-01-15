using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivityReceiver.Models
{
    public class DeviceAcceleration
    {
        [Key]
        public int ID { get; set; }
        public int AnswerRecordID { get; set; }

        public int Time { get; set; }
        
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}
