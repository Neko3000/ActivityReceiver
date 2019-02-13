using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Data;
using ActivityReceiver.Models;
using ActivityReceiver.Enums;

namespace ActivityReceiver.Functions
{
    public static class ParameterAnalyzer
    {
        
        // D&D Interval Average
        public static float CalculateDDIntervalAVG(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();

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
                if(movement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.DragGroupBegin)
                {
                    totalTime += (movement.Time - previousTapEndTime);
                    count++;
                }
                else if(movement.State == (int)MovementState.DragSingleMove || movement.State == (int)MovementState.DragGroupMove)
                {
                    continue;
                }
                else if(movement.State == (int)MovementState.DragSingleEnd || movement.State == (int)MovementState.DragGroupEnd)
                {
                    previousTapEndTime = movement.Time;
                }
            }

            return (float)totalTime / count;
        }

        // D&D Interval MAX
        public static float CalculateDDIntervalMAX(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();

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
                if (movement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.DragGroupBegin)
                {
                    differTime = movement.Time - previousTapEndTime;
                    if (differTime > differTimeMAX)
                    {
                        differTimeMAX = differTime;
                    }
                }
                else if (movement.State == (int)MovementState.DragSingleMove || movement.State == (int)MovementState.DragGroupMove)
                {
                    continue;
                }
                else if (movement.State == (int)MovementState.DragSingleEnd || movement.State == (int)MovementState.DragGroupEnd)
                {
                    previousTapEndTime = movement.Time;
                }
            }

