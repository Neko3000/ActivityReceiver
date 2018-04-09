using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.Models.DataTransferObject
{
    public class MovementDto
    {
        public int ID { get; set; }
        public int QID { get; set; }
        public int UID { get; set; }
        public int Index { get; set; }
        public int State { get; set; }
        public int Time { get; set; }
        public int XPosition { get; set; }
        public int YPostion { get; set; }
        public bool IsFinished { get; set; }
    }
}
