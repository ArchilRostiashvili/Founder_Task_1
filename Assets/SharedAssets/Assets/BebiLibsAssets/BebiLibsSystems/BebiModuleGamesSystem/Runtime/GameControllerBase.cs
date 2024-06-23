using BebiLibs;
using UnityEngine;
using BebiLibs.PurchaseSystem.Core;
using BebiLibs.ModulesGameSystem;
using BebiLibs.Analytics.GameEventLogger;
using System;

public abstract class GameControllerBase : MonoBehaviour
{
    public System.Action<ModuleGameData, ModuleData> FinishLoadingSceneEvent;
    public System.Action<ModuleGameData, ModuleData> StartGameplayEvent;
    public System.Action<ModuleGameData, ModuleData, GameParameterBuilder> ExitFromGameplayEvent;
    public System.Action<ModuleGameData, ModuleData> UnloadGameplaySceneEvent;

    public void StartGame(IModuleStartData moduleStartData)
    {
        StartModuleInternal(moduleStartData);
    }

    internal abstract void StartModuleInternal(IModuleStartData moduleStartData);

    public System.Action RequestSubscriptionPopupEvent;

    [SerializeField] protected ModuleData _moduleData;
    [SerializeField] protected AbstractExitButton _exitButtonPrefab;
    [SerializeField] protected float _loadGameDelay = 0.3f;
    [SerializeField] private bool _isGameUnlocked = false;


    protected AbstractExitButton _exitButtonInstance;
    protected ModuleGameData _activeModuleGame;
    protected bool _isInitialized = false;

    public ModuleData ModuleIdentifier => _moduleData;
    public AbstractExitButton ExitButtonPrefab => _exitButtonPrefab;

    public bool IsGameUnlocked => _isGameUnlocked;

    public virtual void Initialize(ModuleData moduleData, AbstractExitButton abstractExitButton, bool isGameUnlocked)
    {
        _exitButtonPrefab = abstractExitButton;
        _isGameUnlocked = isGameUnlocked;
        _moduleData = moduleData;
        _isInitialized = true;
    }
}

public abstract class GameControllerBase<T> : GameControllerBase where T : IModuleStartData
{
    protected abstract void StartModuleGameplay(T moduleStartData);

    internal override void StartModuleInternal(IModuleStartData moduleStartData)
    {
        if (moduleStartData == null)
        {
            Debug.LogError($"Passed {nameof(IModuleStartData).Colorize("red", true)} cannot be null");
            return;
        }

        try
        {
            CastAndStartGame(moduleStartData);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{GetErrorMessage(moduleStartData)}, \nError: {e.Message}");
        }
    }

    private void CastAndStartGame(IModuleStartData moduleStartData)
    {
        T data = (T)moduleStartData;

        if (data == null)
        {
            Debug.LogError(GetErrorMessage(moduleStartData));
            return;
        }

        StartModuleGameplay(data);
    }

    public string GetErrorMessage(IModuleStartData moduleStartData)
    {
        string typeName = typeof(T).Name.Colorize("cyan", true);
        string interfaceName = nameof(IModuleStartData).Colorize("red", true);
        string classType = nameof(GameControllerBase).Colorize("red", true);

        string errorMessage = $"Unable to cast {interfaceName} as {typeName} inside {classType} class\n";
        errorMessage += moduleStartData.ModuleIdentifier != null ? $"Passed module: {moduleStartData.ModuleIdentifier.name.Colorize("red", true)}\n" : "Passed module is null\n";
        errorMessage += moduleStartData.ModuleGame != null ? $"Passed module game: {moduleStartData.ModuleGame.name.Colorize("red", true)}\n" : "Passed module game is null\n";
        return errorMessage;
    }

}
