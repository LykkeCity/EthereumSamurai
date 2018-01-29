using System;

namespace Lykke.Service.EthereumSamurai.MongoDb.Utils
{
    public static class DateTimeExtension
    {
        public static int GetUnixTime(this DateTime date)
        {
            var unixTimestamp = (int) date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            return unixTimestamp;
        }
    }
}
