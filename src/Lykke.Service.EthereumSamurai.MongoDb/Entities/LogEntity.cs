using Lykke.Service.EthereumSamurai.Core.Utils;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.EthereumSamurai.MongoDb.Entities
{
    public class LogEntity
    {
        public DateTime DateTime { get; set; }
        public string Level { get; set; }
        public string Env { get; set; }
        public string AppName { get; set; }
        public string Version { get; set; }
        public string Component { get; set; }
        public string Process { get; set; }
        public string Context { get; set; }
        public string Type { get; set; }
        public string Stack { get; set; }
        public string Msg { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public static LogEntity CreateWithoutRowKey(
            string level,
            string component,
            string process,
            string context,
            string type,
            string stack,
            string msg,
            DateTime dateTime)
        {
            return new LogEntity
            {
                PartitionKey = GeneratePartitionKey(dateTime),
                DateTime = dateTime,
                Level = level,
                Env = AppEnvironment.EnvInfo,
                AppName = AppEnvironment.Name,
                Version = AppEnvironment.Version,
                Component = component,
                Process = process,
                Context = Truncate(context),
                Type = type,
                Stack = Truncate(stack),
                Msg = Truncate(msg)
            };
        }

        public static string GeneratePartitionKey(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static string GenerateRowKey(DateTime dateTime, int itemNumber, int retryNumber)
        {
            return retryNumber == 0
                ? $"{dateTime:HH:mm:ss.fffffff}.{itemNumber:000}"
                : $"{dateTime:HH:mm:ss.fffffff}.{itemNumber:000}.{retryNumber:000}";
        }

        private static string Truncate(string str)
        {
            if (str == null)
            {
                return null;
            }

            const int maxLength = 32766 - 3;

            if (str.Length > maxLength)
            {
                return string.Concat(str.Substring(0, maxLength), "...");
            }

            return str;
        }
    }
}
