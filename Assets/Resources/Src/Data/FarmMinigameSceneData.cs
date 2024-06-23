using BebiLibs;
using FarmLife.MiniGames;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "FarmMinigameSceneData", menuName = "FarmLife/GameData/Scenes")]
public class FarmMinigameSceneData : ScriptableObject
{
    public SceneReference GameSceneReference;
    public string SceneName;
    public Sprite Thumbnail;

    private MiniGameBaseData _miniGameBaseData;

    public MiniGameBaseData MiniGameBaseData
    {
        get => _miniGameBaseData;
        set => _miniGameBaseData = value;
    }
}
