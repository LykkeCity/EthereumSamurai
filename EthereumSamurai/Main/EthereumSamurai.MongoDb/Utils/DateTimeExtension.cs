using System;
using System.Collections.Generic;
using System.Text;

namespace EthereumSamurai.MongoDb.Utils
{
    public static class DateTimeExtension
    {
        public static int GetUnixTime(this DateTime date)
        {
            int unixTimestamp = (int)(date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            return unixTimestamp;
        }
    }
}
