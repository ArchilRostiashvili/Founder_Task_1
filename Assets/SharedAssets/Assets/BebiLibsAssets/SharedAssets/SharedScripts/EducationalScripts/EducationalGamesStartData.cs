using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EducationalGamesStartData : ScriptableObject
{
    [field: SerializeField] public string GameName { get; private set; }
    [field: SerializeField] public GameObject ScenePrefab { get; private set; }
    [field: SerializeField] public GameContentBaseSO ContentSO { get; private set; }
    [field: System.NonSerialized] public Sprite Thumbnail { get; private set; }
}
