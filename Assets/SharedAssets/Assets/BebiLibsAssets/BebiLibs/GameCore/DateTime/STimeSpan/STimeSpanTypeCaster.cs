using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BebiLibs
{

    public class STimeSpanTypeCaster : JsonConverter<STimeSpan>
    {
        public STimeSpanTypeCaster() : base()
        {

        }

        public static readonly STimeSpanTypeCaster Instance = new STimeSpanTypeCaster();

        public override STimeSpan ReadJson(JsonReader reader, Type objectType, [AllowNull] STimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //Debug.Log("Parsing: |" + reader.Value + " | " + reader.Path + " " + reader.ValueType);

            Type type = reader.ValueType;
            if (type == typeof(System.TimeSpan))
            {
                return STimeSpan.FromString(reader.Value.ToString());
            }
            if (type == typeof(System.Double))
            {
                return STimeSpan.FromDouble(double.Parse(reader.Value.ToString()));
            }
            else if (type == typeof(System.Int64))
            {
                return STimeSpan.FromDouble((Double)long.Parse(reader.Value.ToString()));
            }
            else if (type == typeof(string))
            {
                try
                {
                    STimeSpan dateTime = JsonConvert.DeserializeObject<STimeSpan>(reader.Value.ToString());
                    return dateTime;
                }
                catch (System.Exception e)
                {
                    Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "STimeSpanTypeCaster: Parsing FAILED - Value: {0}, Path: {1}, Type: {2} ", reader.Value, reader.Path, reader.ValueType);
                    Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "STimeSpanTypeCaster: Load From Unknown String, Error: {0}", e);
                    return STimeSpan.Zero;
                }
            }

            Debug.LogError($"Unable To Parse {reader.Path} {reader.Value} As STimeSpan");
            return STimeSpan.Zero;
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] STimeSpan value, JsonSerializer serializer)
        {
            string json = JsonConvert.SerializeObject(value.timeSpanInSeconds);
            writer.WriteValue(json);
        }
    }
}
