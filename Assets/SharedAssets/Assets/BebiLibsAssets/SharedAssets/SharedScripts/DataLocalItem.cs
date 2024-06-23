using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataLocalItem", menuName = "Modules/BalloonAdventure/DataLocalItem", order = 0)]
[System.Serializable]
public class DataLocalItem : ScriptableObject
{
    public string id;
    public Sprite sprite;
    public Sprite sprite_small;
    public AudioClip sound;
}
