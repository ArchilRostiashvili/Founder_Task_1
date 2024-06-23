using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.RegistrationSystem
{
    using BebiLibs.RegistrationSystem.Core;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ProviderIconDataSO", menuName = "BebiLibs/RegistrationSystem/ProviderIconDataSO", order = 0)]
    public class ProviderIconDataSO : ScriptableObject
    {
        [SerializeField] private List<ProviderIcons> _arrayProviderIcons;
        public bool TryGetIcon(Provider provaider, out Sprite icon)
        {
            ProviderIcons spritePData = _arrayProviderIcons.Find(x => x.registrationProvaider == provaider);
            if (spritePData != null)
            {
                icon = spritePData.icon;
                return true;
            }

            icon = null;
            return false;
        }
    }
}
