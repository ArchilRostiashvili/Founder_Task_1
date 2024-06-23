using System.Collections.Generic;
using UnityEngine;
using BebiLibs;
using BebiLibs.ServerLoader;

public class BulkDownloader
{
    private DataCache _dataCache;
    private readonly Dictionary<string, Texture2D> _arrayImages;
    private int _loadedImageCount;
    private int _loadCount;
    private bool _isDone = false;
    public bool isDone => _isDone;

    public BulkDownloader(DataCache dataCache)
    {
        _dataCache = dataCache;
        _arrayImages = new Dictionary<string, Texture2D>();
        _loadedImageCount = 0;
        _loadCount = 0;
        _isDone = true;
    }

    public void UpdateDataCache()
    {
        _dataCache.UpdateLoadData();
    }


    //Returns CallBack With Successfully downloaded image count, and image dictionary with loaded images 
    public void Download(List<Banner> arrayBanners, System.Action<int, Dictionary<string, Texture2D>> CallBack_LoadEnds)
    {
        if (!_isDone)
        {
            Debug.LogWarning("Don't call this function twice while isDone property is false or create new instance of this class");
            return;
        }

        _isDone = false;
        int maxImageCount = arrayBanners.Count;
        _loadedImageCount = 0;
        _loadCount = 0;
        _arrayImages.Clear();

        for (int i = 0; i < arrayBanners.Count; i++)
        {
            int index = i;
            bool forceToLoad = arrayBanners[i].version > _dataCache.GetVersion(arrayBanners[i].ImageName);
            if (forceToLoad)
            {
                _dataCache.UpdateVersion(arrayBanners[i].ImageName, arrayBanners[i].version);
            }
            ManagerServerLoader.LoadImage(arrayBanners[index].ImageName, forceToLoad, (res, tex) =>
            {
                if (res == RequestStatus.Success)
                {
                    _loadedImageCount++;
                    _arrayImages.Add(arrayBanners[index].ImageName, tex);
                }
                _loadCount++;
                if (_loadCount > arrayBanners.Count - 1)
                {
                    _isDone = true;
                    CallBack_LoadEnds(_loadedImageCount, _arrayImages);
                }
            });
        }
    }
}