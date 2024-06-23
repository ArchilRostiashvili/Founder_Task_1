using System.Collections;
using System.Collections.Generic;
using BebiLibs.AudioSystem;
using UnityEngine;

namespace FarmLife.MiniGames.TractorFruit
{
    [System.Serializable]
    public class FruitData
    {
        public Sprite FruitSprite;
        public Color FruitColor;
        public string FruitID;
        public AudioTrackBaseSO FruitAudio;
    }
}