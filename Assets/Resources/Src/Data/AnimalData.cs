using Bebi.FarmLife;
using BebiLibs.AudioSystem;
using UnityEngine;

namespace FarmLife
{
    [System.Serializable]
    public class AnimalData
    {
        public FarmAnimalAnimator AnimalAnimator;
        public Sprite AnimalSprite;
        public AudioTrackBaseSO Sound;
        public AudioTrackBaseSO Name;
        public float ScaleUpValue = 1f;
    }
}