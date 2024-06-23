using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace BebiLibs.ServerLoader
{
    [System.Serializable]
    public class TextureCacheInfo
    {
        [JsonProperty("fn")]
        public string fileName;
        [JsonProperty("t")]
        [SerializeField] private string _creationTime;
        [JsonProperty("v")]
        public int version;
        [JsonIgnore()]
        public DateTime dateTime
        {
            get
            {
                TimeUtils.TryLoadTime(_creationTime, out DateTime time);
                return time;
            }
            set
            {
                _creationTime = TimeUtils.TimeToString(value);
            }
        }

        public TextureCacheInfo(string fileName, int version, DateTime dateTime)
        {
            this.fileName = fileName;
            _creationTime = TimeUtils.TimeToString(dateTime);
            this.version = version;
        }
    }

    [System.Serializable]
    public class DataCache
    {
        [JsonProperty("c")]
        public int imageCount = 0;
        [JsonProperty("i")]
        public List<TextureCacheInfo> arrayDataTexture = new List<TextureCacheInfo>();

        [JsonIgnore()]
        private readonly string _cacheImageDataID;

        public DataCache(string cacheImageDataID)
        {
            _cacheImageDataID = cacheImageDataID;
        }

        public int GetVersion(string fileName)
        {
            int index = arrayDataTexture.FindIndex(x => x.fileName == fileName);
            if (index >= 0)
            {
                return arrayDataTexture[index].version;
            }
            return -1;
        }

        public void UpdateLoadData()
        {
            if (JsonFileLoader.TryLoadJsonFile<DataCache>(_cacheImageDataID, out DataCache dataCache))
            {
                imageCount = dataCache.imageCount;
                arrayDataTexture = dataCache.arrayDataTexture;
            }
            else
            {
                SerializeData();
            }
        }

        public void UpdateVersion(string fileName, int vestion)
        {
            TextureCacheInfo info = new TextureCacheInfo(fileName, vestion, DateTime.Now);
            int index = arrayDataTexture.FindIndex(x => x.fileName == fileName);
            if (index < 0)
            {
                arrayDataTexture.Add(info);
                imageCount++;
                SerializeData();
            }
            else
            {
                info.dateTime = arrayDataTexture[index].dateTime;
                arrayDataTexture[index] = info;
                SerializeData();
            }
        }

        public void UpdateTime(string fileName, DateTime dateTime)
        {
            TextureCacheInfo info = new TextureCacheInfo(fileName, 0, dateTime);
            int index = arrayDataTexture.FindIndex(x => x.fileName == fileName);
            if (index < 0)
            {
                arrayDataTexture.Add(info);
                imageCount++;
                SerializeData();
            }
            else
            {
                info.version = arrayDataTexture[index].version;
                arrayDataTexture[index] = info;
                SerializeData();
            }
        }

        public void RemoveImage(string fileName)
        {
            int index = arrayDataTexture.FindIndex(x => x.fileName == fileName);
            if (index != -1)
            {
                arrayDataTexture.RemoveAt(index);
                imageCount--;
                SerializeData();
            }
        }

        public void SerializeData()
        {
            string json = JsonConvert.SerializeObject(this);
            JsonFileLoader.TrySaveJsonFile<DataCache>(_cacheImageDataID, this);
        }

        public TextureCacheInfo this[int index]
        {
            get => arrayDataTexture[index];
            set => arrayDataTexture[index] = value;
        }

        [JsonIgnore()]
        public int Count => arrayDataTexture.Count();
        public void Add(TextureCacheInfo item) => arrayDataTexture.Add(item);
        public void Clear() => arrayDataTexture.Clear();
        public bool Contains(TextureCacheInfo item) => arrayDataTexture.Contains(item);
        public bool Remove(TextureCacheInfo item) => arrayDataTexture.Remove(item);
    }
}