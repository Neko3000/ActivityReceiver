using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Functions;
using ActivityReceiver.Models;
using ActivityReceiver.DataTransferObjects;
using ActivityReceiver.Filters;
using ActivityReceiver.Enums;

namespace ActivityReceiver.Functions
{
    public class MovementSupervisor
    {
        private float ThACC;
        private float ThFOC;

        // ms
        private int offsetForTime = 500;

        private readonly IList<Movement> movementCollection;
        private readonly IList<DeviceAcceleration> deviceAccelerationCollection;

        public MovementSupervisor(IList<Movement> movementCollection, IList<DeviceAcceleration> deviceAccelerationCollection, float ThACC = 0.1f, float ThFOC = 0.8f)
        {
            this.movementCollection = movementCollection;
            this.deviceAccelerationCollection = deviceAccelerationCollection;

            this.ThACC = ThACC;
            this.ThFOC = ThFOC;
        }

        private int FindMovementIndexByTime(List<MovementSupervised> movementSupervisedCollection, int time)
        {
            var movementSupervisedLocated = movementSupervisedCollection.FirstOrDefault();

            int index;
            for (index = 0; index < movementSupervisedCollection.Count; index++)
            {
                var movementSupervised = movementSupervisedCollection[index];
                if (time < movementSupervisedCollection[index].Time)
                {
                    break;
                }
            }

            return index;
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

                var movementSupervised = new MovementSupervised
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
                    var selectedMovementSupervisedIndex = FindMovementIndexByTime(movementSupervisedCollection, deviceAccelerationCombinedFiltered.Time);
                    var movementSupervised = movementSupervisedCollection[selectedMovementSupervisedIndex];
                    if (movementSupervised.Force >= ThFOC)
                    {
                        movementSupervised.IsAbnormal = true;
                    }
                }
            }
            return movementSupervisedCollection;
        }

        public IList<MovementSupervised> SuperviseByAcceleration()
        {
            var deviceAccelerationCombinedCollection = CombineDeviceAcceleration(deviceAccelerationCollection);
            var deviceAccelerationCombinedFilteredCollection = FilterDeviceAccelerationCombinedCollection(deviceAccelerationCombinedCollection);

            // Build movementSupervised list
            var movementSupervisedCollection = new List<MovementSupervised>();
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                var movementSupervised = new MovementSupervised
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
                    var selectedMovementSupervisedIndex = FindMovementIndexByTime(movementSupervisedCollection, deviceAccelerationCombinedFiltered.Time);
                    var movementSupervised = movementSupervisedCollection[selectedMovementSupervisedIndex];

                    movementSupervised.IsAbnormal = true;
                }
            }
            return movementSupervisedCollection;
        }

        public IList<MovementSupervised> SuperviseByAccelerationAndForce()
        {
            var deviceAccelerationCombinedCollection = CombineDeviceAcceleration(deviceAccelerationCollection);
            var deviceAccelerationCombinedFilteredCollection = FilterDeviceAccelerationCombinedCollection(deviceAccelerationCombinedCollection);

            // Build movementSupervised list
            var movementSupervisedCollection = new List<MovementSupervised>();
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                var movementSupervised = new MovementSupervised
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
                    var selectedMovementSupervisedIndex = FindMovementIndexByTime(movementSupervisedCollection, deviceAccelerationCombinedFiltered.Time);
                    var movementSupervised = movementSupervisedCollection[selectedMovementSupervisedIndex];

                    if (movementSupervised.Force > ThFOC)
                    {
                        movementSupervised.IsAbnormal = true;
                    }
                }
            }
            return movementSupervisedCollection;
        }

        public static IList<DeviceAccelerationCombined> CombineDeviceAcceleration(IList<DeviceAcceleration> deviceAccelerationCollection)
        {
            var deviceAccelerationCombinedCollection = new List<DeviceAccelerationCombined>();
            for (int i = 0; i < deviceAccelerationCollection.Count; i++)
            {
                var deviceAcceleration = deviceAccelerationCollection[i];

                var deviceAccelerationCombined = new DeviceAccelerationCombined
                {
                    ID = deviceAcceleration.ID,
                    AnswerRecordID = deviceAcceleration.ID,
                    Index = deviceAcceleration.Index,
                    Time = deviceAcceleration.Time,

                    Acceleration = (float)Math.Sqrt(Math.Pow(deviceAcceleration.X, 2.0) + Math.Pow(deviceAcceleration.Y, 2.0) + Math.Pow(deviceAcceleration.Z, 2.0)),
                };

                deviceAccelerationCombinedCollection.Add(deviceAccelerationCombined);
            }
            return deviceAccelerationCombinedCollection;
        }

        public static IList<DeviceAccelerationCombined> FilterDeviceAccelerationCombinedCollection(IList<DeviceAccelerationCombined> deviceAccelerationCombinedCollection, int windowSize = 20)
        {
            var deviceAccelerationCombinedFilteredCollection = new List<DeviceAccelerationCombined>();

            // The windows default size is 20
            var movingFilter = new MovingAverage(windowSize);

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

        public static IList<Movement> FilterMovementForceCollection(IList<Movement> movementCollection, int windowSize = 20)
        {
            var movementFilteredCollection = new List<Movement>();

            // The windows default size is 20
            var movingFilter = new MovingAverage(windowSize);

            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                var movementFiltered = new Movement
                {
                    ID = movement.ID,
                    AnswerRecordID = movement.AnswerRecordID,
                    Index = movement.Index,
                    Time = movement.Time,
                    State = movement.State,
                    TargetElement = movement.TargetElement,
                    XPosition = movement.XPosition,
                    YPosition = movement.YPosition,

                    Force = movingFilter.ComputeAverage(movement.Force)
                };

                movementFilteredCollection.Add(movementFiltered);
            }
            return movementFilteredCollection;
        }

        // DD Count
        public static int CalculateAbnormalTrajectoryCount(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();
            MovementSupervised movementSupervised = movementSupervisedCollection.FirstOrDefault();

            int count = 0;
            bool isAbnormal = false;

            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                movementSupervised = movementSupervisedCollection[i];

                if (movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.DragGroupBegin || movementSupervised.State == (int)MovementState.MakeGroupBegin)
                {
                    isAbnormal = false;             
                }

                if (movementSupervised.IsAbnormal&&isAbnormal == false)
                {
                    isAbnormal = true;
                    count++;
                }
                else if(!movementSupervised.IsAbnormal && isAbnormal == true)
                {
                    isAbnormal = false;
                }
            }

            return count;
        }

    }
}
