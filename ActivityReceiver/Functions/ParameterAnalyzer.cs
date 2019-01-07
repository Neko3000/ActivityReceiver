﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Data;
using ActivityReceiver.Models;

namespace ActivityReceiver.Functions
{
    public class ParameterAnalyzer
    {
        private readonly ActivityReceiverDbContext _arDbContext;

        public ParameterAnalyzer(ActivityReceiverDbContext arDbContext)
        {
            _arDbContext = arDbContext;
        }
        
        // D&D Interval Average
        public static float CalculateDDIntervalAVG(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();

            int previousTapEndTime = 0;
            int totalTime = 0;
            int count = 0;
            for(int i=0;i<movementCollection.Count;i++)
            {
                if(i==0)
                {
                    continue;
                }

                var movement = movementCollection[i];
                if(movement.State == 0)
                {
                    totalTime += (movement.Time - previousTapEndTime);
                    count++;
                }
                else if(movement.State == 1)
                {
                    continue;
                }
                else if(movement.State == 2)
                {
                    previousTapEndTime = movement.Time;
                }
            }

            return (float)totalTime / count;
        }

        // D&D Interval Max
        public static float CalculateDDIntervalMAX(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();

            int differTime;
            int differTimeMAX = 0;
            int previousTapEndTime = 0;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                var movement = movementCollection[i];
                if (movement.State == 0)
                {
                    differTime = movement.Time - previousTapEndTime;
                    if (differTime > differTimeMAX)
                    {
                        differTimeMAX = differTime;
                    }
                }
                else if (movement.State == 1)
                {
                    continue;
                }
                else if (movement.State == 2)
                {
                    previousTapEndTime = movement.Time;
                }
            }

            return (float)differTimeMAX;
        }

        // D&D Interval Min
        public static float CalculateDDIntervalMIN(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();

            int differTime;
            int differTimeMIN = 0;
            int previousTapEndTime = 0;
            bool isFirstCalculatedDDInterval = true;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                var movement = movementCollection[i];
                if (movement.State == 0)
                {
                    differTime = movement.Time - previousTapEndTime;
                    if(isFirstCalculatedDDInterval)
                    {
                        differTimeMIN = differTime;
                        isFirstCalculatedDDInterval = false;
                    }
                    else
                    {
                        if (differTime < differTimeMIN)
                        {
                            differTimeMIN = differTime;
                        }
                    }

                }
                else if (movement.State == 1)
                {
                    continue;
                }
                else if (movement.State == 2)
                {
                    previousTapEndTime = movement.Time;
                }
            }

