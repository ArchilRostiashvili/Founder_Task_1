using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BebiLibs.Localization
{
    [System.Serializable]
    public class AudioAssetReference
    {
        public string AssetName;
        public string LocalAssetName;
        public AssetReference AddressableAudioReference;

        [System.NonSerialized]
        public AudioLoadingStatus AudioLoadingStatus = AudioLoadingStatus.Unknown;

        [Header("Not Implemented yet: ")]
        public bool PreloadAsset = false;
        public bool HasAssetReference = false;

        public AudioAssetReference(string assetName)
        {
            AssetName = assetName;
            HasAssetReference = false;
        }

        public AudioAssetReference(string assetName, string assetNameLocal, string guid)
        {
            AssetName = assetName;
            LocalAssetName = assetNameLocal;

            AddressableAudioReference = new AssetReference(guid);
            HasAssetReference = true;
        }

        public static implicit operator AssetReference(AudioAssetReference d) => d.AddressableAudioReference;
    }
}

