using BebiLibs.AudioSystem;
using System;
using UnityEngine;

namespace FarmLife.MiniGames.SortingOfEggs
{
    [Serializable]
    public class EggColorData
    {
        public Color EggColor;
        public string ColorID;
        public AudioTrackBaseSO audioTrack;
    }
}