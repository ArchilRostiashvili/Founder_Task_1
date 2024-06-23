using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class ManagerTime : MonoBehaviour
    {
        private static ManagerTime _instance = null;
        public static ManagerTime Instance
        {
            get
            {
                if (_instance == null)
                {
                    //Don't be a pussy, don't use Singletons
                    Debug.LogError("You got what you wanted, ha ha it's null");
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                _instance = this;
            }
            else if (_instance != this)
            {
                GameObject.Destroy(gameObject);
            }
        }

        private static void Initialize()
        {
            if (_instance == null && Application.isPlaying)
            {
                GameObject obj = new GameObject("Manager Time Auto Instance");
                _instance = obj.AddComponent<ManagerTime>();
                //Debug.LogWarning($"{nameof(ManagerTime)} was initialized automatically, this may cause unexpected behaviour");
            }
        }


        public static Coroutine CallOnMainThread(Func<bool> condition, System.Action action)
        {
            Initialize();
            return _instance.StartCoroutine(waitMainTread());
            IEnumerator waitMainTread()
            {
                yield return new WaitUntil(condition);
                action?.Invoke();
            }
        }

        public static Coroutine Delay(IEnumerator action)
        {
            Initialize();
            if (_instance != null)
            {
                return _instance.StartCoroutine(action);
            }
            else
            {
                Debug.LogError("Instance Of ManagerTime is null, Make Sure you instantiate ManagerTime current inside scene");
            }

            return null;
        }

        public static Coroutine DelayUntil(MonoBehaviour behaviour, float timeOut, Func<bool> pred, Action onComplete)
        {
            if (behaviour != null)
            {
                if (behaviour.gameObject.activeSelf && behaviour.gameObject.activeInHierarchy)
                {
                    return behaviour.StartCoroutine(WaitUntilDone());
                }
                else
                {
                    Debug.LogError($"Unable To Run action on gameobject {behaviour.name} which is not enabled", behaviour.gameObject);
                }
            }
            else
            {
                Debug.LogError("Passed component is null");
            }

            IEnumerator WaitUntilDone()
            {
                yield return new WaitForDone(timeOut, pred);
                onComplete?.Invoke();

            }
            return null;
        }


        public static Coroutine Delay(MonoBehaviour behaviour, IEnumerator enumerator)
        {
            if (behaviour != null)
            {
                if (behaviour.gameObject.activeSelf && behaviour.gameObject.activeInHierarchy)
                {
                    return behaviour.StartCoroutine(enumerator);
                }
                else
                {
                    Debug.LogError($"Unable To Run action on gameobject {behaviour.name} which is not enabled", behaviour.gameObject);
                }
            }
            else
            {
                Debug.LogError("Passed component is null");
            }

            return null;
        }

        public static Coroutine Delay(float delayTime, MonoBehaviour behaviour, Action action)
        {
            return behaviour.StartCoroutine(DelayCallBack(delayTime, action));
        }

        public static Coroutine Delay(float delayTime, Action action)
        {
            Initialize();
            if (_instance != null)
            {
                return _instance.StartCoroutine(DelayCallBack(delayTime, action));
            }
            else
            {
                Debug.LogError("Instance Of ManagerTime is null, Make Sure you instantiate ManagerTime current inside scene");
            }

            return null;
        }


        public static Coroutine DelayFrame(int frameTime, Action action)
        {
            Initialize();
            if (_instance != null)
            {
                return _instance.StartCoroutine(DelayCallBackFrameTime(frameTime, action));
            }
            else
            {
                Debug.LogError("Instance Of ManagerTime is null, Make Sure you instantiate ManagerTime current inside scene");
            }

            return null;
        }

        private static IEnumerator DelayCallBackFrameTime(int frameTime, Action action)
        {
            for (int i = 0; i < frameTime; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            action.Invoke();
        }

        public static Coroutine StartSelfCoroutine(IEnumerator enumerator)
        {
            Initialize();
            if (_instance != null)
            {
                return _instance.StartCoroutine(enumerator);
            }
            else
            {
                Debug.LogError("Instance Of ManagerTime is null, Make Sure you instantiate ManagerTime current inside scene");
            }
            return null;
        }


        private static void StartCoroutines(MonoBehaviour behaviour, List<IEnumerator> routines, float delay)
        {
            behaviour.StartCoroutine(MultiRoutineRunner());
            IEnumerator MultiRoutineRunner()
            {
                foreach (var item in routines)
                {
                    yield return item;
                    yield return new WaitForSeconds(delay);
                }
            }
        }

        private static IEnumerator DelayCallBack(float delayTime, Action action)
        {
            yield return new WaitForSeconds(delayTime);
            action.Invoke();
        }

        public static void StopCoroutines()
        {
            Initialize();
            if (_instance != null)
            {
                _instance.StopAllCoroutines();
            }
            else
            {
                Debug.LogError("Instance Of ManagerTime is null, Make Sure you instantiate ManagerTime current inside scene");
            }
        }


        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public static void StopSelfCoroutine(Coroutine textureLoadRoutine)
        {
            Initialize();
            if (_instance != null)
            {
                _instance.StopCoroutine(textureLoadRoutine);
            }
            else
            {
                Debug.LogError("Instance Of ManagerTime is null, Make Sure you instantiate ManagerTime current inside scene");
            }
        }
    }
}
