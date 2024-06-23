using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class SpriteEffectHue : MonoBehaviour
    {
        public enum ColorState { NORMAL, HUE };

        public Material colorHue;
        public Material colorDefault;

        public List<SpriteRenderer> arraySR;

        public void SetColorState(ColorState colorState)
        {
            if (colorState == ColorState.HUE)
            {
                this.ChangeHue(this.colorHue);
            }
            else
            if (colorState == ColorState.NORMAL)
            {
                this.ChangeHue(this.colorDefault);
            }
        }

        public void ChangeHue(Material matrial)
        {
            for (int i = 0; i < this.arraySR.Count; i++)
            {
                if (this.arraySR[i] != null)
                {
                    this.arraySR[i].material = matrial;
                }
            }
        }
        /*
        public void ChangeHue(Transform rootTransform, Material matrial, bool includeRoot = true)
        {
            SpriteRenderer renderer = rootTransform.GetComponent<SpriteRenderer>();

            if(renderer != null && includeRoot)
            {
                renderer.material = matrial;
            }

            for (int i = 0; i < rootTransform.childCount; i++)
            {
                this.ChangeHue(rootTransform.GetChild(i), matrial, true);
            }
        }
        */
    }
}
