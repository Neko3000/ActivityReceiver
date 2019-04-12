using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Functions;
using ActivityReceiver.Models;
using ActivityReceiver.DataTransferObjects;


namespace ActivityReceiver.Functions
{
    public static class MovementSupervisor
    {
        private static float Threshold = 0.5f;

        public static IList<MovementSupervised> Supervise(IList<Movement> movementCollection,IList<DeviceAcceleration> deviceAccelerationCollection)
        {

            return null;
        }
    }
}
