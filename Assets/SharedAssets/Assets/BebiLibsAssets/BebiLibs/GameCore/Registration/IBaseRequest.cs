using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ServerConfigLoaderSystem.Core
{
    public interface IBaseRequest : IRequestModifier
    {
        public ResponseData RequestResponseData { get; set; }
    }
}
