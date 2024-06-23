using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public abstract class StringFormatBase : ScriptableObject
    {
        public abstract string key { get; }
        public abstract string value { get; }
    }
}
