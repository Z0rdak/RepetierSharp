using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RepetierMqtt.Util
{
    public class DeserialisationHelper
    {
        public static JsonSerializerOptions IngoreNullableFieldsOptions => new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            WriteIndented = true
        };
    }
}
