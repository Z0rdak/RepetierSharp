using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Util
{
    /// <summary>
    /// Code taken from: https://stackoverflow.com/questions/61565947/the-json-value-could-not-be-converted-to-system-byte
    /// See https://github.com/dotnet/runtime/issues/30456 
    /// </summary>
    public class ByteArrayConverter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            short[] sByteArray = JsonSerializer.Deserialize<short[]>(ref reader);
            byte[] value = new byte[sByteArray.Length];
            for (int i = 0; i < sByteArray.Length; i++)
            {
                value[i] = (byte)sByteArray[i];
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var val in value)
            {
                writer.WriteNumberValue(val);
            }

            writer.WriteEndArray();
        }


        public static JsonSerializerOptions ByteArraySerializerOptions { get => ByteArraySerializerOption(); }
        /// <summary>
        /// Adapted from: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to#registration-sample---converters-collection
        /// </summary>
        /// <returns></returns>
        private static JsonSerializerOptions ByteArraySerializerOption()
        {
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new ByteArrayConverter());
            serializeOptions.WriteIndented = true;
            return serializeOptions;
        }


    } 
}
