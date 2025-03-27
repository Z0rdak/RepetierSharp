using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using RepetierSharp.Config;
using RepetierSharp.Models;
using RepetierSharp.Models.Communication;
using RepetierSharp.Models.Config;
using RepetierSharp.Models.Events;
using RepetierSharp.Util;
using static RepetierSharp.Util.EventConstants;

namespace RepetierSharp.Serialization
{
    public class RepetierCommandConverter : JsonConverter<BaseCommand>
    {
        private readonly ILogger<RepetierCommandConverter> _logger;

        public RepetierCommandConverter()
        {
            using var factory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            _logger = factory.CreateLogger<RepetierCommandConverter>();
        }
   
        public override BaseCommand Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new JsonException("Not implemented, do not deserialize!");
        }
        
        public override void Write(Utf8JsonWriter writer, BaseCommand baseCmd, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("action");
            writer.WriteStringValue(baseCmd.Action);
            writer.WritePropertyName("data");
            JsonSerializer.Serialize(writer, baseCmd.Data, baseCmd.Data.GetType(), options);
            writer.WritePropertyName("printer");
            writer.WriteStringValue(baseCmd.Printer);
            writer.WritePropertyName("callback_id");
            writer.WriteNumberValue(baseCmd.CallbackId);
            writer.WriteEndObject();
        }
    }
}
