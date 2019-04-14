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
        private readonly float ThACC = 0.1f;

        private readonly IList<Movement> movementCollection;
        private readonly IList<DeviceAcceleration> deviceAccelerationCollection;

        public MovementSupervisor(IList<Movement> movementCollection, IList<DeviceAcceleration> deviceAccelerationCollection)
        {
            this.movementCollection = movementCollection;
            this.deviceAccelerationCollection = deviceAccelerationCollection;
        }

        private void SetMovementSupervisedToAbnormalByTime(ref List<MovementSupervised> movementSupervisedCollection, int time)
        {
            var movementSupervisedLocated = movementSupervisedCollection.FirstOrDefault();
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                var movementSupervised = movementSupervisedCollection[i];
                if (time < movementSupervisedCollection[i].Time)
                {
                    break;
                }
                movementSupervisedLocated = movementSupervised;
            }
            movementSupervisedLocated.IsAbnormal = true;
        }

        public IList<MovementSupervised> Supervise()
        {
            var deviceAccelerationCombinedCollection = CombineDeviceAcceleration(deviceAccelerationCollection);
            var deviceAccelerationCombinedFilteredCollection = FilterDeviceAccelerationCombinedCollection(deviceAccelerationCombinedCollection);

            // Build movementSupervised list
            var movementSupervisedCollection = new List<MovementSupervised>();
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                var movementSupervised= new MovementSupervised
                {
                    ID = movement.ID,
                    AnswerRecordID = movement.AnswerRecordID,
                    Index = movement.Index,
                    Time = movement.Time,
                    State = movement.State,
                    TargetElement = movement.TargetElement,
                    XPosition = movement.XPosition,
                    YPosition = movement.YPosition,
                    Force = movement.Force,

                    IsAbnormal = false,
                   };

                movementSupervisedCollection.Add(movementSupervised);
            }

            for (int i = 0; i < deviceAccelerationCombinedFilteredCollection.Count; i++)
            {
                var deviceAccelerationCombinedFiltered = deviceAccelerationCombinedFilteredCollection[i];
                if (deviceAccelerationCombinedFiltered.Acceleration >= ThACC)
                {
                    SetMovementSupervisedToAbnormalByTime(ref movementSupervisedCollection, deviceAccelerationCombinedFiltered.Time);
                }
            }


            return movementSupervisedCollection;
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
