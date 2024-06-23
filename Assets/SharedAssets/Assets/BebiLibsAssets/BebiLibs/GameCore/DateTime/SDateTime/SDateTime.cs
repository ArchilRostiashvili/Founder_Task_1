using System;
using System.Collections;
using UnityEngine;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BebiLibs
{
    [JsonConverter(typeof(SDataTypeCaster))]
    [System.Serializable]
    public class SDateTime : ISerializationCallbackReceiver, IEquatable<SDateTime>, ICloneable, IComparable<SDateTime>, IComparable
    {
        [SerializeField] private System.DateTime _dateTime;
        [SerializeField] private long _timeSinceEpoch;
        [SerializeField] private int _kind;
        private bool _isSerialized = false;

        public DateTimeKind Kind => (DateTimeKind)_kind;
        public DateTime DateTime => _dateTime;
        public long EpochTime => _timeSinceEpoch;


        public SDateTime(DateTime dateTime)
        {
            _dateTime = dateTime;
            _kind = (int)_dateTime.Kind;
            _isSerialized = false;
            _timeSinceEpoch = this.GetUniversalEpochTime(_dateTime);
            this.UpdateEditorPropertyes();
        }

        public SDateTime Clone() => new SDateTime(_dateTime);
        object ICloneable.Clone() => this.Clone();

        private long GetUniversalEpochTime(DateTime dateTime) => ((DateTimeOffset)dateTime.ToUniversalTime()).ToUnixTimeSeconds();

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            this.OnAfterDeserialize();
        }

        public void OnBeforeSerialize()
        {
            _timeSinceEpoch = this.GetUniversalEpochTime(_dateTime);
            _kind = (int)_dateTime.Kind;
            _isSerialized = true;
        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            if (!_isSerialized) return;
#endif
            DateTimeOffset offsetTime = DateTimeOffset.FromUnixTimeSeconds(_timeSinceEpoch);
            _dateTime = (DateTimeKind)_kind == DateTimeKind.Utc ? offsetTime.UtcDateTime : offsetTime.UtcDateTime.ToLocalTime();
            this.UpdateEditorPropertyes();
        }

        public static SDateTime FromLong(Int64 timeSinceEpoch, DateTimeKind dateTimeKind)
        {
            DateTimeOffset offsetTime = DateTimeOffset.FromUnixTimeSeconds(timeSinceEpoch);
            DateTime calculatedTime = offsetTime.UtcDateTime;
            if (dateTimeKind == DateTimeKind.Local)
            {
                calculatedTime = calculatedTime.ToLocalTime();
            }
            return new SDateTime(calculatedTime);
        }

        public static SDateTime FromString(string timeString, DateTimeKind dateTimeKind)
        {
            bool success = DateTime.TryParse(timeString, out DateTime dateTime);
            if (!success) Debug.LogError($"Unable To parse {timeString} To DateTime");
            dateTime = DateTime.SpecifyKind(dateTime, dateTimeKind);
            //Debug.Log("Specified Kind " + dateTime.Kind + " " + dateTimeKind);
            return new SDateTime(dateTime);
        }


        public static SDateTime Min => new SDateTime(DateTime.MinValue);
        public static SDateTime UtcMin => new SDateTime(DateTime.MinValue.ToUniversalTime());
        public static SDateTime Now => new SDateTime(DateTime.Now);
        public static SDateTime UtcNow => new SDateTime(DateTime.Now.ToUniversalTime());
        public static SDateTime Max => new SDateTime(DateTime.MaxValue);
        public static SDateTime Utcmax => new SDateTime(DateTime.MaxValue.ToUniversalTime());

        public override string ToString() => _dateTime.ToString();

        public static implicit operator SDateTime(DateTime d) => new SDateTime(d);
        public static explicit operator DateTime(SDateTime b) => b.DateTime;

        public override bool Equals(object obj) => this.Equals((SDateTime)obj);
        public override int GetHashCode() => (_dateTime, _timeSinceEpoch).GetHashCode();

        public bool Equals(SDateTime other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this.GetType() != other.GetType()) return false;
            return _timeSinceEpoch == other._timeSinceEpoch;
        }

        public static bool operator ==(SDateTime lhs, SDateTime rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SDateTime lhs, SDateTime rhs) => !(lhs == rhs);



#if UNITY_EDITOR
        [SerializeField] private string _displayTime;
        [SerializeField] private int _day, _month, _year, _hours, _minutes, _seconds;
#endif
        private void UpdateEditorPropertyes()
        {
#if UNITY_EDITOR
            _displayTime = _dateTime.ToString();
            _day = _dateTime.Day;
            _month = _dateTime.Month;
            _year = _dateTime.Year;
            _hours = _dateTime.Hour;
            _minutes = _dateTime.Minute;
            _seconds = _dateTime.Second;
#endif
        }

        public int CompareTo(SDateTime other)
        {
            if (other == null) return 1;

            return DateTime.CompareTo(other.DateTime);
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            SDateTime sDateTime = obj as SDateTime;
            if (sDateTime != null)
            {
                return DateTime.CompareTo(sDateTime.DateTime);
            }
            else
            {
                throw new ArgumentException("Object is not a DateTime Object");
            }
        }
    }

}
