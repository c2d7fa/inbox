using System;
using System.Net;
using Newtonsoft.Json;

namespace Inbox.Azure
{
    // https://stackoverflow.com/a/18669492
    class IPAddressJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(IPAddress));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return IPAddress.Parse((string)reader.Value);
        }
    }

    static class IPAddressJsonConverterExtensions {
        public static JsonSerializerSettings WithIPAddress(this JsonSerializerSettings settings) {
            settings.Converters.Add(new IPAddressJsonConverter());
            return settings;
        }
    }
}
