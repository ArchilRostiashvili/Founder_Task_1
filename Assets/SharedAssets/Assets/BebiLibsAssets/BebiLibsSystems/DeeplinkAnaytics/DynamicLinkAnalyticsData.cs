using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace DynamicAnalyticsLogger
{
    [System.Serializable]
    public class DynamicLinkData
    {
        public string EventID;
        [Header("Dynamic Properties:")]
        [JsonIgnore()]
        public List<DynamicProperty> PropertiesEntryList = new List<DynamicProperty>();
        public Dictionary<string, string> PropertyDictionary = new Dictionary<string, string>();
        [Header("Dynamic Events:")]
        [JsonIgnore()]
        public List<DynamicEvents> DynamicEventsList = new List<DynamicEvents>();
        public Dictionary<string, Dictionary<string, string>> EventDictionary = new Dictionary<string, Dictionary<string, string>>();

        public override string ToString()
        {
            string compText = "";
            AppendProperties(PropertiesEntryList, ref compText);
            AppendEvent(DynamicEventsList, ref compText);
            return compText;
        }

        private void AppendEvent(List<DynamicEvents> dynamicEvents, ref string data)
        {
            foreach (var item in dynamicEvents)
            {
                data += $"{item.EventName}:\n";
                AppendProperties(item.PropertiesEntryList, ref data);
            }
        }

        private void AppendProperties(List<DynamicProperty> dataEntries, ref string data)
        {
            foreach (var item in dataEntries)
                data += "\t" + nameof(item.Key) + " - " + item.Value + "\n";
        }


        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            PropertyDictionary.Clear();
            foreach (var item in PropertiesEntryList)
            {
                PropertyDictionary.Add(item.Key, item.Value);
            }

            EventDictionary.Clear();
            foreach (var item in DynamicEventsList)
            {
                EventDictionary.Add(item.EventName, item.GetDictionary());
            }
        }


        [OnDeserialized]
        internal void OnDeserializing(StreamingContext context)
        {
            PropertiesEntryList.Clear();
            foreach (var item in PropertyDictionary)
            {
                PropertiesEntryList.Add(new DynamicProperty(item.Key, item.Value));
            }

            DynamicEventsList.Clear();
            foreach (var item in EventDictionary)
            {
                DynamicEvents events = new DynamicEvents(item.Key);
                events.PropertiesEntryList.Clear();
                foreach (var property in item.Value)
                {
                    events.AddProperty(property.Key, property.Value);
                }
                DynamicEventsList.Add(events);
            }
        }
    }



    [System.Serializable]
    public class DynamicEvents
    {
        public string EventName;
        [JsonIgnore()]
        public List<DynamicProperty> PropertiesEntryList = new List<DynamicProperty>();
        public Dictionary<string, string> PropertyDictionary = new Dictionary<string, string>();

        public DynamicEvents(string eventName)
        {
            EventName = eventName;
        }

        public DynamicEvents(string eventName, List<DynamicProperty> dynamicProperties)
        {
            EventName = eventName;
            PropertiesEntryList = dynamicProperties;
        }

        public void AddProperty(DynamicProperty dynamicProperty)
        {
            PropertiesEntryList.Add(dynamicProperty);
        }

        public void AddProperty(string key, string value)
        {
            PropertiesEntryList.Add(new DynamicProperty(key, value));
        }

        internal Dictionary<string, string> GetDictionary()
        {
            PropertyDictionary.Clear();
            foreach (var item in PropertiesEntryList)
            {
                PropertyDictionary.Add(item.Key, item.Value);
            }
            return PropertyDictionary;
        }
    }

    [System.Serializable]
    public class DynamicProperty
    {
        public string Key;
        public string Value;

        public DynamicProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
