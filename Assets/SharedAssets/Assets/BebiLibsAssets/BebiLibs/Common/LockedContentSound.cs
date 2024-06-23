using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs.PurchaseSystem;
using BebiLibs.Localization;

namespace BebiLibs
{
    [System.Flags]
    public enum PromoAudioMode
    {
        NONE = 0,
        PLAY = 1,
        FREE_TRIAL = 2,
        PLAY_FUll = 4
    }

    public class LockedContentSound : GenericSingletonClass<LockedContentSound>
    {

        private const string _FREE_TRIAL_AUDIO_KEY = "fx_tx_subscribe_try_for_free";
        private const string _FULL_EXPERIENCE_KEY = "fx_tx_subscribe_enjoy_full_experience";
        private const string _ASK_FOR_HELP = "fx_tx_subscribe_ask_an_adult_for_help";
        private const string _CONTENT_IS_LOCKED_KEY = "fx_tx_content_is_locked";

        private static bool _isWrongAudioIsPlaying = false;

        protected override void OnInstanceAwake()
        {
            _dontDestroyOnLoad = true;
        }

        protected override void OnInstanceDestroy()
        {
            StopAllCoroutines();
        }

        public static void Clear()
        {
            _isWrongAudioIsPlaying = false;
            ManagerSounds.PlayEffect(_CONTENT_IS_LOCKED_KEY, 0, 0.3f);
        }

        public static void Play(bool isContentLocked = true)
        {
            if (!_isWrongAudioIsPlaying)
            {
                _isWrongAudioIsPlaying = true;
                ItemAudioSource itemAudioSource = ManagerSounds.PlayEffect("fx_wrong7");
                if (itemAudioSource == null)
                {
                    _isWrongAudioIsPlaying = false;
                    Debug.Log("itemAudioSource is null");
                    return;
                }

                itemAudioSource.SetEndCallBack(() =>
                {
                    _isWrongAudioIsPlaying = false;
                });

                if (isContentLocked)
                {
                    ManagerSounds.PlayEffect(_CONTENT_IS_LOCKED_KEY, 0, 0.3f);
                }
            }
        }


        public static void PlaySubscribeVoice(PromoAudioMode promoAudioPlayMode, float delay = 0.1f)
        {
            ManagerSounds.StopSound(_CONTENT_IS_LOCKED_KEY, 0.2f);
            if (promoAudioPlayMode.HasFlag(PromoAudioMode.PLAY) || promoAudioPlayMode.HasFlag(PromoAudioMode.PLAY_FUll) || promoAudioPlayMode.HasFlag(PromoAudioMode.FREE_TRIAL))

                if (LocalizationManager.ActiveLanguage == LocalizationManager.English)
                {
                    bool isFreeTrial = promoAudioPlayMode.HasFlag(PromoAudioMode.FREE_TRIAL);
                    string audioToPlay = isFreeTrial ? _FREE_TRIAL_AUDIO_KEY : _FULL_EXPERIENCE_KEY;
                    float delayOffset = isFreeTrial ? 2.35f : 2.45f;
                    ManagerSounds.PlayEffect(audioToPlay, 0, delay);

                    bool playFullAudio = promoAudioPlayMode.HasFlag(PromoAudioMode.PLAY_FUll);
                    if (playFullAudio)
                    {
                        ManagerSounds.PlayEffect(_ASK_FOR_HELP, 0, delayOffset + delay);
                    }
                }
        }

        public static void StopPlayingSubscriptionVoices()
        {
            ManagerSounds.StopSound(_FREE_TRIAL_AUDIO_KEY, 0.1f);
            ManagerSounds.StopSound(_FULL_EXPERIENCE_KEY, 0.1f);
            ManagerSounds.StopSound(_ASK_FOR_HELP, 0.1f);
        }


    }
}
