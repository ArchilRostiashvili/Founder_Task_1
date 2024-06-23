using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.GameApplicationConfig
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ApplicationConfigProvider", menuName = "BebiLibs/GameApplicationConfig/ApplicationConfigProvider", order = 0)]
    public class ApplicationConfigProvider : ScriptableConfig<ApplicationConfigProvider>
    {
        [SerializeField] private ApplicationConfig _applicationConfig;

        public ApplicationConfig CurrentConfig => _applicationConfig;

        public void SetActiveApplicationConfig(ApplicationConfig applicationConfig)
        {
            _applicationConfig = applicationConfig;
        }
    }
}
