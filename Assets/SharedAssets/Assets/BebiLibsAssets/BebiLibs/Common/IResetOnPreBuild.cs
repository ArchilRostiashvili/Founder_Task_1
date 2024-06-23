using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    /// <summary>
    /// Interface for scriptable objects that need to be cleared before build
    /// </summary>
    public interface IResetOnPreBuild
    {
        /// <summary>
        /// reset all called before build
        /// </summary>
        void ResetOnPreBuild();
    }
}
