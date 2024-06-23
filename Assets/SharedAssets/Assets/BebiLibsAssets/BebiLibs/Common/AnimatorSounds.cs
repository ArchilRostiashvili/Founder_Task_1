using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class AnimatorSounds : MonoBehaviour
    {
        public void Trigger_Animation_PlayEffect(string soundName)
        {
            ManagerSounds.PlayEffect(soundName);
        }
    }
}
