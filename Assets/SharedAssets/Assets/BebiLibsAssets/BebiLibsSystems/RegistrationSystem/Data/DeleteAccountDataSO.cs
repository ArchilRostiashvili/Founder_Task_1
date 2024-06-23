using BebiLibs.ServerConfigLoaderSystem.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.RegistrationSystem
{
    [CreateAssetMenu(fileName = "DeleteAccountDataSO", menuName = "BebiLibs/RegistrationSystem/DeleteAccountDataSO", order = 0)]
    public class DeleteAccountDataSO : BaseRequestData
    {
        [SerializeField] private int storeId;
        [SerializeField] private string guideUrl;
        [SerializeField] private bool error;

        public bool hasError => error;
        public string GuideUrl => guideUrl;
        public int StoreId => storeId;

        public override void ResetData()
        {
            storeId = 0;
            guideUrl = string.Empty;
            error = false;
        }

        protected override bool ValidateDeserializedData()
        {
            return true;
        }
    }
}
