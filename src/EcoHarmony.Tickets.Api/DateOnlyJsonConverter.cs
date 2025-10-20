using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EcoHarmony.Tickets.Api
{
    // Fuerza el formato ISO "yyyy-MM-dd" para DateOnly
    public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s))
                throw new JsonException("Empty date.");

            // Intenta parseo exacto ISO (sin espacios ni comentarios)
            if (DateOnly.TryParseExact(s.Trim(), Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                return d;

            throw new JsonException($"Invalid DateOnly. Expected format {Format}.");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}
