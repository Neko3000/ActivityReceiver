using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace ActivityReceiver.Functions
{
    public class AutoMapperHandler
    {
        public static IList<S> ListMapper<T, S>(IList<T> objs)
        {
            var objectDTOCollection = new List<S>();

            foreach (var obj in objs)
            {
                var objectDTO = Mapper.Map<T, S>(obj);

                objectDTOCollection.Add(objectDTO);
            }

            return objectDTOCollection;
        }
    }
}
