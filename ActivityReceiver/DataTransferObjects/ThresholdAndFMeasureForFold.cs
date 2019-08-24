using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.DataTransferObjects
{
    public class ThresholdAndFMeasureForFold
    {
        public float theBestThACC { get; set; }
        public float theBestThFOC { get; set; }
        public float theBestFMeasure { get; set; }
    }
}
