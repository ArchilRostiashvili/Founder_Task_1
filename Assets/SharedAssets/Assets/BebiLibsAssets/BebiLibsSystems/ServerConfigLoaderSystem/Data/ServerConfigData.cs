using BebiLibs.ServerConfigLoaderSystem.Core;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace BebiLibs.ServerConfigLoaderSystem
{
    [CreateAssetMenu(fileName = "RegistrationConfigSO", menuName = "BebiLibs/RegistrationSystem/RegistrationConfigSO", order = 0)]
    public class ServerConfigData : BaseRequestData
    {
        [SerializeField] private STimeSpan ttl;
        [SerializeField] private int v;
        [SerializeField] private SDateTime modifyTime = SDateTime.UtcMin;

        [JsonProperty("endpoint")]
        [SerializeField] private string _endpoint;

        [JsonProperty("transfer")]
        [SerializeField] private bool _transfer;

        [JsonIgnore()]
        [SerializeField] private bool _registrationEnabled = false;

        public void SetRegistrationActive(bool value)
        {
            _registrationEnabled = value;
        }

        public string urlEndpoint => _endpoint;
        public bool isRegistrationEnabled => _transfer && _registrationEnabled;


        public override void LoadDataFromMemory()
        {
            base.LoadDataFromMemory();
            if (System.DateTime.UtcNow - lastUpdateTime.DateTime > ttl.TimeSpan)
            {
                _isDataLoadSuccessfull = false;
            }
        }

        public void MarkAsDirty()
        {
            _isDataLoadSuccessfull = false;
        }

        public override void ResetData()
        {
            v = 0;
            ttl = STimeSpan.Zero;
            modifyTime = SDateTime.UtcMin;
            _endpoint = string.Empty;
            _transfer = false;
            _isDataLoadSuccessfull = false;
            _registrationEnabled = false;
        }

        protected override bool ValidateDeserializedData()
        {
            return v != 0;
        }

        public override void ModifyRequestHeader(UnityWebRequest request)
        {
            base.ModifyRequestHeader(request);
#if UNITY_EDITOR
            request.SetRequestHeader(BaseRequestHandler.AUTHORIZATION, "");
#endif
            request.certificateHandler = new CertPublicKey();
        }

        class CertPublicKey : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        }


    }
}
