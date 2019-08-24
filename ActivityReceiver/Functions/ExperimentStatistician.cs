using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Models;
using ActivityReceiver.DataTransferObjects;
using ActivityReceiver.Functions;
using ActivityReceiver.Filters;

namespace ActivityReceiver.Functions
{
    public class ExperimentStatistician
    {
        float[] timingArray = { 8, 15, 22, 29, 36, 60, 70, 80, 90, 100 };
        float duration = 5.0f;

        public float CalculatePrecision(IList<MovementSupervised> movementSupervisedCollection)
        {
            int abnormalMarkedCount = 0;
            int abnormalHitCount = 0;
            for(int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                var movementSupervised = movementSupervisedCollection[i];

                if (movementSupervised.IsAbnormal)
                {
                    abnormalMarkedCount++;

                    for (int j = 0; j < timingArray.Count(); j++)
                    {
                        var timing = timingArray[j];

                        if (movementSupervised.Time >= timing * 1000 && movementSupervised.Time < timing * 1000 + duration * 1000)
                        {
                            abnormalHitCount++;
                        }
                    }
                }
           
            }
            return abnormalMarkedCount == 0 ? 0 : (float)abnormalHitCount/ abnormalMarkedCount;
        }

        public float CalculateRecall(IList<MovementSupervised> movementSupervisedCollection)
        {
            int abnormalRealCount = 0;
            int abnormalHitCount = 0;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                var movementSupervised = movementSupervisedCollection[i];

                for (int j = 0; j < timingArray.Count(); j++)
                {
                    var timing = timingArray[j];
                    if (movementSupervised.Time >= timing * 1000 && movementSupervised.Time < timing * 1000 + duration * 1000)
                    {
                        abnormalRealCount++;

                        if (movementSupervised.IsAbnormal)
                        {
                            abnormalHitCount++;
                        }
                    }
                }

            }
            return abnormalRealCount == 0 ? 0 : (float) abnormalHitCount / abnormalRealCount;
        }

        public float CalculateFMeasure(IList<MovementSupervised> movementSupervisedCollection)
        {
            float precision = CalculatePrecision(movementSupervisedCollection);
            float recall = CalculateRecall(movementSupervisedCollection);

            return (precision == 0 && recall == 0) ? 0 : 2 * precision * recall / (precision + recall);
        }

        static public float CalculateDifferencePercentage(float x,float y)
        {
            return x == 0 ? 0 : (y - x) / x;
        }
    }
}
