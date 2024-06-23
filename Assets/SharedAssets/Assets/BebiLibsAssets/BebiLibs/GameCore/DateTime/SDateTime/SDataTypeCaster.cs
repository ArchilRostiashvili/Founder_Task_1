using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using UnityEngine;

namespace BebiLibs
{
    public class SDataTypeCaster : JsonConverter<SDateTime>
    {
        public SDataTypeCaster() : base()
        {

        }

        [System.Serializable]
        public class SerializeTime
        {
            public long time;
            public int kind;
        }

        public static readonly SDataTypeCaster Instance = new SDataTypeCaster();

        public override SDateTime ReadJson(JsonReader reader, Type objectType, [AllowNull] SDateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            //Debug.Log("Parsing: |" + reader.Value + " | " + reader.Path + " " + reader.ValueType);

            DateTimeKind dateTimeKind = existingValue != null ? existingValue.Kind : DateTimeKind.Local;
            Type type = reader.ValueType;
            if (type == typeof(System.DateTime))
            {
                return SDateTime.FromString(reader.Value.ToString(), dateTimeKind);
            }
            if (type == typeof(System.Int64))
            {
                return SDateTime.FromLong(long.Parse(reader.Value.ToString()), dateTimeKind);
            }
            else if (type == typeof(string))
            {
                return TryToParseCustomString(reader);
            }

            Debug.LogError($"Unable To Parse {reader.Path} {reader.Value} As SDateTime");
            return new SDateTime(DateTime.MinValue);
        }

        public SDateTime TryToParseCustomString(JsonReader reader)
        {
            try
            {
                SerializeTime dateTime = JsonConvert.DeserializeObject<SerializeTime>(reader.Value.ToString());
                return SDateTime.FromLong(dateTime.time, (DateTimeKind)dateTime.kind);
            }
            catch (System.Exception e)
            {
                if (TryParserTimeString(reader, out SDateTime dateTime))
                {
                    return dateTime;
                }
                else
                {
                    Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "SDateTypeCaster: Parsing FAILED - Value: {0}, Path: {1}, Type: {2} ", reader.Value, reader.Path, reader.ValueType);
                    Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "SDateTypeCaster: Load From Unknown String, Error: {0}", e);
                    return new SDateTime(DateTime.MinValue);
                }
            }
        }

        public bool TryParserTimeString(JsonReader reader, out SDateTime dateTime)
        {
            if (DateTime.TryParse(reader.Value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime date))
            {
                dateTime = new SDateTime(date);
                return true;
            }
            else
            {
                Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "SDateTypeCaster: Parsing FAILED - Value: {0}, Path: {1}, Type: {2} ", reader.Value, reader.Path, reader.ValueType);
                dateTime = new SDateTime(DateTime.MinValue);
                return false;
            }
        }


        public override void WriteJson(JsonWriter writer, [AllowNull] SDateTime value, JsonSerializer serializer)
        {
            SerializeTime data = new SerializeTime()
            {
                time = value.EpochTime,
                kind = (int)value.Kind
            };
            //Debug.LogWarning("Cast Data " + data.time + " " + data.kind);
            string json = JsonConvert.SerializeObject(data);
            writer.WriteValue(json);
        }
    }
}
