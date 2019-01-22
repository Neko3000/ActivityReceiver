using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityReceiver.Converters
{
    public static class StringConverter
    {
        public static string ConvertToSingleString(IList<string> stringList, string seperator)
        {
            var singleString = "";
            for (int i = 0; i < stringList.Count(); i++)
            {
                if (i == 0)
                {
                    singleString = singleString + stringList[i];
                }
                else
                {
                    singleString = singleString + seperator + stringList[i];
                }
            }

            return singleString;
        }
    }
}
