using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ServerConfigLoaderSystem.Core
{
    public abstract class AbstractInitData : BaseRequestData
    {
        public abstract bool GetDeviceID(out string deviceID);
    }
}
