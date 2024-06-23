using BebiLibs.ServerConfigLoaderSystem.Core;
using BebiLibs.RegistrationSystem.Core;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine;
using BebiLibs.ServerConfigLoaderSystem;

namespace BebiLibs.RegistrationSystem
{
    [CreateAssetMenu(fileName = "TokenTransfer", menuName = "BebiLibs/RegistrationSystem/TokenTransfer", order = 0)]
    public class TokenTransferData : BaseRequestData
    {
        [JsonIgnore()]
        public GameUserDataSO _userData;

        public string token;
        public bool error;

        public override void ResetData()
        {
            token = string.Empty;
            error = false;
        }

        protected override bool ValidateDeserializedData()
        {
            return !error;
        }

        public override void ModifyRequestHeader(UnityWebRequest request)
        {
            base.ModifyRequestHeader(request);
            BaseRequestHandler.SetBearerToken(request, _userData);
        }
    }
}
