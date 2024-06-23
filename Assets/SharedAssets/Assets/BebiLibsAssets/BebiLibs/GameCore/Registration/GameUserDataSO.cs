using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.RegistrationSystem.Core
{
    [CreateAssetMenu(fileName = "RegistrationUserDataSO", menuName = "BebiLibs/RegistrationSystem/RegistrationUserDataSO", order = 0)]
    public class GameUserDataSO : AbstractUserData
    {
        static Newtonsoft.Json.Serialization.IContractResolver _resolver = new FixedUnityTypeContractResolver();
        public event System.Action<GameUserDataSO> CallBack_OnUserChanged;
        public const string UserDataPrefID = "RegistrationUserDataPrefID";

        [Header("User Token Info:")]
        [SerializeField] private UserSignInToken _userToken;
        [SerializeField] private bool _signUserOutOnNextStartUp = false;
        [SerializeField] private int _signInProvider = 0;

        public override bool isUserSignedIn => _userToken.isUserSignedIn;

        public override string userEmail => _userToken.userEmail;
        public override string userName => _userToken.userName;
        public override string userID => _userToken.userID;
        public override string rawToken => _userToken.rawToken;
        public override TimeSpan tokenExpiresIn => _userToken.tokenExpiresIn;

        public bool canRefreshUserToken => _userToken.canRefreshUserToken;
        public bool isBindButtonEnabled { get; set; } = false;
        public bool isTokenTransferred { get; set; } = false;

        public Provider singInProvider
        {
            get => (Provider)_signInProvider;
            set
            {
                //Debug.Log("Set Provider: " + value);
                _signInProvider = (int)value;
                SaveDataToMemory();
            }
        }

        public void SetUserSignInDirty() => _signUserOutOnNextStartUp = true;

        public void UpdateUserState()
        {
            if (isUserSignedIn == false || _signUserOutOnNextStartUp)
            {
                _signUserOutOnNextStartUp = false;
                SignUserOut();
            }
        }

        public bool UpdateUserToken(string userToken)
        {
            UserSignInToken userSignInToken = new UserSignInToken(userToken);

            if (userSignInToken.isValid && userSignInToken.isUserSignedIn)
            {
                _userToken = userSignInToken;
                SaveDataToMemory();
                CallBack_OnUserChanged?.Invoke(this);
            }
            else
            {
                Debug.LogError($"User Token Is Not Valid: IsValid: {userSignInToken.isValid}, IsSignedIn {userSignInToken.isUserSignedIn}");
                SignUserOut();
            }


            return userSignInToken.isValid && userSignInToken.isUserSignedIn;
        }

        private void SaveDataToMemory()
        {
            try
            {
                string userDataJson = JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings()
                {
                    ContractResolver = _resolver
                });
                PlayerPrefs.SetString(UserDataPrefID, userDataJson);
                //Debug.LogWarning("Save User Data: " + userDataJson);
                PlayerPrefs.Save();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Unable To Save User Data To Memory, Error: " + e);
            }
        }

        private void LoadDataFromMemory()
        {
            try
            {
                ResetData();
                string DefaultDataJson = JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings()
                {
                    ContractResolver = _resolver
                });

                string loadedUserDataString = PlayerPrefs.GetString(UserDataPrefID, DefaultDataJson);
                //Debug.LogWarning("Load User Data: " + loadedUserDataString);
                JsonConvert.PopulateObject(loadedUserDataString, this, new JsonSerializerSettings()
                {
                    ContractResolver = _resolver
                });
            }

            catch (System.Exception e)
            {
                Debug.LogError("Unable To Load User Data from Memory, Error: " + e);
            }
        }

        public void SignUserOut()
        {
            ResetData();
            SaveDataToMemory();
            CallBack_OnUserChanged?.Invoke(this);
        }

        public void ResetData()
        {
            _userToken.ResetData();
            isBindButtonEnabled = false;
        }


        public override bool GetRawUserToken(out string rawUserToken)
        {
            rawUserToken = _userToken.rawToken;
            return isUserSignedIn;
        }

        public void RefreshDataOnInitialize()
        {
            ResetData();
            LoadDataFromMemory();
        }

        public override string ToString()
        {
            return _userToken.ToString();
        }


    }
#if UNITY_EDITOR
    [CustomEditor(typeof(GameUserDataSO))]
    public class RegistrationUserDataSOEditor : Editor
    {
        private GameUserDataSO _registrationUserDataSO;

        private void OnEnable()
        {
            _registrationUserDataSO = (GameUserDataSO)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Test Token Serialization"))
            {
                //_registrationUserDataSO.userToken = new UserSignInToken(_registrationUserDataSO.userToken.rawToken);

                // string date = "2022-02-21T09:56:21";
                // SDateTime dateTime = (SDateTime)date;
                // Debug.Log(dateTime);
            }
        }
    }
#endif
}
