using BebiLibs.ServerConfigLoaderSystem.Core;
using BebiLibs.RegistrationSystem.Core;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;
using BebiLibs.ServerConfigLoaderSystem;

namespace BebiLibs.RegistrationSystem
{


    [CreateAssetMenu(fileName = "SubscriptionPostDataSO", menuName = "BebiLibs/RegistrationSystem/SubscriptionPostDataSO", order = 0)]
    public class SubscriptionPostDataSO : BaseRequestData
    {
        [JsonIgnore()]
        [SerializeField] private GameUserDataSO _userData;

        public override void ResetData()
        {
            _isDataLoadSuccessfull = true;
        }

        protected override bool ValidateDeserializedData()
        {
            return true;
        }

        public override void ModifyRequestHeader(UnityWebRequest request)
        {
            base.ModifyRequestHeader(request);
            BaseRequestHandler.SetBearerToken(request, _userData);
        }
    }
}
