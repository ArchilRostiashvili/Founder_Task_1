using BebiLibs.ServerConfigLoaderSystem.Core;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.RegistrationSystem
{
    [CreateAssetMenu(fileName = "ManualSignInPostDataSO", menuName = "BebiLibs/RegistrationSystem/ManualSignInPostDataSO", order = 0)]
    public class ManualSignInPostDataSO : BaseRequestData
    {
        [JsonProperty("errors")]
        [SerializeField] private ErrorsDict _errors;

        [JsonProperty("token")]
        [SerializeField] private string _token;

        [JsonProperty("error")]
        [SerializeField] private bool _error;

        public string Token => _token;
        public bool HasError => _error;
        public ErrorsDict Errors => _errors;

        public override void ResetData()
        {
            _isDataLoadSuccessfull = false;
            _error = false;
            _token = string.Empty;
            _errors = new ErrorsDict();
        }

        protected override bool ValidateDeserializedData()
        {
            return true;
        }

        [System.Serializable]
        public class ErrorsDict
        {
            [JsonProperty("Username")]
            public string UsernameRelatedError;

            [JsonProperty("Password")]
            public string PasswordRelatedError;

            [JsonProperty("")]
            public string GeneralError;
        }
    }
}
