#if ACTIVATE_FIREBASE
#define IS_ACTIVE_FireBase
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if IS_ACTIVE_FireBase
using Firebase;
#endif


namespace BebiLibs
{
    public enum FirebaseLoadState { UNKNOWN, FAILED, SUCCEEDED }

    public class FirebaseDependencyResolver : MonoBehaviour
    {
        public delegate void FirebaseInitializationDelegate(bool isFirebaseAvailable);

        public static bool IsFirebaseAvailable { get; private set; } = false;
        public static bool IsInitializationFinished { get; private set; } = false;

        private static event FirebaseInitializationDelegate _FirebaseInitializationEvent;

        private static FirebaseLoadState _LoadStatus = FirebaseLoadState.UNKNOWN;

        public static void AddInitializationListener(FirebaseInitializationDelegate onFirebaseInitialize, bool immediate)
        {
            _FirebaseInitializationEvent += onFirebaseInitialize;

            if (immediate && IsInitializationFinished)
            {
                onFirebaseInitialize?.Invoke(IsFirebaseAvailable);
            }
        }

        public static void RemoveInitializationListener(FirebaseInitializationDelegate onFirebaseInitialize)
        {
            _FirebaseInitializationEvent -= onFirebaseInitialize;
        }

#if IS_ACTIVE_FireBase
        private void Start()
        {
            IsFirebaseAvailable = false;
            IsInitializationFinished = false;
            FixAndResolveDependencies();
        }

        public void FixAndResolveDependencies()
        {
            try
            {
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    if (task.Result == DependencyStatus.Available)
                    {
                        _LoadStatus = FirebaseLoadState.SUCCEEDED;
                    }
                    else
                    {
                        _LoadStatus = FirebaseLoadState.FAILED;
                    }
                });
            }
            catch
            {
                //Debug.LogError("Firebase Dependency Resolver class stopped due to error: " + e);
                _LoadStatus = FirebaseLoadState.FAILED;
            }
        }

        public void OnDependenciesFixedSuccess()
        {
            StartCoroutine(WaitForSecond());
            IEnumerator WaitForSecond()
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                enabled = false;
                IsFirebaseAvailable = true;
                IsInitializationFinished = true;
                _FirebaseInitializationEvent?.Invoke(true);
            }

        }

        public void OnDependenciesFixedFailed()
        {
            Debug.Log("Firebase Dependency Resolver Failed");
            StartCoroutine(WaitForSecond());
            IEnumerator WaitForSecond()
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                StopAllCoroutines();
                enabled = false;
                IsFirebaseAvailable = false;
                IsInitializationFinished = true;
                _FirebaseInitializationEvent?.Invoke(false);
            }
        }

        private void Update()
        {
            if (_LoadStatus == FirebaseLoadState.SUCCEEDED)
            {
                OnDependenciesFixedSuccess();
                _LoadStatus = FirebaseLoadState.UNKNOWN;
            }
            else
            if (_LoadStatus == FirebaseLoadState.FAILED)
            {
                OnDependenciesFixedFailed();
                _LoadStatus = FirebaseLoadState.UNKNOWN;
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

#else
        private void Awake()
        {
            IsInitializationFinished = true;
            IsFirebaseAvailable = false;
        }
#endif
    }
}
