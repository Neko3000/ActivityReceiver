using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Data;
using ActivityReceiver.Models;
using ActivityReceiver.Enums;
using ActivityReceiver.DataTransferObjects;

namespace ActivityReceiver.Functions
{
    public static class ParameterAnalyzerForMovementSupervised
    {
        // D&D Interval Total
        public static float CalculateDDIntervalTotal(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();

            int previousTapEndTime = 0;
            int totalTime = 0;
            int count = 0;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                var movementSupervised = movementSupervisedCollection[i];
                if (movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.DragGroupBegin)
                {
                    totalTime += (movementSupervised.Time - previousTapEndTime);
                    count++;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragGroupMove)
                {
                    continue;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.DragGroupEnd)
                {
                    previousTapEndTime = movementSupervised.Time;
                }
            }

            return totalTime;
        }

        // D&D Interval Average
        public static float CalculateDDIntervalAVG(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();

            int previousTapEndTime = 0;
            int totalTime = 0;
            int count = 0;
            for(int i=0;i<movementSupervisedCollection.Count;i++)
            {
                if(i==0)
                {
                    continue;
                }

                var movementSupervised = movementSupervisedCollection[i];
                if(movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.DragGroupBegin)
                {
                    totalTime += (movementSupervised.Time - previousTapEndTime);
                    count++;
                }
                else if(movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragGroupMove)
                {
                    continue;
                }
                else if(movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.DragGroupEnd)
                {
                    previousTapEndTime = movementSupervised.Time;
                }
            }

            return (float)totalTime / count;
        }

        // D&D Interval MAX
        public static float CalculateDDIntervalMAX(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();

            int differTime;
            int differTimeMAX = 0;
            int previousTapEndTime = 0;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                var movementSupervised = movementSupervisedCollection[i];
                if (movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.DragGroupBegin)
                {
                    differTime = movementSupervised.Time - previousTapEndTime;
                    if (differTime > differTimeMAX)
                    {
                        differTimeMAX = differTime;
                    }
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragGroupMove)
                {
                    continue;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.DragGroupEnd)
                {
                    previousTapEndTime = movementSupervised.Time;
                }
            }

