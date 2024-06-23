using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ModulesGameSystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ModuleGameSettings", menuName = "BebiLibs/GameModules/ModuleGameSettings", order = 0)]
    public class ModuleGameSettings : ScriptableObject
    {
        [SerializeField] private ModuleCollectionData _activeModuleCollectionData;

        public ModuleCollectionData ModuleCollectionData => _activeModuleCollectionData;

        public void SetModuleCollectionData(ModuleCollectionData newModuleCollectonData)
        {
            _activeModuleCollectionData = newModuleCollectonData;
        }
    }
}
