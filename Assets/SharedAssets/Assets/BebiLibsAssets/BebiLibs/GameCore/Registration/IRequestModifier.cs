using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace BebiLibs.ServerConfigLoaderSystem.Core
{
    public interface IRequestModifier
    {
        abstract void ModifyRequestHeader(UnityWebRequest unityWebRequest);
    }
}
