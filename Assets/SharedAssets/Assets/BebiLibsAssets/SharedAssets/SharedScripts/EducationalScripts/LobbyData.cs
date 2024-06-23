using BebiLibs;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class LobbyData
{
    public readonly static DataLoadStatus None = DataLoadStatus.None;
    public readonly static DataLoadStatus Started = DataLoadStatus.Started;
    public readonly static DataLoadStatus Successfull = DataLoadStatus.Successfull;
    public readonly static DataLoadStatus Failed = DataLoadStatus.Failed;

    [SerializeField] public string GameName;
    [SerializeField] public Sprite ThumbnailSprite;
    [SerializeField] public bool HasCustomBackgroundMusic;
    [SerializeField] public AssetReferenceT<EducationalGamesStartData> AssetReference;
    [SerializeField] public bool PreloadAssets = false;

    [System.NonSerialized] public EducationalGamesStartData RuntimeAsset;
    [System.NonSerialized] public AsyncOperationHandle<EducationalGamesStartData> OperationHandler;
    [System.NonSerialized] public DataLoadStatus DataLoadStatus = None;
    private bool _markForUnload;

    public IEnumerator LoadEducationGameStartData(System.Object actionReceiver, System.Action<LobbyData> OnLoadComplete)
    {
        if (DataLoadStatus == Successfull)
        {
            OnLoadComplete?.Invoke(this);
            yield break;
        }

        if (DataLoadStatus == Started)
        {
            yield return new WaitForDone(5, () => DataLoadStatus != Started);
            OnLoadComplete?.Invoke(this);
            yield break;
        }

        OperationHandler = AssetReference.LoadAssetAsync();
        DataLoadStatus = Started;

        yield return OperationHandler;

        if (OperationHandler.Status == AsyncOperationStatus.Succeeded)
        {
            RuntimeAsset = OperationHandler.Result;
            DataLoadStatus = DataLoadStatus.Successfull;
        }
        else
        {
            DataLoadStatus = DataLoadStatus.Failed;
        }

        if (actionReceiver == null) yield break;
        OnLoadComplete?.Invoke(this);
    }

    public void UnloadLoadedAssets(bool unloadPreloadedAssets = false)
    {
        if ((PreloadAssets && !unloadPreloadedAssets) || DataLoadStatus == Started || DataLoadStatus == None) return;

        if (OperationHandler.IsValid())
        {
            Addressables.Release(OperationHandler);
        }

        RuntimeAsset = null;
        DataLoadStatus = None;
    }
}

public enum DataLoadStatus
{
    None, Started, Successfull, Failed
}