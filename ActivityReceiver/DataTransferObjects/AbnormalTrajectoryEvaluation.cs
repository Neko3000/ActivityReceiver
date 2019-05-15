using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.DataTransferObjects
{
    public class AbnormalTrajectoryEvaluation
    {
        public float ThACC { get; set; }
        public float ThFOC { get; set; }

        public float Precision { get; set; }
        public float Recall { get; set; }
        public float FMeasure { get; set; }

        public string Username { get; set; }
    }
}
