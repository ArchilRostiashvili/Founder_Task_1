using System.Collections;
using System.Collections.Generic;
using BebiLibs.RegistrationSystem.Core;
using UnityEngine;


namespace BebiLibs.RegistrationSystem
{

    [CreateAssetMenu(fileName = "UserEmailFormatter", menuName = "BebiLibs/RegistrationSystem/UserEmailFormatter", order = 0)]
    public class UserEmailFormatter : StringFormatBase
    {
        [SerializeField] private string _key;
        [SerializeField]
        private GameUserDataSO _gameUserDataSO;

        public override string key => _key;
        public override string value => _gameUserDataSO.userEmail;
    }
}
