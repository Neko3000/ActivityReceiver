using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ActivityReceiver.DataTransferObjects
{
    public class DeviceAccelerationCombined
    {
        public int ID { get; set; }
        public int AnswerRecordID { get; set; }

        public int Index { get; set; }
        public int Time { get; set; }
        
        public float Acceleration { get; set; }
    }
}
