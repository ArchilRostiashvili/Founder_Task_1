using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BebiLibs.GameApplicationConfig
{
    [CreateAssetMenu(fileName = "ApplicationConfig", menuName = "BebiLibs/GameApplicationConfig/ApplicationConfig", order = 0)]
    public class ApplicationConfig : ScriptableObject, IApplicationConfig
    {
        [SerializeField] private string _configID;
        [SerializeField] private Sprite _appDisplayIconSprite;
        [SerializeField] private RuntimePlatform _runtimePlatform;
        [SerializeField] private bool _isDebugMode = false;

        [ObjectInspector(false)]
        [SerializeField] private StoreAccessConfig _storeAccessConfig;

        [ObjectInspector(false)]
        [SerializeField] private List<PlatformSpecificData> _platformSpecificDataList = new List<PlatformSpecificData>();

        [ObjectInspector(false)]
        [SerializeField] private List<ApplicationConfig> _targetAppSpecificDataList;

        private PersistentInteger _ageGroup = new PersistentInteger("Age_Group", 0);
        private PersistentBoolean _handSide = new PersistentBoolean("HandSide", true);

        public List<PlatformSpecificData> PlatformSpecificDataList => _platformSpecificDataList;
        public List<ApplicationConfig> TargetApplicationConfigList => _targetAppSpecificDataList;

        public Sprite AppDisplayIcon => _appDisplayIconSprite;

        private void OnEnable()
        {
#if UNITY_EDITOR && UNITY_ANDROID
            if (!_isDebugMode)
            {
                _runtimePlatform = RuntimePlatform.Android;
            }
#elif UNITY_EDITOR && UNITY_IOS
            if (!_isDebugMode)
            {
                _runtimePlatform = RuntimePlatform.IPhonePlayer;
            }
#else
            _runtimePlatform = Application.platform;
#endif
        }

        public int AgeGroup
        {
            get => _ageGroup.GetValue();
            set => _ageGroup.SetValue(value);
        }

        public bool HandSide
        {
            get => _handSide.GetValue();
            set => _handSide.SetValue(value);
        }

        public string UnlockDateKey() => _configID + "_UnlockDate";
        public string VersionKey() => _configID + "_Version";
        public string UserSignInStatusKey() => _configID + "_UserSignInStatus";
        public string UserTokenKey() => _configID + "_UserToken";
        public string UserEmailKey() => _configID + "_UserEmail";
        public string DeviceIDKey() => _configID + "_DeviceID";
        public string OpenToTargetAppKey() => _configID + "_OpenToNewApp";
        public string LockStateKey() => _configID + "_IsUnlocked";
        public string SubscriptionTokenKey() => _configID + "_SubToken";


        public StoreAccessConfig GetStoreAccessData() => _storeAccessConfig;

        public string GetStoreUrl()
        {
            if (TryGetPlatformSpecificData(_runtimePlatform, out PlatformSpecificData data))
            {
                return _storeAccessConfig.GetStoreURL(_runtimePlatform, data.StoreSpecificID);
            }
            return string.Empty;
        }

        public string GetAppShareURl()
        {
            if (TryGetPlatformSpecificData(_runtimePlatform, out PlatformSpecificData data))
            {
                return data.ShareLinksConfigData.GetLocalizedShareLink();
            }
            return string.Empty;
        }

        public NotificationConfig GetNotificationConfig()
        {
            if (TryGetPlatformSpecificData(_runtimePlatform, out PlatformSpecificData data))
            {
                return data.NotificationConfigData;
            }
            return null;
        }

        public string GetPrivacyPolicyUrl()
        {
            if (TryGetPlatformSpecificData(_runtimePlatform, out PlatformSpecificData data))
            {
                return data.PrivacyPolicyURL;
            }
            return string.Empty;
        }

        public string GetTermsOfUseUrl()
        {
            if (TryGetPlatformSpecificData(_runtimePlatform, out PlatformSpecificData data))
            {
                return data.TermsOfUseURL;
            }
            return string.Empty;
        }

        public string GetBundleID()
        {
            if (TryGetPlatformSpecificData(_runtimePlatform, out PlatformSpecificData data))
            {
                return data.BundleID;
            }
            return string.Empty;
        }

        public string GetOpenDeepLinkUrl(string from = null)
        {
            if (TryGetPlatformSpecificData(_runtimePlatform, out PlatformSpecificData data))
            {
                return data.BundleID + "://open" + (!string.IsNullOrEmpty(from) ? "?from=" + from : string.Empty);
            }
            return string.Empty;
        }

        public string GetInitDeepLinkUrl(string from = null)
        {
            if (TryGetPlatformSpecificData(_runtimePlatform, out PlatformSpecificData data))
            {
                return data.BundleID + "://init" + (!string.IsNullOrEmpty(from) ? "?from=" + from : string.Empty);
            }
            return string.Empty;
        }


        public string GetStoreSpecificID()
        {
            if (TryGetPlatformSpecificData(_runtimePlatform, out PlatformSpecificData data))
            {
                return data.StoreSpecificID;
            }
            return string.Empty;
        }

        public string GetGroupID()
        {
            if (TryGetPlatformSpecificData(_runtimePlatform, out PlatformSpecificData data))
            {
                return data.GroupID;
            }
            return string.Empty;
        }

        public string GetFullGroupID()
        {
            if (TryGetPlatformSpecificData(_runtimePlatform, out PlatformSpecificData data))
            {
                return data.FullGroupID;
            }
            return string.Empty;
        }

        public string GetTeamID()
        {
            if (TryGetPlatformSpecificData(_runtimePlatform, out PlatformSpecificData data))
            {
                return data.TeamID;
            }
            return string.Empty;
        }

        public string GetAssociatedDomain()
        {
            if (TryGetPlatformSpecificData(_runtimePlatform, out PlatformSpecificData data))
            {
                return data.AssociatedDomain;
            }
            return string.Empty;
        }

        private bool TryGetPlatformSpecificData(RuntimePlatform runtimePlatform, out PlatformSpecificData platformSpecific)
        {
            platformSpecific = _platformSpecificDataList.FirstOrDefault(x => x.Platform.Contains(runtimePlatform));
            if (platformSpecific == null)
            {
                Debug.LogWarning($"{nameof(PlatformSpecificData)} for {runtimePlatform} not found!");
                return false;
            }
            return true;
        }

        public bool TryGetPlatformSpecificData(out PlatformSpecificData platformSpecific)
        {
            platformSpecific = _platformSpecificDataList.FirstOrDefault(x => x.Platform.Contains(_runtimePlatform));
            if (platformSpecific == null)
            {
                Debug.LogWarning($"{nameof(PlatformSpecificData)} for {_runtimePlatform} not found!");
                return false;
            }
            return true;
        }
    }
}
