using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.Filters
{
    public class MovingAverage
    {
        private int windowSize = 10;

        private Queue<float> samples = new Queue<float>();
        private float sampleAccumulator = 0;

        public MovingAverage(int windowSize)
        {
            this.windowSize = windowSize;
        }

        /// <summary>
        /// Computes a new windowed average each time a new sample arrives
        /// </summary>
        /// <param name="newSample"></param>
        public float ComputeAverage(float newSample)
        {
            sampleAccumulator += newSample;
            samples.Enqueue(newSample);

            if (samples.Count > windowSize)
            {
                sampleAccumulator -= samples.Dequeue();
            }

            float Average = sampleAccumulator / samples.Count;

            return Average;
        }
    }
}
