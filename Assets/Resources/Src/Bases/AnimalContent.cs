using Bebi.FarmLife;
using BebiLibs.AudioSystem;
using UnityEngine;

namespace FarmLife
{
    [System.Serializable]
    public class AnimalContent
    {
        public string AnimalID;
        public FarmAnimalAnimator Animal;
        public Vector3 AnimalSize = Vector3.one;
        public FarmAnimalAnimator SmallAnimal;
        public Vector3 SmallAnimalSize = Vector3.one;
        public Sprite FoodSprite;
        public Sprite FoodSilhouette;
        public AudioClip AnimalSound;
        public AudioTrackBaseSO AnimalSoundTrack;
        public AudioTrackBaseSO AnimalName;
    }
}