            return (float)differTimeMAX;
        }

        // D&D Interval MIN
        public static float CalculateDDIntervalMIN(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();

            int differTime;
            int differTimeMIN = 0;
            int previousTapEndTime = 0;
            bool isFirstCalculatedDDInterval = true;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                var movementSupervised = movementSupervisedCollection[i];
                if (movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.DragGroupBegin)
                {
                    differTime = movementSupervised.Time - previousTapEndTime;
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
                else if (movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragGroupMove)
                {
                    continue;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.DragGroupEnd)
                {
                    previousTapEndTime = movementSupervised.Time;
                }
            }

            return (float)differTimeMIN;
        }

        // D&D Process Average
        public static float CalculateDDProcessAVG(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();

            MovementSupervised lastMovementSupervised = null;
            int totalTime = 0;
            int count = 0;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                var movementSupervised = movementSupervisedCollection[i];
                if (movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.DragGroupBegin)
                {
                    lastMovementSupervised = null;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragGroupMove)
                {
                    if (!movementSupervised.IsAbnormal || !lastMovementSupervised.IsAbnormal)
                    {
                        totalTime += (movementSupervised.Time - lastMovementSupervised.Time);
                    }
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.DragGroupEnd)
                {
                    if (!movementSupervised.IsAbnormal || !lastMovementSupervised.IsAbnormal)
                    {
                        totalTime += (movementSupervised.Time - lastMovementSupervised.Time);
                    }
                    count++;
                }

                lastMovementSupervised = movementSupervised;
            }

            return (float)totalTime / count;
        }

        // D&D Process MAX
        public static float CalculateDDProcessMAX(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();

            MovementSupervised lastMovementSupervised = null;
            int differTime = 0;
            int differTimeMAX = 0;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                var movementSupervised = movementSupervisedCollection[i];
                if (movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.DragGroupBegin)
                {
                    differTime = 0;
                    lastMovementSupervised = null;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragGroupMove)
                {
                    if (!movementSupervised.IsAbnormal || !lastMovementSupervised.IsAbnormal)
                    {
                        differTime += (movementSupervised.Time - lastMovementSupervised.Time);
                    }
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.DragGroupEnd)
                {
                    if (!movementSupervised.IsAbnormal || !lastMovementSupervised.IsAbnormal)
                    {
                        differTime += (movementSupervised.Time - lastMovementSupervised.Time);
                    }

                    if(differTime > differTimeMAX)
                    {
                        differTimeMAX = differTime;
                    }
                }

                lastMovementSupervised = movementSupervised;
            }

            return (float)differTimeMAX;
        }

        // D&D Process MIN
        public static float CalculateDDProcessMIN(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();

            MovementSupervised lastMovementSupervised = null;
            int differTime = 0;
            int differTimeMIN = 0;
            bool isFirstCalculatedDDProcess = true;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                var movementSupervised = movementSupervisedCollection[i];
                if (movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.DragGroupBegin)
                {
                    differTime = 0;
                    lastMovementSupervised = null;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragGroupMove)
                {
                    if (!movementSupervised.IsAbnormal || !lastMovementSupervised.IsAbnormal)
                    {
                        differTime += (movementSupervised.Time - lastMovementSupervised.Time);
                    }
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.DragGroupEnd)
                {
                    if (!movementSupervised.IsAbnormal || !lastMovementSupervised.IsAbnormal)
                    {
                        differTime += (movementSupervised.Time - lastMovementSupervised.Time);
                    }

                    if (isFirstCalculatedDDProcess)
                    {
                        differTimeMIN = differTime;
                        isFirstCalculatedDDProcess = false;
                    }
                    else
                    {
                        if (differTime < differTimeMIN)
                        {
                            differTimeMIN = differTime;
                        }
                    }
                }

                lastMovementSupervised = movementSupervised;
            }

            return (float)differTimeMIN;
        }

        // Total Distance
        public static float CalculateTotalDistance(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();

            MovementSupervised lastMovementSupervised = movementSupervisedCollection.FirstOrDefault();
            float distance;
            float totalDistance = 0;    
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                var movementSupervised = movementSupervisedCollection[i];

                if (movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.MakeGroupBegin || movementSupervised.State == (int)MovementState.DragGroupBegin)
                {
                    lastMovementSupervised = null;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.MakeGroupMove || movementSupervised.State == (int)MovementState.MakeGroupEnd || movementSupervised.State == (int)MovementState.DragGroupMove || movementSupervised.State == (int)MovementState.DragSingleEnd)
                {
                    if (!movementSupervised.IsAbnormal || !lastMovementSupervised.IsAbnormal)
                    {
                        distance = (float)(Math.Sqrt(Math.Pow(lastMovementSupervised.XPosition - movementSupervised.XPosition, 2) + Math.Pow(lastMovementSupervised.YPosition - movementSupervised.YPosition, 2)));
                        totalDistance += distance;
                    }
                }
                lastMovementSupervised = movementSupervised;
            }

            return (float)Math.Round(totalDistance,2);
        }

        // DD Speed Average
        public static float CalculateDDSpeedAVG(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();

            MovementSupervised lastMovementSupervised = movementSupervisedCollection.FirstOrDefault();
            float distance;
            float DDSpeedTotal = 0;
            int count = 0;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                var movementSupervised = movementSupervisedCollection[i];

                if (movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.MakeGroupBegin || movementSupervised.State == (int)MovementState.DragGroupBegin)
                {
                    lastMovementSupervised = movementSupervised;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.MakeGroupMove || movementSupervised.State == (int)MovementState.MakeGroupEnd || movementSupervised.State == (int)MovementState.DragGroupMove || movementSupervised.State == (int)MovementState.DragSingleEnd)
                {
                    if (movementSupervised.Time == lastMovementSupervised.Time)
                    {
                        lastMovementSupervised = movementSupervised;
                        continue;
                    }

                    if (!movementSupervised.IsAbnormal || !lastMovementSupervised.IsAbnormal)
                    {
                        distance = (float)(Math.Sqrt(Math.Pow(lastMovementSupervised.XPosition - movementSupervised.XPosition, 2) + Math.Pow(lastMovementSupervised.YPosition - movementSupervised.YPosition, 2)));
                        DDSpeedTotal += (float)(distance / ((movementSupervised.Time - lastMovementSupervised.Time) / 1000.0));
                        count++;
                    }

                    lastMovementSupervised = movementSupervised;
                }
            }

            return (float)Math.Round(DDSpeedTotal/count,2);
        }

        // DD Speed MAX
        public static float CalculateDDSpeedMAX(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();

            MovementSupervised lastMovementSupervised = movementSupervisedCollection.FirstOrDefault();
            float distance;
            float DDSpeed = 0;
            float DDSpeedMAX = 0;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                var movementSupervised = movementSupervisedCollection[i];

                if (movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.MakeGroupBegin || movementSupervised.State == (int)MovementState.DragGroupBegin)
                {
                    lastMovementSupervised = movementSupervised;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.MakeGroupMove || movementSupervised.State == (int)MovementState.MakeGroupEnd || movementSupervised.State == (int)MovementState.DragGroupMove || movementSupervised.State == (int)MovementState.DragSingleEnd)
                {
                    if (movementSupervised.Time == lastMovementSupervised.Time)
                    {
                        lastMovementSupervised = movementSupervised;
                        continue;
                    }

                    if (!movementSupervised.IsAbnormal || !lastMovementSupervised.IsAbnormal)
                    {
                        distance = (float)(Math.Sqrt(Math.Pow(lastMovementSupervised.XPosition - movementSupervised.XPosition, 2) + Math.Pow(lastMovementSupervised.YPosition - movementSupervised.YPosition, 2)));
                        DDSpeed = (float)(distance / ((movementSupervised.Time - lastMovementSupervised.Time) / 1000.0));
                        if (DDSpeed > DDSpeedMAX)
                        {
                            DDSpeedMAX = DDSpeed;
                        }
                    }

                    lastMovementSupervised = movementSupervised;
                }
            }

            return (float)Math.Round(DDSpeedMAX, 2);
        }

        // DD Speed MIN
        public static float CalculateDDSpeedMIN(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();

            MovementSupervised lastMovementSupervised = movementSupervisedCollection.FirstOrDefault();
            float distance;
            float DDSpeed = 0;
            float DDSpeedMIN = 0;
            bool isFirstCalculatedDDSpeed = true;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                var movementSupervised = movementSupervisedCollection[i];

                if (movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.MakeGroupBegin || movementSupervised.State == (int)MovementState.DragGroupBegin)
                {
                    lastMovementSupervised = movementSupervised;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.MakeGroupMove || movementSupervised.State == (int)MovementState.MakeGroupEnd || movementSupervised.State == (int)MovementState.DragGroupMove || movementSupervised.State == (int)MovementState.DragSingleEnd)
                {
                    if (movementSupervised.Time == lastMovementSupervised.Time)
                    {
                        lastMovementSupervised = movementSupervised;
                        continue;
                    }

                    if (!movementSupervised.IsAbnormal || !lastMovementSupervised.IsAbnormal)
                    {
                        distance = (float)(Math.Sqrt(Math.Pow(lastMovementSupervised.XPosition - movementSupervised.XPosition, 2) + Math.Pow(lastMovementSupervised.YPosition - movementSupervised.YPosition, 2)));
                        DDSpeed = (float)(distance / ((movementSupervised.Time - lastMovementSupervised.Time) / 1000.0));

                        if (isFirstCalculatedDDSpeed)
                        {
                            DDSpeedMIN = DDSpeed;
                            isFirstCalculatedDDSpeed = false;
                        }
                        else
                        {
                            if (DDSpeed < DDSpeedMIN)
                            {
                                DDSpeedMIN = DDSpeed;
                            }
                        }
                    }

                    lastMovementSupervised = movementSupervised;
                }
            }

            return (float)Math.Round(DDSpeedMIN, 2);
        }

        // DD First Time
        public static float CalculateDDFirstTime(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();

            MovementSupervised movementSupervised = movementSupervisedCollection.FirstOrDefault();

            return movementSupervised == null ? 0 : movementSupervised.Time;
        }

        // DD From First Time
        public static float CalculateDDFromFirstTime(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();

            MovementSupervised movementSupervisedFirst = movementSupervisedCollection.FirstOrDefault();
            MovementSupervised movementSupervisedLast = movementSupervisedCollection.LastOrDefault();

            return movementSupervisedFirst == null || movementSupervisedLast == null ? 0 : movementSupervisedLast.Time - movementSupervisedFirst.Time;
        }

        // DD Count
        public static int CalculateDDCount(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();
            MovementSupervised movementSupervised = movementSupervisedCollection.FirstOrDefault();

            int count = 0;
            bool isOperating = false;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                movementSupervised = movementSupervisedCollection[i];

                if (movementSupervised.State == (int)MovementState.DragSingleBegin ||  movementSupervised.State == (int)MovementState.DragGroupBegin)
                {
                    isOperating = true;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragGroupMove)
                {
                    continue;
                }
                else if(movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.DragSingleEnd)
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
        public static int CalculateUTurnHorizontalCount(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();
            MovementSupervised lastMovementSupervised = movementSupervisedCollection.FirstOrDefault();

            int count = 0;
            bool isToPositiveDirection = true;
            bool isDirectionHasBeenCalculated = false;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                var movementSupervised = movementSupervisedCollection[i];

                if (movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.DragGroupBegin || movementSupervised.IsAbnormal)
                {
                    lastMovementSupervised = movementSupervised;
                    isDirectionHasBeenCalculated = false;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.DragGroupMove || movementSupervised.State == (int)MovementState.DragGroupEnd)
                {
                    if (!isDirectionHasBeenCalculated)
                    {
                        if(movementSupervised.XPosition - lastMovementSupervised.XPosition != 0)
                        {
                            isDirectionHasBeenCalculated = true;

                            isToPositiveDirection = movementSupervised.XPosition - lastMovementSupervised.XPosition > 0 ? true : false;

                            lastMovementSupervised = movementSupervised;
                        }
                    }
                    else
                    {
                        if (movementSupervised.XPosition - lastMovementSupervised.XPosition > 0 && !isToPositiveDirection)
                        {
                            count++;
                            isToPositiveDirection = true;
                        }
                        else if (movementSupervised.XPosition - lastMovementSupervised.XPosition < 0 && isToPositiveDirection)
                        {
                            count++;
                            isToPositiveDirection = false;                       
                        }

                        lastMovementSupervised = movementSupervised;
                    }
                }
            }

            return count;
        }

        // U Turn Vertical Count
        public static int CalculateUTurnVerticalCount(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();
            MovementSupervised lastMovementSupervised = movementSupervisedCollection.FirstOrDefault();

            int count = 0;
            bool isToPositiveDirection = true;
            bool isDirectionHasBeenCalculated = false;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                var movementSupervised = movementSupervisedCollection[i];

                if (movementSupervised.State == (int)MovementState.DragSingleBegin || movementSupervised.State == (int)MovementState.DragGroupBegin || movementSupervised.IsAbnormal)
                {
                    lastMovementSupervised = movementSupervised;
                    isDirectionHasBeenCalculated = false;
                }
                else if (movementSupervised.State == (int)MovementState.DragSingleMove || movementSupervised.State == (int)MovementState.DragSingleEnd || movementSupervised.State == (int)MovementState.DragGroupMove || movementSupervised.State == (int)MovementState.DragGroupEnd)
                {
                    if (!isDirectionHasBeenCalculated)
                    {
                        if (movementSupervised.YPosition - lastMovementSupervised.YPosition != 0)
                        {
                            isDirectionHasBeenCalculated = true;

                            isToPositiveDirection = movementSupervised.YPosition - lastMovementSupervised.YPosition > 0 ? true : false;

                            lastMovementSupervised = movementSupervised;
                        }
                    }
                    else
                    {
                        if (movementSupervised.YPosition - lastMovementSupervised.YPosition > 0 && !isToPositiveDirection)
                        {
                            count++;
                            isToPositiveDirection = true;
                        }
                        else if (movementSupervised.YPosition - lastMovementSupervised.YPosition < 0 && isToPositiveDirection)
                        {
                            count++;
                            isToPositiveDirection = false;
                        }

                        lastMovementSupervised = movementSupervised;
                    }
                }
            }

            return count;
        }

        // Grouping DD Count
        public static int CalculateGroupingDDCount(IList<MovementSupervised> movementSupervisedCollection)
        {
            movementSupervisedCollection = movementSupervisedCollection.OrderBy(m => m.Time).ToList();
            MovementSupervised movementSupervised = movementSupervisedCollection.FirstOrDefault();

            int count = 0;
            bool isOperating = false;
            for (int i = 0; i < movementSupervisedCollection.Count; i++)
            {
                movementSupervised = movementSupervisedCollection[i];

                if (movementSupervised.State == (int)MovementState.MakeGroupBegin)
                {
                    isOperating = true;
                }
                else if (movementSupervised.State == (int)MovementState.MakeGroupMove)
                {
                    continue;
                }
                else if (movementSupervised.State == (int)MovementState.MakeGroupEnd)
                {
                    if (isOperating)
                    {
                        count++;
                        isOperating = false;
                    }
                }
            }

            return count;
        }

    }
}
