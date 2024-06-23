using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;
using Newtonsoft.Json;

[JsonObject()]
[System.Serializable]
public class SetDataList : IList<BannerSet>
{
    [JsonProperty("rand")]
    public bool rand;
    [JsonProperty("sets")]
    public List<BannerSet> arrayBannerSets = new List<BannerSet>();

    public static bool FromJson(string jsonText, out SetDataList packList)
    {
        try
        {
            packList = JsonConvert.DeserializeObject<SetDataList>(jsonText);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            packList = null;
            return false;
        }
    }

    [JsonIgnore()]
    public BannerSet this[int index]
    {
        get => this.arrayBannerSets[index];
        set => this.arrayBannerSets[index] = value;
    }
    [JsonIgnore()]
    public int Count => this.arrayBannerSets.Count;
    [JsonIgnore()]
    public bool IsReadOnly => false;

    public void Add(BannerSet item) => this.arrayBannerSets.Add(item);
    public void Clear() => this.arrayBannerSets.Clear();
    public bool Contains(BannerSet item) => this.arrayBannerSets.Contains(item);
    public void CopyTo(BannerSet[] array, int arrayIndex) => this.arrayBannerSets.CopyTo(array, arrayIndex);
    public int IndexOf(BannerSet item) => this.arrayBannerSets.IndexOf(item);
    public void Insert(int index, BannerSet item) => this.arrayBannerSets.Insert(index, item);
    public bool Remove(BannerSet item) => this.arrayBannerSets.Remove(item);
    public void RemoveAt(int index) => this.arrayBannerSets.RemoveAt(index);
    IEnumerator IEnumerable.GetEnumerator() => this.arrayBannerSets.GetEnumerator();
    public IEnumerator<BannerSet> GetEnumerator() => this.arrayBannerSets.GetEnumerator();
}

[System.Serializable]
public class BannerSet
{
    [JsonProperty("set_id")]
    public string setId;
    [JsonProperty("p")]
    public int P;
    [JsonProperty("data_phone")]
    public List<DataPhoneType> dataPhone;
    [JsonProperty("data_tablet")]
    public List<DataPhoneType> dataTablet;
}

[System.Serializable]
public class DataPhoneType
{
    [JsonProperty("lang")]
    public string language;
    [JsonProperty("banners")]
    public List<Banner> arrayBanners;
}


[System.Serializable]
public class Banner
{
    [JsonProperty("ban_id")]
    public string banId;
    [JsonProperty("banner")]
    public string ImageName;
    [JsonProperty("ban_text")]
    public string bannerText;
    [JsonProperty("v")]
    public int version;
}




