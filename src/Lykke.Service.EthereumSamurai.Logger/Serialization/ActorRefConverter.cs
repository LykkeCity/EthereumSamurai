using System;
using Akka.Actor;
using Newtonsoft.Json;

namespace Lykke.Service.EthereumSamurai.Logger.Serialization
{
    public class ActorRefConverter : JsonConverter
    {
        public override bool CanRead { get; }
            = false;

        public override bool CanWrite { get; }
            = true;


        public override bool CanConvert(Type objectType)
        {
            return typeof(IActorRef).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var actorRef = value as IActorRef;

            writer.WriteValue(actorRef?.ToString());
        }
    }
}