            return (float)differTimeMIN;
        }

        // D&D Process Average
        public static float CalculateDDProcessAVG(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();

            int previousTapBeginTime = 0;
            int totalTime = 0;
            int count = 0;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];
                if (movement.State == 0)
                {
                    previousTapBeginTime = movement.Time;
                    
                }
                else if (movement.State == 1)
                {
                    continue;
                }
                else if (movement.State == 2)
                {
                    totalTime += (movement.Time - previousTapBeginTime);
                    count++;
                }
            }

            return (float)totalTime / count;
        }

        // D&D Process Max
        public static float CalculateDDProcessMAX(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();

            int differTime;
            int differTimeMAX = 0;
            int previousTapBeginTime = 0;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];
                if (movement.State == 0)
                {
                    previousTapBeginTime = movement.Time;
                }
                else if (movement.State == 1)
                {
                    continue;
                }
                else if (movement.State == 2)
                {
                    differTime = movement.Time - previousTapBeginTime;
                    if (differTime > differTimeMAX)
                    {
                        differTimeMAX = differTime;
                    }
                }
            }

            return (float)differTimeMAX;
        }

        // D&D Process Min
        public static float CalculateDDProcessMIN(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();

            int differTime;
            int differTimeMIN = 0;
            int previousTapBeginTime = 0;
            bool isFirstCalculatedDDInterval = true;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];
                if (movement.State == 0)
                {
                    previousTapBeginTime = movement.Time;
                }
                else if (movement.State == 1)
                {
                    continue;
                }
                else if (movement.State == 2)
                {
                    differTime = movement.Time - previousTapBeginTime;
                    if (isFirstCalculatedDDInterval)
                    {
                        differTimeMIN = differTime;
                        isFirstCalculatedDDInterval = false;
                    }
                    else
                    {
                        if (differTime < differTimeMIN)
                        {
                            differTimeMIN = differTime;
                        }
                    }
                }
            }

            return (float)differTimeMIN;
        }

        // Total Distance
        public static float CalculateTotalDistance(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();

            Movement lastMovement = movementCollection.FirstOrDefault();
            float distance;
            float totalDistance = 0;    
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                if (movement.State == 0)
                {
                    lastMovement = movementCollection[i];
                }
                else if (movement.State == 1 || movement.State == 2)
                {
                    distance = (float)(Math.Sqrt(Math.Pow(lastMovement.XPosition - movement.XPosition, 2) + Math.Pow(lastMovement.YPosition - movement.YPosition, 2)));
                    totalDistance += distance;

                    lastMovement = movement;
                }
            }

            return (float)Math.Round(totalDistance,2);
        }

        // DD Speed Average
        public static float CalculateDDSpeedAVG(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();

            Movement lastMovement = movementCollection.FirstOrDefault();
            float distance;
            float DDSpeedTotal = 0;
            int count = 0;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                if (movement.State == 0)
                {
                    lastMovement = movement;
                }
                else if (movement.State == 1 || movement.State == 2)
                {
                    if(movement.Time == lastMovement.Time)
                    {
                        lastMovement = movement;
                        continue;
                    }

                    distance = (float)(Math.Sqrt(Math.Pow(lastMovement.XPosition - movement.XPosition, 2) + Math.Pow(lastMovement.YPosition - movement.YPosition, 2)));
                    DDSpeedTotal += (float)(distance / ((movement.Time - lastMovement.Time) / 1000.0));
                    count++;

                    lastMovement = movement;
                }
            }

            return (float)Math.Round(DDSpeedTotal/count,2);
        }

        // DD Speed MAX
        public static float CalculateDDSpeedMAX(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();

            Movement lastMovement = movementCollection.FirstOrDefault();
            float distance;
            float DDSpeed = 0;
            float DDSpeedMAX = 0;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                if (movement.State == 0)
                {
                    lastMovement = movement;
                }
                else if (movement.State == 1 || movement.State == 2)
                {
                    if (movement.Time == lastMovement.Time)
                    {
                        lastMovement = movement;
                        continue;
                    }

                    distance = (float)(Math.Sqrt(Math.Pow(lastMovement.XPosition - movement.XPosition, 2) + Math.Pow(lastMovement.YPosition - movement.YPosition, 2)));
                    DDSpeed = (float)(distance / ((movement.Time - lastMovement.Time) / 1000.0));
                    if(DDSpeed > DDSpeedMAX)
                    {
                        DDSpeedMAX = DDSpeed;
                    }

                    lastMovement = movement;
                }
            }

            return (float)Math.Round(DDSpeedMAX, 2);
        }

        // DD Speed MIN
        public static float CalculateDDSpeedMIN(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();

            Movement lastMovement = movementCollection.FirstOrDefault();
            float distance;
            float DDSpeed = 0;
            float DDSpeedMIN = 0;
            bool isFirstCalculatedDDSpeed = true;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                if (movement.State == 0)
                {
                    lastMovement = movement;
                }
                else if (movement.State == 1 || movement.State == 2)
                {
                    if (movement.Time == lastMovement.Time)
                    {
                        lastMovement = movement;
                        continue;
                    }

                    distance = (float)(Math.Sqrt(Math.Pow(lastMovement.XPosition - movement.XPosition, 2) + Math.Pow(lastMovement.YPosition - movement.YPosition, 2)));
                    DDSpeed = (float)(distance / ((movement.Time - lastMovement.Time) / 1000.0));

                    if(isFirstCalculatedDDSpeed)
                    {
                        DDSpeedMIN = DDSpeed;
                        isFirstCalculatedDDSpeed = false;
                    }
                    else
                    {
                        if(DDSpeed < DDSpeedMIN)
                        {
                            DDSpeedMIN = DDSpeed;
                        }
                    }

                    lastMovement = movement;
                }
            }

            return (float)Math.Round(DDSpeedMIN, 2);
        }

        // DD First Time
        public static float CalculateDDFirstTime(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();

            Movement movement = movementCollection.FirstOrDefault();

            return movement == null ? 0 : movement.Time;
        }

        // DD First Time
        public static int CalculateDDCount(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();
            Movement movement = movementCollection.FirstOrDefault();

            int count = 0;
            bool isOperating = false;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                movement = movementCollection[i];

                if (movement.State == 0)
                {
                    isOperating = true;
                }
                else if(movement.State == 1)
                {
                    continue;
                }
                else if(movement.State == 2)
                {
                    if(isOperating)
                    {
                        count++;
                        isOperating = false;
                    }
                }
            }

            return count;
        }

        // U Turn Horizontal Count
        public static int CalculateUTurnHorizontalCount(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();
            Movement lastMovement = movementCollection.FirstOrDefault();

            int count = 0;
            bool isToPositiveDirection = true;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                if(movement.State == 0)
                {
                    lastMovement = movement;
                }
                else if (movement.State == 1 || movement.State == 2)
                {
                    if(lastMovement.State == 0)
                    {
                        isToPositiveDirection = movement.XPosition - movement.XPosition > 0 ? true : false;
                        continue;
                    }

                    if(movement.XPosition - lastMovement.XPosition > 0  && !isToPositiveDirection)
                    {
                        count++;
                        isToPositiveDirection = false;
                    }
                    else if(movement.XPosition - lastMovement.XPosition < 0 && isToPositiveDirection)
                    {
                        count++;
                        isToPositiveDirection = true;
                    }
                }
            }

            return count;
        }

        // U Turn Vertical Count
        public static int CalculateUTurnVerticalCount(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(mc => mc.Time).ToList();
            Movement lastMovement = movementCollection.FirstOrDefault();

            int count = 0;
            bool isToPositiveDirection = true;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                if(movement.State == 0)
                {
                    lastMovement = movement;
                }
                else if (movement.State == 1 || movement.State == 2)
                {
                    if (lastMovement.State == 0)
                    {
                        isToPositiveDirection = movement.YPosition - movement.YPosition > 0 ? true : false;
                        continue;
                    }

                    if (movement.YPosition - lastMovement.YPosition > 0 && !isToPositiveDirection)
                    {
                        count++;
                        isToPositiveDirection = false;
                    }
                    else if (movement.YPosition - lastMovement.YPosition < 0 && isToPositiveDirection)
                    {
                        count++;
                        isToPositiveDirection = true;
                    }
                }
            }

            return count;
        }
    }
}
