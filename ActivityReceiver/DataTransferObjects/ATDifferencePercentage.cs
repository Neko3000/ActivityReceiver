using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.DataTransferObjects
{
    public class ATDifferencePercentage
    {
        public string Username { get; set; }

        public float DDIntervalAVGDifferencePercentage { get; set; }

        public float DDIntervalMAXDifferencePercentage { get; set; }

        public float DDIntervalMINDifferencePercentage { get; set; }

        public float DDProcessAVGDifferencePercentage { get; set; }

        public float DDProcessMAXDifferencePercentage { get; set; }

        public float DDProcessMINDifferencePercentage { get; set; }

        public float TotalDistanceDifferencePercentage { get; set; }

        public float DDSpeedAVGDifferencePercentage { get; set; }

        public float DDSpeedMAXDifferencePercentage { get; set; }

        public float DDSpeedMINDifferencePercentage { get; set; }

        public float DDFirstTimeDifferencePercentage { get; set; }

        public float DDCountDifferencePercentage { get; set; }

        public float UTurnHorizontalCountDifferencePercentage { get; set; }

        public float UTurnVerticalCountDifferencePercentage { get; set; }

        public int AbnormalTrajectoryCount { get; set; }

        public int AbnormalTrajectoryPointCount { get; set; }

        public float AbnormalTrajectoryPointPercentage { get; set; }
    }
}
