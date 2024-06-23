using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;
namespace Pets
{
    public class AnimationEventHelper : MonoBehaviour
    {
        private string[] _arrayPiu = { "fx_piu1", "fx_piu2" };

        public void MouseEnterFrame()
        {
            ManagerSounds.PlayEffect("fx_mouse1");
        }

        public void mouseExitFrame()
        {
            ManagerSounds.PlayEffect("fx_mouse_2");
        }

        public void BallTouch()
        {
            ManagerSounds.PlayEffect(_arrayPiu[Random.Range(0, _arrayPiu.Length)], 0, 0, false, null, 0.5f);
        }
    }
}

