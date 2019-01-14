using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityReceiver.Models;
using ActivityReceiver.ViewModels;
using ActivityReceiver.Data;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace ActivityReceiver.Functions
{

    public class AnswerReplayHandler
    {
        public static IList<S> ConvertToDTOCollection<T,S>(IList<T> objs)
        {
            var objectDTOCollection = new List<S>();

            foreach(var obj in objs)
            {
                var objectDTO = Mapper.Map<T, S>(obj);

                objectDTOCollection.Add(objectDTO);
            }

            return objectDTOCollection;
        }

        //public static IList<MovementDTO> ConvertToMovementDTOForEachMovement(IList<Movement> movements)
        //{
        //    var movementDTOs = new List<MovementDTO>();

        //    foreach(var movement in movements)
        //    {
        //        var movementDTO = Mapper.Map<Movement,MovementDTO>(movement);

        //        movementDTOs.Add(movementDTO);
        //    }
        //    return movementDTOs;
        //}
    }
}
