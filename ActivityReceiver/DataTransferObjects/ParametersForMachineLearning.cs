using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.DataTransferObjects
{
    public class ParametersForMachineLearning
    {
        public string User { get; set; }

        public int Question { get; set; }

        public int Understand { get; set; }

        public DateTime Date { get; set; }

        // Leave it be
        public int Check { get; set; }

        public int Time { get; set; }

        public float Distance { get; set; }

        public float AverageSpeed { get; set; }

        public float MaxSpeed { get; set; }

        // These two are trouble
        public float ThinkingTime { get; set; }

        public float AnsweringTime { get; set; }

        // Stops are gone = 0
        public float TotalStopTime { get; set; }

        public float MaxStopTime { get; set; }

        public float TotalDDIntervalTime { get; set; }

        public float MaxDDIntervalTime { get; set; }

        public float MaxDDTime { get; set; }

        public float MinDDTime { get; set; }

        public int DDCount { get; set; }

        public float GroupingDDCount { get; set; }

        public int XUTurnCount { get; set; }

        public int YUTurnCount { get; set; }     
    }
}
