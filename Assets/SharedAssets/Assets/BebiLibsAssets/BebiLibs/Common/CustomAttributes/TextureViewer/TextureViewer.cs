using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class TextureViewer : PropertyAttribute
    {
        public int Height;
        public int Spacing;
        public bool IsSprite;

        public TextureViewer(bool isSprite, int height = 50, int spacing = 3)
        {
            Height = height;
            Spacing = spacing;
            IsSprite = isSprite;
        }
    }
}
