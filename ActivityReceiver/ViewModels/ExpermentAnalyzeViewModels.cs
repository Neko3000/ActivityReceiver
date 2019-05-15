using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.DataTransferObjects;

namespace ActivityReceiver.ViewModels.ExperimentAnalyze
{
    public class AbnormalTrajectoryViewModel
    {
        public IList<AbnormalTrajectoryEvaluation> AbnormalTrajectoryEvaluationCollection { get; set; }
    }

    public class ForceAverageViewModel
    {
        public IList<ForceAverageForUser> ForceAverageForUserCollection { get; set; }
    }
}
