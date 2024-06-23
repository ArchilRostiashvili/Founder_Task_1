using BebiLibs.AudioSystem;
using EducationalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameContentBaseSO : ScriptableObject
{
    public AudioTrackBaseSO GameIntroTrack;
    public AudioTrackBaseSO CustomBackgroundTrack;
    public bool PlayQuestionEachRound;
    public int RoundsToPlay;

    protected int _roundIndex;
    public int RoundIndex
    {
        get
        {
            return _roundIndex;
        }
    }
    public abstract void Init();
    public abstract GameData GenerateContent();
}

[System.Serializable]
public class ContentInstanceData
{
    public GameObject Obj;
    public GameObject AdditionalObj;
    public Sprite MainSprite;
    public Sprite AdditionalSprite;
    public string MainText;
    public AudioTrackBaseSO MainTrack;
    public AudioTrackBaseSO AdditionalTrack;
    public ColorsSharedData ColorData;
    public string ContentID;
    public int ItemCount;
}

[System.Serializable]
public class RoundGenerator
{
    public GameContentBaseSO GeneratorToUse;
    public List<int> RoundsList = new List<int>();
}

[System.Serializable]
public class GameData
{
    public ContentInstanceData MainData;
    public ContentInstanceData ExtraData;
    public List<ContentInstanceData> ResultsList;
    public List<ContentInstanceData> TargetsList;
    public bool PlayQuestion;
    public string StageName;
    public bool IsLast;
    public int CorrectCount;
    public int RoundIndex;
}