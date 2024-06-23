using BebiAnimations.Libs.Core;
using System.Collections.Generic;
using UnityEngine;

namespace BebiAnimations.Libs.Actions
{
    [System.Serializable]
    public class Action_ParticlePlayer : AnimationAction
    {
        [SerializeField] private List<ParticleSystem> _particleSystemList;
        [SerializeField] private bool _playParticles;

        protected override void ActionPlay()
        {
            if (_playParticles)
            {
                foreach (var item in _particleSystemList)
                {
                    item.gameObject.SetActive(true);
                    item.Play();
                }
            }
            else
            {
                foreach (var item in _particleSystemList)
                {
                    item.Stop();
                }
            }
        }

        protected override void ActionStop()
        {
            /*
            foreach (var item in _particleSystemList)
            {
                item.Stop();
            }
            */
        }
    }
}
