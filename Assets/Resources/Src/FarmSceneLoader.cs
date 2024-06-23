using System;
using System.Collections;
using BebiLibs;
using BebiLibs.AudioSystem;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmLife
{
    public class FarmSceneLoader : MonoBehaviour
    {
        [SerializeField] private FeelAnimator _feelAnimator;
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _exitButton;
        [SerializeField] private SceneReference _lobbyReference;
        private string _currentScene;
        private static FarmSceneLoader _Instance;

        private void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
                return;
            }
            else if (_Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public static void LoadScene(string sceneName, string SceneToUnload, Action SceneLoadEvent = null, Action AfterAnimationEvent = null)
        {
            _Instance._exitButton.SetActive(false);
            _Instance._feelAnimator.Play(AnimationNamesData.ANIM_SHOW, () =>
            {
                _Instance.LoadNewScene(sceneName, SceneToUnload, SceneLoadEvent, AfterAnimationEvent);
            });
        }

        internal void DeactivateExitButton()
        {
            _Instance._exitButton.SetActive(false);
        }

        public static void UnloadLevel(string sceneToLoad, Action AfterSceneUnLoadEvent, Action AfterAnimationEvent = null)
        {
            MMSoundManager.Instance.StopAllSounds();
            AudioManager.StopAllSounds();
            if (_Instance._currentScene == null)
            {
                Debug.LogWarning("currentScene is null");
            }

            _Instance._exitButton.SetActive(false);

            _Instance._feelAnimator.Play(AnimationNamesData.ANIM_SHOW, () =>
            {
                _Instance.LoadNewScene(sceneToLoad, _Instance._currentScene, AfterSceneUnLoadEvent, AfterAnimationEvent);
            });
        }

        private void LoadNewScene(string sceneName, string sceneToUnload, Action AfterSceneLoadsAction = null, Action AfterAnimationEvent = null)
        {
            StopAllCoroutines();
            StartCoroutine(LoadSceneCoroutine());

            IEnumerator LoadSceneCoroutine()
            {
                if (CheckForScene(sceneToUnload))
                    yield return UnLoadSceneAsync(sceneToUnload);

                _camera.gameObject.SetActive(true);
                yield return LoadSceneAsync(sceneName);
                _camera.gameObject.SetActive(false);

                AfterSceneLoadsAction?.Invoke();
                _Instance._currentScene = sceneName;
                _feelAnimator.Play(AnimationNamesData.ANIM_HIDE, () =>
                    {
                        if (sceneName != _lobbyReference.SceneName)
                            _exitButton.SetActive(true);

                        AfterAnimationEvent?.Invoke();
                    });
            }
        }

        private bool CheckForScene(string sceneToUnload)
            => SceneManager.GetSceneByName(sceneToUnload).isLoaded;

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!asyncOperation.isDone)
            {
                yield return null;
            }
        }

        private IEnumerator UnLoadSceneAsync(string sceneName)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName);

            while (!asyncOperation.isDone)
            {
                yield return null;
            }

            Resources.UnloadUnusedAssets();
        }
    }
}