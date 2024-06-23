using System.Collections;
using System.Collections.Generic;
using BebiLibs.AudioSystem;
using UnityEngine;

namespace FarmLife.Data.MiniGame
{
    public class FarmMiniGameData : ScriptableObject
    {
        public AudioTrackBaseSO GameIntroTrack;
        public AudioTrackBaseSO CustomBackgroundTrack;
        public bool PlayQuestionEachRound;
    }
}