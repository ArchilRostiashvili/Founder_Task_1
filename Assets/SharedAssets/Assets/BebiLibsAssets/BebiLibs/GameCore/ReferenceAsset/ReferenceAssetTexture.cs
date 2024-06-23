using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    [System.Serializable]
    public class ReferenceAssetTexture : Referenceable
    {
#if UNITY_EDITOR
        public Texture2D assetReference;

        public ReferenceAssetTexture()
        {
            this.elementType = typeof(Texture2D).ToString();
        }

        public ReferenceAssetTexture(Texture2D texture)
        {
            this.elementType = typeof(Texture2D).ToString();

#if UNITY_EDITOR
            if (texture != null)
            {
                this.assetResourcePath = GetElementPath(texture);
                this.assetReference = texture;
            }
#endif
        }
#endif
    }
}
