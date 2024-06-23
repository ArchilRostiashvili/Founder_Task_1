//#define ADMOB_ACTIVE

#if ADMOB_ACTIVE
using GoogleMobileAds;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BebiLibs
{
    public class ManagerAds : MonoBehaviour
    {
        //public const string AD_MOB_ID = "ca-app-pub-6815111077712958~8739197541";

        public static ManagerAds instance;

#if ADMOB_ACTIVE
        public InterstitialAd interstitialAd;
        public RewardedAd rewardedAd;
#endif

        public static System.Action CallBack_InterstitialLoadFailed;
        public static System.Action CallBack_InterstitialShowFailed;
        public static System.Action CallBack_InterstitialClosed;
        public static System.Action CallBack_InterstitialLoaded;
        public static System.Action CallBack_InterstitialOpen;

        public static System.Action CallBack_RewardVideoLoadFailed;
        public static System.Action CallBack_RewardVideoShowFailed;
        public static System.Action CallBack_RewardVideoClosed;
        public static System.Action CallBack_RewardVideoRewarded;
        public static System.Action CallBack_RewardVideoOpen;
        public static System.Action CallBack_RewardVideoLoaded;

        private bool _isInterstitialLoadFailed = false;
        private bool _isInterstitialShowFailed = false;
        private bool _isInterstitialClosed = false;
        private bool _isInterstitialLoaded = false;
        private bool _isInterstitialOpen = false;


        private bool _isRewardVideoLoadFailed = false;
        private bool _isRewardVideoShowFailed = false;
        private bool _isRewardVideoClosed = false;
        private bool _isRewardVideoRewarded = false;
        private bool _isRewardVideoOpen = false;
        private bool _isRewardVideoLoaded = false;

        public static bool isRewardAdLoaded = false;

        private PersistentString userID = new PersistentString("Player_UserID", System.Guid.NewGuid().ToString());

        public string statusText = "";

        public readonly List<string> testDeviceID = new List<string>()
        {
            "7A103DA311313F0B772B85016DEAD308",
            "C71E246F2C96C0A04D7D9709A62609DE",
            "D185150070EB6C69E97B0D2B50D4F592"
        };

        private void Awake()
        {

            instance = this;
        }


#if ADMOB_ACTIVE
        private void Start()
        { 

            MobileAds.SetiOSAppPauseOnBackground(true);

            List<string> deviceIds = new List<string>();

#if UNITY_IPHONE
            deviceIds.Add(this.userID);
#elif UNITY_ANDROID
            deviceIds.Add(this.userID);
#endif
            ;
            RequestConfiguration.Builder builder = new RequestConfiguration.Builder();
            builder.SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.True);
            builder.SetTestDeviceIds(this.testDeviceID);
            RequestConfiguration requestConfiguration = builder.build();

            MobileAds.SetRequestConfiguration(requestConfiguration);
            MobileAds.Initialize(this.HandleInitCompleteAction);
        }

        private void HandleInitCompleteAction(InitializationStatus initstatus)
        {
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                this.statusText = "Initialization complete";
                this.RequestAndLoadInterstitialAd();
                this.RequestAndLoadRewardedAd();
            });
        }
