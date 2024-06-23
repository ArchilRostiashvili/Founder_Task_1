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
    public class ReferenceAssetSprite : Referenceable
    {
#if UNITY_EDITOR
        public Sprite assetReference;

        public ReferenceAssetSprite()
        {
            this.elementType = typeof(Sprite).ToString();
        }

        public ReferenceAssetSprite(Sprite sprite) : base()
        {
            this.elementType = typeof(Sprite).ToString();
#if UNITY_EDITOR
            if (sprite != null)
            {
                this.assetResourcePath = GetElementPath(sprite);
                this.assetReference = sprite;
            }
#endif
        }
#endif
    }
}
