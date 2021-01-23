using System;
using System.Net;
using Newtonsoft.Json;

namespace Inbox.Core {
    internal class IpAddressJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(IPAddress));
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
            writer.WriteValue(value?.ToString());
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
        ) {
            return IPAddress.Parse(reader.Value?.ToString() ?? "");
        }
    }

    internal static class IpAddressJsonConverterExtensions {
        public static JsonSerializerSettings WithIpAddress(this JsonSerializerSettings settings) {
            settings.Converters.Add(new IpAddressJsonConverter());
            return settings;
        }
    }
}
