using System;
using System.Collections;
using UnityEngine;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BebiLibs
{
    [JsonConverter(typeof(STimeSpanTypeCaster))]
    [Serializable]
    public class STimeSpan : ISerializationCallbackReceiver, IEquatable<STimeSpan>, ICloneable
    {
        [JsonProperty()]
        [SerializeField] private double _timeSpanInSeconds;


        [JsonIgnore()]
        [SerializeField] private System.TimeSpan _timeSpan;

        [JsonIgnore()]
        private bool _isSerialized = false;


        [JsonIgnore()]
        public TimeSpan TimeSpan => _timeSpan;
        [JsonIgnore()]
        public double timeSpanInSeconds => _timeSpanInSeconds;

        public STimeSpan(System.TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
            _isSerialized = false;
            _timeSpanInSeconds = _timeSpan.TotalSeconds;
            this.UpdateEditorPropertyes();
        }

        public STimeSpan Clone() => new STimeSpan(_timeSpan);
        object ICloneable.Clone() => this.Clone();

        private TimeSpan GetTimespanFromSeconds(double seconds) => TimeSpan.FromSeconds(seconds);

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            this.OnAfterDeserialize();
        }

        public void OnBeforeSerialize()
        {
            _timeSpanInSeconds = _timeSpan.TotalSeconds;
            _isSerialized = true;
        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            if (!_isSerialized) return;
#endif
            _timeSpan = TimeSpan.FromSeconds(_timeSpanInSeconds);
            this.UpdateEditorPropertyes();
        }

        public static STimeSpan FromDouble(double timeSeconds)
        {
            return new STimeSpan(TimeSpan.FromSeconds(timeSeconds));
        }

        public static STimeSpan FromString(string timeString)
        {
            bool success = TimeSpan.TryParse(timeString, out TimeSpan timeSpan);
            if (!success) Debug.LogError($"Unable To parse {timeString} To timeSpan");
            return new STimeSpan(timeSpan);
        }


        public static STimeSpan Min => new STimeSpan(TimeSpan.MinValue);
        public static STimeSpan Max => new STimeSpan(TimeSpan.MaxValue);
        public static STimeSpan Zero => new STimeSpan(TimeSpan.Zero);

        public override string ToString() => _timeSpan.ToString();

        public static implicit operator STimeSpan(TimeSpan d) => new STimeSpan(d);
        public static explicit operator TimeSpan(STimeSpan b) => b._timeSpan;

        public override bool Equals(object obj) => this.Equals((STimeSpan)obj);
        public override int GetHashCode() => (_timeSpan, _timeSpan).GetHashCode();

        public bool Equals(STimeSpan other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (this.GetType() != other.GetType()) return false;
            return _timeSpan == other._timeSpan;
        }

        public static bool operator ==(STimeSpan lhs, STimeSpan rhs)
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

        public static bool operator !=(STimeSpan lhs, STimeSpan rhs) => !(lhs == rhs);



#if UNITY_EDITOR
        [SerializeField] private string _displayTime;
        [SerializeField] private int _day, _hours, _minutes, _seconds;
#endif
        private void UpdateEditorPropertyes()
        {
#if UNITY_EDITOR
            _displayTime = _timeSpan.ToString();
            _day = _timeSpan.Days;
            _hours = _timeSpan.Hours;
            _minutes = _timeSpan.Minutes;
            _seconds = _timeSpan.Seconds;
#endif
        }
    }

}
