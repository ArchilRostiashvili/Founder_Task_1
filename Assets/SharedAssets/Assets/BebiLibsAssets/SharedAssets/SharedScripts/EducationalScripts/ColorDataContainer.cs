using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorDataContainer", menuName = "Modules/Educational/ColorDataContainer", order = 0)]
public class ColorDataContainer : ScriptableObject
{
    [SerializeField] public List<ColorsSharedData> ColorsDatasList;
}

[System.Serializable]
public class ColorsSharedData
{
    [SerializeField] public Color MainColor;
    [SerializeField] public Color OutlineColor;

    public static ColorsSharedData Create(Color mainColor, Color outlineColor)
    {
        ColorsSharedData data = new ColorsSharedData
        {
            MainColor = mainColor,
            OutlineColor = outlineColor
        };

        return data;
    }
}