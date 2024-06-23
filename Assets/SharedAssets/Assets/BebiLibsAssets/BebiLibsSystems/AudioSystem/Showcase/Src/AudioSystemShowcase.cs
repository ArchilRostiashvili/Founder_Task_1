using BebiLibs.AudioSystem;
using BebiLibs.Localization;
using BebiLibs.Localization.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BebiLibs
{
    [ExecuteInEditMode]
    public class AudioSystemShowcase : MonoBehaviour
    {
        [Header("Audio Sequence")]
        public LanguageIdentifier LanguageSO;

        [SerializeField] private AudioTrackSequenceSO _moreQuestionSequence;

        [ObjectInspector]
        [SerializeField] private List<AudioTrackSO> _mathNumbersList = new List<AudioTrackSO>();
        [SerializeField] private TMP_Text _subtitlesText;

        [Header("Track")]
        [SerializeField] private AudioTrackBaseSO _localizedAudioTrack;

        [SerializeField] private List<LanguageIdentifier> _languageIdentifiers = new List<LanguageIdentifier>();
        [SerializeField] private TMP_Text _languageText;
        private int _activeLanguageIndex = 0;

        [SerializeField] AudioTrackSO _audioTrack;

        private void OnEnable()
        {
            _activeLanguageIndex = _languageIdentifiers.IndexOf(LocalizationManager.ActiveLanguage);
            UpdateLanguageText();
        }

        private void UpdateLanguageText()
        {
            _languageText.text = _languageIdentifiers[_activeLanguageIndex].LanguageName;
        }

        public void UnloadAssets()
        {
            Debug.LogWarning($"Start Unloading Unused Resources {Time.frameCount}");
            Resources.UnloadUnusedAssets();
        }

        public void UnloadTestScene()
        {
            if (SceneLoader.IsSceneLoaded("TestScene2"))
            {
                Debug.LogWarning($"Start Unloading \"TestScene2\" Scene, Frame {Time.frameCount}");
                SceneLoader.UnloadSceneAsync("TestScene2", () =>
                {
                    Debug.LogWarning("\"TestScene2\" Scene Unloaded");
                });
            }
        }

        public void LoadTestScene()
        {
            if (!SceneLoader.IsSceneLoaded("TestScene2"))
            {
                Debug.LogWarning($"Start Loading \"TestScene2\" Scene, Frame {Time.frameCount}");
                SceneLoader.LoadScene("TestScene2", LoadSceneMode.Additive, () =>
                {
                    Debug.LogWarning("\"TestScene2\" Scene loaded");
                });
            }
        }

        private IEnumerator WaitForFrame(int frameCount)
        {
            for (int i = 0; i < frameCount; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        public void PlayLocalizedAudio()
        {
            if (_localizedAudioTrack != null && _localizedAudioTrack.IsPlaying() == false)
            {
                _localizedAudioTrack.Play();
            }
            else
            {
                Debug.Log("Localized Audio is playing");
            }
        }

        public void PlayAudio()
        {
            var audioSource = AudioManager.GetPlayingAudioSource(_audioTrack);

            if (audioSource != null)
            {
                Debug.Log($"AudioSource {audioSource.name} is playing");
            }
            else
            {
                Debug.Log($"AudioSource is not playing");
                _audioTrack.Play();
            }
        }

        private void OnDisable()
        {
            _audioTrack.Stop();
        }

        public void PlaySequence()
        {
            List<AudioTrackSO> numbersToAskList = new List<AudioTrackSO>();

            numbersToAskList.Add(_mathNumbersList.GetRandomElement());
            numbersToAskList.Add(Utils.GetRandomUniqueFromLists(_mathNumbersList, numbersToAskList));

            _moreQuestionSequence.MargeSequence(numbersToAskList);
            _moreQuestionSequence.Play((text) =>
            {
                _subtitlesText.text = text;
            });
        }

        public void ChangeLanguage()
        {
            _activeLanguageIndex = (_activeLanguageIndex + 1) % _languageIdentifiers.Count;
            LocalizationManager.SetActiveLanguage(_languageIdentifiers[_activeLanguageIndex]);
            UpdateLanguageText();
        }
    }
}
