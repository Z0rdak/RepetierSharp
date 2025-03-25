using System.Text.Json;
using System.Text.Json.Serialization;

namespace RepetierSharp.Serialization
{
    public static class SerializationOptions
    {
        public static JsonSerializerOptions ResponseConverter(string commandId)
        {
            return new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                Converters =
                {
                    new RepetierResponseConverter(commandId)
                }
            };
        }
        
        public static readonly JsonSerializerOptions DefaultOptions = new()
        {
            AllowTrailingCommas = true,
            WriteIndented = true,
            Converters =
            {
                new RepetierBaseEventConverter()
            }
        };
        
        public static readonly JsonSerializerOptions WriteOptions = new()
        {
            
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
}