#endif

        public static bool IsRewordAdsLoaded()
        {
#if ADMOB_ACTIVE
            return instance.IsRewardedAdLoaded();
#else
            return false;
#endif
        }

        public static void ShowRewordVideo()
        {
#if ADMOB_ACTIVE
            instance.ShowRewardedAd();
#endif
        }

        private void Update()
        {
            ///////////////////////
            if (_isInterstitialLoadFailed)
            {
                CallBack_InterstitialLoadFailed?.Invoke();
                _isInterstitialLoadFailed = false;
            }
            if (_isInterstitialShowFailed)
            {
                CallBack_InterstitialShowFailed?.Invoke();
                _isInterstitialShowFailed = false;
            }
            if (_isInterstitialClosed)
            {
                CallBack_InterstitialClosed?.Invoke();
                _isInterstitialClosed = false;
                this.RequestAndLoadInterstitialAd();
            }
            if (_isInterstitialLoaded)
            {
                CallBack_InterstitialLoaded?.Invoke();
                _isInterstitialLoaded = false;
            }
            if (_isInterstitialOpen)
            {
                CallBack_InterstitialOpen?.Invoke();
                _isInterstitialOpen = false;
            }

            //////////////////////

            if (_isRewardVideoLoadFailed)
            {
                CallBack_RewardVideoLoadFailed?.Invoke();
                _isRewardVideoLoadFailed = false;
                isRewardAdLoaded = false;
            }
            if (_isRewardVideoShowFailed)
            {
                CallBack_RewardVideoShowFailed?.Invoke();
                _isRewardVideoShowFailed = false;
                isRewardAdLoaded = false;
            }
            if (_isRewardVideoClosed)
            {
                CallBack_RewardVideoClosed?.Invoke();
                _isRewardVideoClosed = false;
                this.LoadNewRewardAdd();
            }
            if (_isRewardVideoRewarded)
            {
                CallBack_RewardVideoRewarded?.Invoke();
                _isRewardVideoRewarded = false;
            }
            if (_isRewardVideoOpen)
            {
                CallBack_RewardVideoOpen?.Invoke();
                _isRewardVideoOpen = false;
            }

            if (_isRewardVideoLoaded)
            {
                isRewardAdLoaded = true;
                CallBack_RewardVideoLoaded?.Invoke();
                _isRewardVideoLoaded = false;
            }
        }


        public void RequestAndLoadRewardedAd()
        {
#if ADMOB_ACTIVE
            string adUnitId;
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-6815111077712958/1351855631";
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-6815111077712958/1351855631";
#else
            adUnitId = "unexpected_platform";
#endif

            this.statusText = "Requesting Rewarded Ad.";
            this.rewardedAd = new RewardedAd(adUnitId);
            this.rewardedAd.OnAdLoaded += (sender, args) => _isRewardVideoLoaded = true;
            this.rewardedAd.OnAdFailedToLoad += (sender, args) => _isRewardVideoLoadFailed = true;
            this.rewardedAd.OnAdOpening += (sender, args) => _isRewardVideoOpen = true;
            this.rewardedAd.OnAdFailedToShow += (sender, args) => _isRewardVideoShowFailed = true;
            this.rewardedAd.OnAdClosed += (sender, args) => _isRewardVideoClosed = true;
            this.rewardedAd.OnUserEarnedReward += (sender, args) => _isRewardVideoRewarded = true;
            this.rewardedAd.LoadAd(this.CreateAdRequest());
#endif
        }

        public void LoadNewRewardAdd()
        {
#if ADMOB_ACTIVE
            if (this.rewardedAd != null)
            {
                this.rewardedAd.LoadAd(this.CreateAdRequest());
            }
#endif
        }

        public bool IsRewardedAdLoaded()
        {
#if ADMOB_ACTIVE
            if (this.rewardedAd == null)
            {
                this.RequestAndLoadRewardedAd();
                return false;
            }
            if (this.rewardedAd.IsLoaded())
            {
                return true;
            }
#endif
            return false;
        }

        public void ShowRewardedAd()
        {
#if ADMOB_ACTIVE
            if (this.rewardedAd != null && this.rewardedAd.IsLoaded())
            {
                this.rewardedAd.Show();
            }
            else
            {
                this.statusText = "Rewarded ad is not ready yet.";
            }
#endif
        }


        public void RequestAndLoadInterstitialAd()
        {
#if ADMOB_ACTIVE
            string adUnitId;
#if UNITY_ANDROID
            adUnitId = "ca-app-pub-6815111077712958/7993640826";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-6815111077712958/7993640826";
#else
        string adUnitId = "unexpected_platform";
#endif

            this.statusText = "Requesting Interstitial Ad.";

            if (this.interstitialAd != null)
            {
                this.interstitialAd.Destroy();
            }
            this.interstitialAd = new InterstitialAd(adUnitId);

            this.interstitialAd.OnAdLoaded += (sender, args) => _isInterstitialLoaded = true;
            this.interstitialAd.OnAdFailedToLoad += (sender, args) => _isInterstitialLoadFailed = true;
            this.interstitialAd.OnAdOpening += (sender, args) => _isInterstitialOpen = true;
            this.interstitialAd.OnAdClosed += (sender, args) => _isInterstitialClosed = true;
            this.interstitialAd.LoadAd(this.CreateAdRequest());
#endif
        }

        public void LoadNewInterstitialAd()
        {
#if ADMOB_ACTIVE
            if (this.interstitialAd != null)
            {
                this.interstitialAd.LoadAd(this.CreateAdRequest());
            }
#endif
        }

#if ADMOB_ACTIVE
        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .AddKeyword("game")
                .Build();
        }
#endif


        public bool ShowInterstitialAd()
        {
#if ADMOB_ACTIVE
            if (this.interstitialAd == null)
            {
                instance.RequestAndLoadInterstitialAd();
                return false;
            }
            else if (this.interstitialAd.IsLoaded())
            {
                this.interstitialAd.Show();
                return true;
            }
#endif
            this.statusText = "Interstitial ad is not ready yet";
            return false;
        }

        public static bool ShowInterstitial()
        {
            if (instance != null)
            {
                return instance.ShowInterstitialAd();
            }
            return false;
        }

        public void DestroyInterstitialAd()
        {
#if ADMOB_ACTIVE
            if (this.interstitialAd != null)
            {
                this.interstitialAd.Destroy();
                this.interstitialAd = null;
            }
#endif
        }


        private void OnDestroy()
        {
            this.DestroyInterstitialAd();
        }
    }
}
