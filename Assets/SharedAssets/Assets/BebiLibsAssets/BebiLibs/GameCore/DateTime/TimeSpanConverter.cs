using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BebiLibs
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public TimeSpanConverter() : base()
        {

        }

        public static readonly TimeSpanConverter Instance = new TimeSpanConverter();

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, [AllowNull] TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            Type type = reader.ValueType;
            if (type == typeof(System.TimeSpan))
            {
                return TimeSpan.Parse(reader.Value.ToString());
            }
            if (type == typeof(System.Double))
            {
                return TimeSpan.FromSeconds(double.Parse(reader.Value.ToString()));
            }
            else if (type == typeof(System.Int64))
            {
                return TimeSpan.FromSeconds((Double)long.Parse(reader.Value.ToString()));
            }
            else if (type == typeof(string))
            {
                try
                {
                    TimeSpan dateTime = JsonConvert.DeserializeObject<TimeSpan>(reader.Value.ToString());
                    return dateTime;
                }
                catch (System.Exception e)
                {
                    Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "TimeSpanConverter: Parsing FAILED - Value: {0}, Path: {1}, Type: {2} ", reader.Value, reader.Path, reader.ValueType);
                    Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "TimeSpanConverter: Load From Unknown String, Error: {0}", e);
                    return TimeSpan.Zero;
                }
            }

            Debug.LogError($"Unable To Parse {reader.Path} {reader.Value} As TimeSpan");
            return TimeSpan.Zero;
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] TimeSpan value, JsonSerializer serializer)
        {
            string json = JsonConvert.SerializeObject(value.TotalSeconds);
            writer.WriteValue(json);
        }
    }
}
