using BebiLibs.Localization.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace BebiLibs.Localization
{
    public class AddressableAudioManager : ManagerAudioAddressableBase
    {
        private static AddressableAudioManager _Instance;

        private static LoadState _LoadState = LoadState.NONE;

        private static AddressableAudioConfig _AddressableAudioData;

        [SerializeField] private LocalAudioData _ActiveLocalLanguage;
        [SerializeField] private List<AudioClip> _RuntimeAudioFilesList = new List<AudioClip>();
        [SerializeField] private List<AudioClip> _IgnoreAudioList = new List<AudioClip>();
        private static LanguageIdentifier _ActiveLanguage;

        public static AddressableAudioManager Instance => GetDefaultInstance();

        public static event System.Action OnAudioAddressableAssetLoadEndEvent;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeBeforeSceneLoad()
        {
            GetDefaultInstance();
        }

        public static bool GetDataSound(string soundName, out DataSound dataSound)
        {
            Initialize();
            dataSound = null;
            if (LocalizationManager.IsEnglish() || _LoadState != LoadState.FINISHED || string.IsNullOrEmpty(soundName) || !soundName.StartsWith("fx_tx") || Instance._IgnoreAudioList.Any(x => x.name == soundName))
            {
                return false;
            }

            dataSound = Instance._ActiveLocalLanguage.GetDataSound(soundName);

            if (dataSound == null)
            {
                dataSound = _AddressableAudioData.FindNonLocalizedSound(soundName);
            }
#if UNITY_EDITOR
            if (dataSound == null)
                Debug.LogWarning($"Sound: \"{soundName}\" don't have localized translation, Silencing");
#endif
            return true;
        }

        public static void UpdateLocalizationData(DataSoundsLib dataSoundsLib, bool isTemp)
        {
            Initialize();

            IEnumerable<AudioClip> dataSoundAudioList = dataSoundsLib.GetAllAudioName();
            AddRuntimeAudioFileList(dataSoundAudioList);
            LoadAudioClips(dataSoundAudioList, Instance._ActiveLocalLanguage, isTemp);
        }

        private void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (_Instance != this)
            {
                Destroy(gameObject);
            }
        }


        private static void Initialize()
        {
            if (_LoadState != LoadState.NONE)
            {
                return;
            }

            _LoadState = LoadState.STARTED;
            LocalizationManager.OnLanguageChangeEvent -= LoadLanguageAudioData;
            LocalizationManager.OnLanguageChangeEvent += LoadLanguageAudioData;
            _AddressableAudioData = AddressableAudioConfig.DefaultInstance();
            LoadLanguageAudioData(LocalizationManager.ActiveLanguage);
        }

        private static AddressableAudioManager GetDefaultInstance()
        {
            if (_Instance == null)
            {
                GameObject obj = new GameObject("Manager Audio Addressable Instance");
                _Instance = obj.AddComponent<AddressableAudioManager>();
            }
            return _Instance;
        }

        private static void AddRuntimeAudioFileList(IEnumerable<AudioClip> audioList)
        {
            foreach (var item in audioList)
            {

                if (!Instance._RuntimeAudioFilesList.Contains(item))
                {
                    Instance._RuntimeAudioFilesList.Add(item);
                }
            }
        }

        private static void LoadLanguageAudioData(LanguageIdentifier languageIdentifier)
        {
            if (!Application.isPlaying) return;

            if (_LoadState == LoadState.NONE)
            {
                Initialize();
                Debug.Log("Unable To Change Localization Sound, Manager Audio Is Not Initialized");
                return;
            }

            if (_ActiveLanguage != languageIdentifier)
            {
                _ActiveLanguage = languageIdentifier;


                if (Instance._ActiveLocalLanguage != null)
                {
                    Instance._ActiveLocalLanguage.UnloadResources();
                    Instance._ActiveLocalLanguage = null;
                }

                if (LocalizationManager.IsEnglish()) return;

                LocalAudioData dataLocalLanguage = _AddressableAudioData.GetAllLocalByIdentifier(languageIdentifier);
                Instance._ActiveLocalLanguage = dataLocalLanguage;
                AddRuntimeAudioFileList(_AddressableAudioData.AssetsToPreload);
                LoadAudioClips(Instance._RuntimeAudioFilesList, Instance._ActiveLocalLanguage, false);
            }
        }

        private static void LoadAudioClips(IEnumerable<AudioClip> audioClipsToLocalize, LocalAudioData sourceData, bool isTemp)
        {
            if (LocalizationManager.IsEnglish()) return;

            if (sourceData == null)
            {
                Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, $"#AddressableAudioManager.LoadAudioClips: Error - {nameof(sourceData)} is not loaded");
                return;
            }

            List<AssetReference> audioAssetsToLoad = new List<AssetReference>();
            List<AudioAssetReference> audioReferenceList = sourceData.AudioAssetReferenceList;
            foreach (AudioClip audioClip in audioClipsToLocalize)
            {
                AudioAssetReference assetReference = audioReferenceList.Find(x => x != null && audioClip != null && x.AssetName == audioClip.name);
#if UNITY_EDITOR
                if (assetReference == null)
                {
                    if (audioClip == null)
                    {
                        Debug.LogWarning($"{nameof(assetReference)} is null because AudioClip is null");
                    }
                    else if (audioClip != null && _AddressableAudioData.FindNonLocalizedSound(audioClip.name) == null)
                    {
                        string failureReason = $"{audioClip.name} not found in {nameof(audioClipsToLocalize)} list";
                        Debug.LogWarning($"{nameof(assetReference)} is null because {failureReason}");
                    }
                }
#endif

                if (assetReference != null && assetReference.AudioLoadingStatus == AudioLoadingStatus.Unknown)
                {
                    assetReference.AudioLoadingStatus = AudioLoadingStatus.LoadStarted;
                    audioAssetsToLoad.Add(assetReference.AddressableAudioReference);
                }
            }

            Instance.StartCoroutine(loadAudioClip(audioAssetsToLoad));

            IEnumerator loadAudioClip(IList<AssetReference> assetReferences)
            {                   //Debug.Log("StartLoading " + sourceData.Identifier.LanguageID);
                AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = Addressables.LoadResourceLocationsAsync(assetReferences, Addressables.MergeMode.Union);
                yield return locationsHandle;
                AsyncOperationHandle<IList<AudioClip>> objectsHandle = Addressables.LoadAssetsAsync<AudioClip>(locationsHandle.Result, null);
                yield return objectsHandle;
                OnLocalDataFinishLoading(sourceData, objectsHandle, isTemp);
            }
        }

        private static void OnLocalDataFinishLoading(LocalAudioData localAudio, AsyncOperationHandle<IList<AudioClip>> asyncOperationHandle, bool isTemp)
        {
            //Debug.Log("OnLocalDataFinishLoading " + localAudio.Identifier.LanguageID + ", Count: " + asyncOperationHandle.Result.Count);

            localAudio.AddLoadedAudioClip(asyncOperationHandle, isTemp);
            _LoadState = LoadState.FINISHED;
            OnAudioAddressableAssetLoadEndEvent?.Invoke();
        }

        public override void UpdateLocalizationLibrary(DataSoundsLib dataSoundsLib, bool isTemp)
        {
            UpdateLocalizationData(dataSoundsLib, isTemp);
        }

        public override bool GetLocalizedAudio(string soundName, out DataSound dataSound)
        {
            return GetDataSound(soundName, out dataSound);
        }

        public static void SetIgnoreAudioList(IEnumerable<AudioClip> values)
        {
            Initialize();
            foreach (var item in values)
            {
                Instance._IgnoreAudioList.Add(item);
            }
        }

        public static void ClearIgnoreAudioList()
        {
            Initialize();
            Instance._IgnoreAudioList.Clear();
        }

        public static void RemoveFromIgnoreAudioList(IEnumerable<AudioClip> values)
        {
            Initialize();
            foreach (var item in values)
            {
                Instance._IgnoreAudioList.Remove(item);
            }
        }


        public enum LoadState
        {
            NONE, STARTED, FINISHED
        }
    }
}
