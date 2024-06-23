using BebiLibs;
using BebiLibs.RegistrationSystem.Core;
using BebiLibs.RemoteConfigSystem;
using BebiLibs.ServerConfigLoaderSystem;
using BebiLibs.ServerConfigLoaderSystem.Environment;
using System;
using System.Collections;
using UnityEngine;

namespace ServerConfigLoaderSystem
{
    public class ServerDataLoadManager : MonoBehaviour
    {
        [Header("Server Config Specific Data")]
        [SerializeField] private ServerEnvironmentConfig _environmentConfig;
        [SerializeField] private ServerConfigData _configData;
        [SerializeField] private ServerInitData _initData;
        [SerializeField] private AbstractUserData _abstractUserData;
        [SerializeField] private ModuleConfig _moduleConfig;

        private PersistentInteger _developmentEnvPref = new PersistentInteger("remote_development_environment_pref", 0);

        private bool _isFetchOperationInProgress = false;
        private bool _isDataRefetchRequested = false;

        public void Initialize()
        {
            _moduleConfig.ResetData();
            _initData.ResetData();
            _configData.LoadDataFromMemory();
            _initData.LoadDataFromMemory();
            _moduleConfig.LoadDataFromMemory();

            if (_developmentEnvPref != 0)
            {
                _configData.MarkAsDirty();
            }
        }

        public void StartFetchingData()
        {
            StartCoroutine(StartFetchingServerData());
        }

        public IEnumerator StartFetchingServerData()
        {
            if (_isFetchOperationInProgress)
            {
                _isDataRefetchRequested = true;
                yield break;
            }
            _isFetchOperationInProgress = true;

            yield return StartLoadingDataFromServer();
            _isFetchOperationInProgress = false;

            if (_isDataRefetchRequested)
            {
                _isDataRefetchRequested = false;
                yield return StartFetchingServerData();
            }
        }

        private IEnumerator StartLoadingDataFromServer()
        {
            BaseRequestHandler.SetUserData(_abstractUserData);
            BaseRequestHandler.SetInitData(_initData);

            _environmentConfig.Initialize(_developmentEnvPref == 1);

            if (!_configData.isLoaded)
            {
                yield return _configData.GetDataFromServer(_environmentConfig.ConfigURL);
            }

            if (!_configData.isLoaded)
            {
                yield break;
            }

            string initializationURL = _environmentConfig.GetInitializationURL(_configData.urlEndpoint);
            yield return _initData.GetDataFromServer(initializationURL);

            if (_initData.isLoaded && !_moduleConfig.isLoaded)
            {
                yield return _moduleConfig.GetDataFromServer(_initData.config);
            }

            float remoteConfigWaitTime = RemoteConfigManager.TimeOutInSeconds + 1f;
            yield return new WaitForDone(remoteConfigWaitTime, () => RemoteConfigManager.IsLoadFinished);

            if (RemoteConfigManager.TryGetBool("server_dev_env", out bool isDevelopmentEnvEnabled))
            {
                if (_developmentEnvPref == 1 && !isDevelopmentEnvEnabled)
                {
                    _developmentEnvPref.SetValue(-1);
                }
                else
                {
                    _developmentEnvPref.SetValue(isDevelopmentEnvEnabled ? 1 : 0);
                }
            }
        }

        private void OnDestroy()
        {
            _configData.ResetData();
        }
    }

    public enum ServerLoadStatus
    {
        UNKNOWN, FAILED, SUCCEEDED
    }
}
