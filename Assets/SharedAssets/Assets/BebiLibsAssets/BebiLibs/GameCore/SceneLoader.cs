using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace BebiLibs
{
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader _Instance;
        private void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
            else if (_Instance != this)
            {
                GameObject.Destroy(gameObject);
            }
        }

        private static void Init()
        {
            if (_Instance == null)
            {
                GameObject obj = new GameObject("Scene Load Manager");
                obj.AddComponent<SceneLoader>();
            }
        }

        //may not work as expected by name though, ✌
        public static void StopLoadingAll()
        {
            Init();
            _Instance.StopAllCoroutines();
        }

        public static void UnloadScene(string name)
        {
            Init();
            if (IsSceneLoaded(name))
            {
                SceneManager.UnloadSceneAsync(name);
            }
            else
            {
                Debug.LogWarning("Scene is already unloaded");
            }
        }

        public static void UnloadSceneAsync(string name, System.Action sceneUnloadEvent = null)
        {
            Init();
            _Instance.StartCoroutine(Unload());
            IEnumerator Unload()
            {
                if (IsSceneLoaded(name))
                {
                    yield return SceneManager.UnloadSceneAsync(name);
                }
                sceneUnloadEvent?.Invoke();
            }
        }

        public static IEnumerator UnloadSceneAsync(string name)
        {
            Init();
            if (IsSceneLoaded(name))
            {
                yield return SceneManager.UnloadSceneAsync(name);
            }
            else
            {
                Debug.LogWarning("Scene is already unloaded");
            }
        }


        public static bool IsSceneLoaded(string name)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public static void UnloadScene(string name, IEnumerator onSceneUnloadAction = null)
        {
            Init();
            _Instance.StartCoroutine(Unload());
            IEnumerator Unload()
            {
                if (IsSceneLoaded(name))
                {
                    yield return SceneManager.UnloadSceneAsync(name);
                }
                else
                {
                    Debug.LogWarning($"Scene named: \"{name}\" is already unloaded");
                }
                yield return onSceneUnloadAction;
            }
        }


        public static void LoadScene(string name, LoadSceneMode loadScene = LoadSceneMode.Additive)
        {
            Init();
            SceneManager.LoadSceneAsync(name, loadScene);
        }

        public static void LoadScene(string name, LoadSceneMode loadScene = LoadSceneMode.Additive, IEnumerator loadCompleteAction = null, IEnumerator loadFailedAction = null)
        {
            Init();
            _Instance.StartCoroutine(load());
            IEnumerator load()
            {
                AsyncOperation sceneLoader = SceneManager.LoadSceneAsync(name, loadScene);
                yield return sceneLoader;
                if (sceneLoader != null && sceneLoader.isDone)
                {
                    yield return loadCompleteAction;
                }
                else
                {
                    Debug.LogError($"Failed To Load Scene named: {name}");
                    yield return loadFailedAction;
                }
            }
        }


        public static void LoadScene(string sceneName, LoadSceneMode loadScene = LoadSceneMode.Additive, System.Action sceneLoadDone = null, System.Action sceneLoadFailedEvent = null)
        {
            Init();
            _Instance.StartCoroutine(LoadSceneAsync(sceneName, loadScene, sceneLoadDone, sceneLoadFailedEvent));
        }

        public static IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadScene, System.Action sceneLoadDone, System.Action sceneLoadFailedEvent)
        {
            AsyncOperation sceneLoader = SceneManager.LoadSceneAsync(sceneName, loadScene);
            yield return sceneLoader;
            if (sceneLoader != null && sceneLoader.isDone)
            {
                sceneLoadDone?.Invoke();
            }
            else
            {
                Debug.LogError($"Failed To Load Scene named: {sceneName}");
                sceneLoadFailedEvent?.Invoke();
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}