using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ExtendedFreeTrialSystem
{
    [System.Serializable]
    public class TextEntry
    {
        public string textKey;
        [TextArea(1, 4)]
        public string infortext;
    }
}
