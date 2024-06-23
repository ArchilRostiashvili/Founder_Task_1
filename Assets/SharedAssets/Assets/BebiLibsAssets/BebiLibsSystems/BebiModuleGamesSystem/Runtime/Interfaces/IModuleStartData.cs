using BebiLibs.Analytics.GameEventLogger;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ModulesGameSystem
{
    public interface IModuleStartData
    {
        public ModuleGameData ModuleGame { get; }
        public ModuleData ModuleIdentifier { get; }
        public Vector3 ClickPosition { get; }
        public List<IGameParameter> GetExtraAnalyticsParameters();
    }
}
