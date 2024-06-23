using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BebiLibs
{
    public class RecalculateLayout : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        private void OnEnable()
        {
            LayoutRebuilder.MarkLayoutForRebuild(_rectTransform);
        }

    }
}
