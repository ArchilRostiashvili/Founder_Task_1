using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.GameApplicationConfig
{
    [CreateAssetMenu(fileName = "PlatformSpecificData", menuName = "BebiLibs/ApplicationConfigs/PlatformSpecificData", order = 0)]
    public class PlatformSpecificData : ScriptableObject
    {
        [SerializeField] private List<RuntimePlatform> _platformList;

        [SerializeField] private string _dynamicLinkURL;
        [SerializeField] private string _appLinkHost;
        [SerializeField] private string _appLinkPathPrefix;
        [SerializeField] private string _associatedDomain;

        [Header("Privacy URLs")]
        [SerializeField] private string _privacyPolicyUrl;
        [SerializeField] private string _termsOfUseUrl;

        [Header("Bundle Data")]
        [SerializeField] private string _teamID;
        [SerializeField] private string _groupID;
        [SerializeField] private string _bundleID;
        [SerializeField] private string _storeSpecificID;

        [Header("Share Links")]

        [ObjectInspector(false)]
        [SerializeField] private ShareLinksConfig _shareLinkConfig;

        [Header("Notification Data")]
        [ObjectInspector(false)]
        [SerializeField] private NotificationConfig _notificationConfig;


        public List<RuntimePlatform> Platform => _platformList;

        public string PrivacyPolicyURL => _privacyPolicyUrl;
        public string TermsOfUseURL => _termsOfUseUrl;
        public string BundleID => _bundleID;
        public string StoreSpecificID => _storeSpecificID;
        public string TeamID => _teamID;
        public string GroupID => _groupID;
        public string FullGroupID => _teamID + "." + _groupID;
        public string DynamicLinkURL => _dynamicLinkURL;
        public string AppLinkHost => _appLinkHost;
        public string AppLinkPathPrefix => _appLinkPathPrefix;
        public string AssociatedDomain => _associatedDomain;

        public ShareLinksConfig ShareLinksConfigData => _shareLinkConfig;
        public NotificationConfig NotificationConfigData => _notificationConfig;

    }
}
