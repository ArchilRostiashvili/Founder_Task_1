using BebiLibs;
using BebiLibs.AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEffectsHandler : MonoBehaviour
{
    [field: SerializeField] public List<ObjectAnimatorData> AvailableAnimationsList = new List<ObjectAnimatorData>();

    public void PlayAnimation(string animationID, System.Action onComplete = null, bool playSound = true, bool playEffect = true)
    {
        foreach (ObjectAnimatorData animationData in AvailableAnimationsList)
        {
            animationData.Animation.Stop(transform);
        }

        StopAllCoroutines();

        ObjectAnimatorData animationDataToPlay = AvailableAnimationsList.Find(x => x.ID == animationID);

        if(animationDataToPlay != null)
        {
            animationDataToPlay.Animation.Play(transform, ()=> 
            {
                onComplete?.Invoke();
            });

            if (playEffect)
            {
                foreach (ParticlesData particlesData in animationDataToPlay.ParticlesDataList)
                {
                    StartCoroutine(DelayAction(particlesData.AnimationStartTime * animationDataToPlay.Animation.AnimationTime, () =>
                    {
                        particlesData.ParticleSystem.Play();
                    }));
                }
            }

            if(playSound)
            {
                foreach (SoundsData soundsData in animationDataToPlay.SoundsDataList)
                {
                    StartCoroutine(DelayAction(soundsData.AnimationStartTime * animationDataToPlay.Animation.AnimationTime, () =>
                    {
                        soundsData.AudioData?.Play();
                    }));
                }
            }
        }
        else
        {
            Debug.LogWarning($"Couldn't find animation on object: {gameObject.name}, with id: {animationID}");
        }
    }

    private IEnumerator DelayAction(float delay, System.Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        onComplete?.Invoke();
    }
}

[System.Serializable]
public class ObjectAnimatorData
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public ObjectAnimatorBase Animation { get; private set; }
    [field: SerializeField] public List<ParticlesData> ParticlesDataList { get; private set; }
    [field: SerializeField] public List<SoundsData> SoundsDataList { get; private set; }
}

[System.Serializable]
public class ParticlesData
{
    [Range(0, 1f)]
    [SerializeField] public float AnimationStartTime;

    [field: SerializeField] public ParticleSystem ParticleSystem { get; private set; }
}

[System.Serializable]
public class SoundsData
{
    [Range(0, 1f)]
    [SerializeField] public float AnimationStartTime;

    [field: SerializeField] public AudioTrackBaseSO AudioData { get; private set; }
}
