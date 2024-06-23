using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public abstract class ManagerAudioAddressableBase : MonoBehaviour
    {
        public abstract void UpdateLocalizationLibrary(DataSoundsLib dataSoundsLib, bool isTemp);
        public abstract bool GetLocalizedAudio(string soundName, out DataSound dataSound);
    }
}
