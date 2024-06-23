using BebiLibs;
using BebiLibs.ModulesGameSystem;
using BebiLibs.Analytics.GameEventLogger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleStartData : IModuleStartData
{
    private readonly ModuleGameData _moduleGameData;
    private readonly ModuleData _moduleIdentifier;
    private readonly Vector3 _clickPosition;
    private readonly List<IGameParameter> _gameParameters = new List<IGameParameter>();

    public ModuleStartData(ModuleGameData moduleGameData, ModuleData moduleIdentifier, Vector3 clickPosition)
    {
        _moduleGameData = moduleGameData;
        _moduleIdentifier = moduleIdentifier;
        _clickPosition = clickPosition;
    }

    public ModuleGameData ModuleGame => _moduleGameData;
    public ModuleData ModuleIdentifier => _moduleIdentifier;
    public Vector3 ClickPosition => _clickPosition;

    public List<IGameParameter> GetExtraAnalyticsParameters()
    {
        return _gameParameters;
    }
}
