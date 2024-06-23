using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace BebiLibs.ServerLoader
{
    public class ManagerServerLoader : MonoBehaviour
    {
        public const string dataCacheID = "imageDataCache";

        public static ManagerServerLoader Instance;
        private DataCache _dataCache = new DataCache(dataCacheID);
        public static DataCache dataCache => Instance._dataCache;
        public int timeOutInSeconds = 15;
        public static System.DateTime lastFetchedDataTime;
        public string rootUrl;

        private void Awake()
        {

            if (Instance == null)
            {
                Instance = this;
            }
            dataCache.UpdateLoadData();
        }

        public static void LoadImage(string imageName, bool forceLoadFromServer, System.Action<RequestStatus, Texture2D> CallBack_OnTextureLoad)
        {
            lastFetchedDataTime = System.DateTime.Now;
            if (forceLoadFromServer)
            {
                LoadImageFromServerAndSaveToStorage(imageName, CallBack_OnTextureLoad);
            }
            else
            {
                LoadTextureFromStorage(imageName, (result, texture) =>
                {
                    if (result == RequestStatus.Failed)
                    {
                        LoadImageFromServerAndSaveToStorage(imageName, CallBack_OnTextureLoad);
                    }
                    else
                    {
                        dataCache.UpdateTime(imageName, System.DateTime.Now);
                        CallBack_OnTextureLoad?.Invoke(RequestStatus.Success, texture);
                    }
                });
            }
        }

        //Load Image Directly from url and store it to file
        private static void LoadImageFromServerAndSaveToStorage(string imageName, System.Action<RequestStatus, Texture2D> CallBack_OnTextureLoad)
        {
            dataCache.UpdateTime(imageName, System.DateTime.Now);
            System.UriBuilder uriBuilder = new System.UriBuilder(Instance.rootUrl + "/" + imageName + ".png");
            LoadTextureFromURL(uriBuilder.Uri, imageName, true, CallBack_OnTextureLoad);
        }

        private static void RemoveImage(string imageName)
        {
            dataCache.RemoveImage(imageName);
            string filePath = Path.Combine(Application.persistentDataPath, imageName + ".png");
            File.Delete(filePath);
        }

        private static void CleanUpUnusedTextures()
        {
            for (int i = dataCache.Count - 1; i >= 0; i--)
            {
                if (dataCache[i].dateTime < lastFetchedDataTime.Subtract(System.TimeSpan.FromDays(3)))
                {
                    RemoveImage(dataCache[i].fileName);
                }
            }
        }

        private static void LoadTextureFromURL(System.Uri uri, string filename, bool saveToDisk, System.Action<RequestStatus, Texture2D> CallBack_OnTextureLoad)
        {
            Instance.StartCoroutine(GetTexture());
            IEnumerator GetTexture()
            {
                using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri))
                {
                    request.timeout = Instance.timeOutInSeconds;
                    yield return request.SendWebRequest();
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        DownloadHandlerTexture dht = (DownloadHandlerTexture)request.downloadHandler;
                        Texture2D tex = dht.texture;
                        CallBack_OnTextureLoad?.Invoke(RequestStatus.Success, tex);
                    }
                    else
                    {
                        CallBack_OnTextureLoad?.Invoke(RequestStatus.Failed, null);
                    }
                }

                if (saveToDisk)
                {
                    using (UnityWebRequest request = UnityWebRequest.Get(uri))
                    {
                        DownloadHandlerBuffer dH = (DownloadHandlerBuffer)request.downloadHandler;
                        yield return request.SendWebRequest();
                        if (request.result == UnityWebRequest.Result.Success)
                        {
                            SaveTextureToStorageAsync(request.downloadHandler.data, filename);
                        }
                    }
                }
            }
        }

        private static void LoadTextureFromStorage(string filename, System.Action<RequestStatus, Texture2D> CallBack_OnTextureLoad)
        {
            string filaPath = Path.Combine(Application.persistentDataPath, filename + ".png");
            if (File.Exists(filaPath))
            {
                System.UriBuilder uriBuilder = new System.UriBuilder(filaPath);
                LoadTextureFromURL(uriBuilder.Uri, filename, false, CallBack_OnTextureLoad);
            }
            else
            {
                CallBack_OnTextureLoad?.Invoke(RequestStatus.Failed, null);
            }
        }

        private static async void SaveTextureToStorageAsync(byte[] data, string filename, System.Action CallBack_ImageSaved = null)
        {
            string filePath = Path.Combine(Application.persistentDataPath, filename + ".png");
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            using FileStream SourceStream = File.Open(filePath, FileMode.OpenOrCreate);
            SourceStream.Seek(0, SeekOrigin.End);
            await SourceStream.WriteAsync(data, 0, data.Length);
            CallBack_ImageSaved?.Invoke();
        }

        private void OnDisable()
        {
            this.StopAllCoroutines();
        }
    }
}
