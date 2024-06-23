using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BebiLibs
{
    public class GraphicColorDimmer : MonoBehaviour
    {
        [SerializeField] private List<GraphicElement> _graphicsToDimList = new List<GraphicElement>();

        public void DimeColor()
        {
            _graphicsToDimList.ForEach(x => x.Graphic.color = x.DimmedColor);
        }

        public void RestoreColor()
        {
            _graphicsToDimList.ForEach(x => x.Graphic.color = x.OriginalColor);
        }

        public void DimeToColor(Color color)
        {
            _graphicsToDimList.ForEach(x => x.Graphic.color = color);
        }


        [System.Serializable]
        public class GraphicElement
        {
            public Graphic Graphic;
            public Color OriginalColor = Color.white;
            public Color DimmedColor = Color.white;
        }
    }

}
