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
}
