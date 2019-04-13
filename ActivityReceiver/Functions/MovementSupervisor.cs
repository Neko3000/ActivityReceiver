using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Functions;
using ActivityReceiver.Models;
using ActivityReceiver.DataTransferObjects;
using ActivityReceiver.Filters;

namespace ActivityReceiver.Functions
{
    public class MovementSupervisor
    {
        private float Threshold = 0.5f;

        private IList<Movement> movementCollection;
        private IList<DeviceAcceleration> deviceAccelerationCollection;

        public MovementSupervisor(IList<Movement> movementCollection, IList<DeviceAcceleration> deviceAccelerationCollection)
        {
            this.movementCollection = movementCollection;
            this.deviceAccelerationCollection = deviceAccelerationCollection;
        }

        public IList<MovementSupervised> Supervise()
        {
            var deviceAccelerationCombinedCollection = CombineDeviceAcceleration(deviceAccelerationCollection);
            var deviceAccelerationCombinedFilteredCollection = FilterDeviceAccelerationCombinedCollection(deviceAccelerationCombinedCollection);

            return null;
        }

        public static IList<DeviceAccelerationCombined> CombineDeviceAcceleration(IList<DeviceAcceleration> deviceAccelerationCollection)
        {
            var deviceAccelerationCombinedCollection = new List<DeviceAccelerationCombined>();
            for(int i = 0; i< deviceAccelerationCollection.Count; i++)
            {
                var deviceAcceleration = deviceAccelerationCollection[i];

                var deviceAccelerationCombined = new DeviceAccelerationCombined
                {
                    ID = deviceAcceleration.ID,
                    AnswerRecordID = deviceAcceleration.ID,
                    Index = deviceAcceleration.Index,
                    Time = deviceAcceleration.Time,

                    Acceleration = (float)Math.Sqrt(Math.Pow(deviceAcceleration.X,2.0) + Math.Pow(deviceAcceleration.Y, 2.0) + Math.Pow(deviceAcceleration.Z, 2.0)),
                };

                deviceAccelerationCombinedCollection.Add(deviceAccelerationCombined);
            }
            return deviceAccelerationCombinedCollection;
        }

        public static IList<DeviceAccelerationCombined> FilterDeviceAccelerationCombinedCollection(IList<DeviceAccelerationCombined> deviceAccelerationCombinedCollection)
        {
            var deviceAccelerationCombinedFilteredCollection = new List<DeviceAccelerationCombined>();

            // The windows size is 20
            var movingFilter = new MovingAverage(20);

            for (int i = 0; i < deviceAccelerationCombinedCollection.Count; i++)
            {
                var deviceAccelerationCombined = deviceAccelerationCombinedCollection[i];

                var deviceAccelerationCombinedFiltered = new DeviceAccelerationCombined
                {
                    ID = deviceAccelerationCombined.ID,
                    AnswerRecordID = deviceAccelerationCombined.ID,
                    Index = deviceAccelerationCombined.Index,
                    Time = deviceAccelerationCombined.Time,

                    Acceleration = movingFilter.ComputeAverage(deviceAccelerationCombined.Acceleration)
                };

                deviceAccelerationCombinedFilteredCollection.Add(deviceAccelerationCombinedFiltered);
            }
            return deviceAccelerationCombinedFilteredCollection;
        }
    }
}