            return (float)differTimeMAX;
        }

        // D&D Interval MIN
        public static float CalculateDDIntervalMIN(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();

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
                if (movement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.DragGroupBegin)
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
                else if (movement.State == (int)MovementState.DragSingleMove || movement.State == (int)MovementState.DragGroupMove)
                {
                    continue;
                }
                else if (movement.State == (int)MovementState.DragSingleEnd || movement.State == (int)MovementState.DragGroupEnd)
                {
                    previousTapEndTime = movement.Time;
                }
            }

            return (float)differTimeMIN;
        }

        // D&D Process Average
        public static float CalculateDDProcessAVG(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();

            int previousTapBeginTime = 0;
            int totalTime = 0;
            int count = 0;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];
                if (movement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.DragGroupBegin)
                {
                    previousTapBeginTime = movement.Time;
                    
                }
                else if (movement.State == (int)MovementState.DragSingleMove || movement.State == (int)MovementState.DragGroupMove)
                {
                    continue;
                }
                else if (movement.State == (int)MovementState.DragSingleEnd || movement.State == (int)MovementState.DragGroupEnd)
                {
                    totalTime += (movement.Time - previousTapBeginTime);
                    count++;
                }
            }

            return (float)totalTime / count;
        }

        // D&D Process MAX
        public static float CalculateDDProcessMAX(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();

            int differTime;
            int differTimeMAX = 0;
            int previousTapBeginTime = 0;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];
                if (movement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.DragGroupBegin)
                {
                    previousTapBeginTime = movement.Time;
                }
                else if (movement.State == (int)MovementState.DragSingleMove || movement.State == (int)MovementState.DragGroupMove)
                {
                    continue;
                }
                else if (movement.State == (int)MovementState.DragSingleEnd || movement.State == (int)MovementState.DragGroupEnd)
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

        // D&D Process MIN
        public static float CalculateDDProcessMIN(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();

            int differTime;
            int differTimeMIN = 0;
            int previousTapBeginTime = 0;
            bool isFirstCalculatedDDInterval = true;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];
                if (movement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.DragGroupBegin)
                {
                    previousTapBeginTime = movement.Time;
                }
                else if (movement.State == (int)MovementState.DragSingleMove || movement.State == (int)MovementState.DragGroupMove)
                {
                    continue;
                }
                else if (movement.State == (int)MovementState.DragSingleEnd || movement.State == (int)MovementState.DragGroupEnd)
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
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();

            Movement lastMovement = movementCollection.FirstOrDefault();
            float distance;
            float totalDistance = 0;    
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                if (movement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.MakeGroupBegin || movement.State == (int)MovementState.DragGroupBegin)
                {
                    lastMovement = movement;
                }
                else if (movement.State == (int)MovementState.DragSingleMove || movement.State == (int)MovementState.DragSingleEnd || movement.State == (int)MovementState.MakeGroupMove || movement.State == (int)MovementState.MakeGroupEnd || movement.State == (int)MovementState.DragGroupMove || movement.State == (int)MovementState.DragSingleEnd)
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
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();

            Movement lastMovement = movementCollection.FirstOrDefault();
            float distance;
            float DDSpeedTotal = 0;
            int count = 0;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                if (movement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.MakeGroupBegin || movement.State == (int)MovementState.DragGroupBegin)
                {
                    lastMovement = movement;
                }
                else if (movement.State == (int)MovementState.DragSingleMove || movement.State == (int)MovementState.DragSingleEnd || movement.State == (int)MovementState.MakeGroupMove || movement.State == (int)MovementState.MakeGroupEnd || movement.State == (int)MovementState.DragGroupMove || movement.State == (int)MovementState.DragSingleEnd)
                {
                    if (movement.Time == lastMovement.Time)
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
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();

            Movement lastMovement = movementCollection.FirstOrDefault();
            float distance;
            float DDSpeed = 0;
            float DDSpeedMAX = 0;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                if (movement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.MakeGroupBegin || movement.State == (int)MovementState.DragGroupBegin)
                {
                    lastMovement = movement;
                }
                else if (movement.State == (int)MovementState.DragSingleMove || movement.State == (int)MovementState.DragSingleEnd || movement.State == (int)MovementState.MakeGroupMove || movement.State == (int)MovementState.MakeGroupEnd || movement.State == (int)MovementState.DragGroupMove || movement.State == (int)MovementState.DragSingleEnd)
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
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();

            Movement lastMovement = movementCollection.FirstOrDefault();
            float distance;
            float DDSpeed = 0;
            float DDSpeedMIN = 0;
            bool isFirstCalculatedDDSpeed = true;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                if (movement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.MakeGroupBegin || movement.State == (int)MovementState.DragGroupBegin)
                {
                    lastMovement = movement;
                }
                else if (movement.State == (int)MovementState.DragSingleMove || movement.State == (int)MovementState.DragSingleEnd || movement.State == (int)MovementState.MakeGroupMove || movement.State == (int)MovementState.MakeGroupEnd || movement.State == (int)MovementState.DragGroupMove || movement.State == (int)MovementState.DragSingleEnd)
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
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();

            Movement movement = movementCollection.FirstOrDefault();

            return movement == null ? 0 : movement.Time;
        }

        // DD Count
        public static int CalculateDDCount(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();
            Movement movement = movementCollection.FirstOrDefault();

            int count = 0;
            bool isOperating = false;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                movement = movementCollection[i];

                if (movement.State == (int)MovementState.DragSingleBegin ||  movement.State == (int)MovementState.DragGroupBegin)
                {
                    isOperating = true;
                }
                else if (movement.State == (int)MovementState.DragSingleMove || movement.State == (int)MovementState.DragGroupMove)
                {
                    continue;
                }
                else if(movement.State == (int)MovementState.DragSingleEnd || movement.State == (int)MovementState.DragSingleEnd)
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
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();
            Movement lastMovement = movementCollection.FirstOrDefault();

            int count = 0;
            bool isToPositiveDirection = true;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                if (movement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.DragGroupBegin)
                {
                    lastMovement = movement;
                }
                else if (movement.State == (int)MovementState.DragSingleMove || movement.State == (int)MovementState.DragSingleEnd || movement.State == (int)MovementState.DragGroupMove || movement.State == (int)MovementState.DragGroupEnd)
                {
                    if(lastMovement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.DragGroupBegin)
                    {
                        isToPositiveDirection = movement.XPosition - lastMovement.XPosition > 0 ? true : false;
                        lastMovement = movement;
                        continue;
                    }

                    if(movement.XPosition - lastMovement.XPosition > 0  && !isToPositiveDirection)
                    {
                        count++;
                        isToPositiveDirection = false;
                        lastMovement = movement;
                    }
                    else if(movement.XPosition - lastMovement.XPosition < 0 && isToPositiveDirection)
                    {
                        count++;
                        isToPositiveDirection = true;
                        lastMovement = movement;
                    }
                    else if(movement.XPosition - lastMovement.XPosition == 0)
                    {
                        lastMovement = movement;
                    }
                }
            }

            return count;
        }

        // U Turn Vertical Count
        public static int CalculateUTurnVerticalCount(IList<Movement> movementCollection)
        {
            movementCollection = movementCollection.OrderBy(m => m.Time).ToList();
            Movement lastMovement = movementCollection.FirstOrDefault();

            int count = 0;
            bool isToPositiveDirection = true;
            for (int i = 0; i < movementCollection.Count; i++)
            {
                var movement = movementCollection[i];

                if (movement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.DragGroupBegin)
                {
                    lastMovement = movement;
                }
                else if (movement.State == (int)MovementState.DragSingleMove || movement.State == (int)MovementState.DragSingleEnd || movement.State == (int)MovementState.DragGroupMove || movement.State == (int)MovementState.DragGroupEnd)
                {
                    if (lastMovement.State == (int)MovementState.DragSingleBegin || movement.State == (int)MovementState.DragGroupBegin)
                    {
                        isToPositiveDirection = movement.YPosition - lastMovement.YPosition > 0 ? true : false;
                        lastMovement = movement;
                        continue;
                    }

                    if (movement.YPosition - lastMovement.YPosition > 0 && !isToPositiveDirection)
                    {
                        count++;
                        isToPositiveDirection = false;
                        lastMovement = movement;
                    }
                    else if (movement.YPosition - lastMovement.YPosition < 0 && isToPositiveDirection)
                    {
                        count++;
                        isToPositiveDirection = true;
                        lastMovement = movement;
                    }
                    else if (movement.YPosition - lastMovement.YPosition == 0)
                    {
                        lastMovement = movement;
                    }
                }
            }

            return count;
        }
    }
}
