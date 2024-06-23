
using UnityEngine;

[System.Serializable]
public class ShapePropertiesData
{
    public string CardID;
    public ShapeProperties ShapeData;
    public ShapeProperties ShapeOutlineData;
}

[System.Serializable]
public class ShapeProperties
{
    public Color ShapeColor;
    public Sprite ShapeSprite;
    public Vector2 ShapeSize = new Vector2(1f, 1f);
    public Vector2 ShapeOffset = new Vector2(0f, 0f);

}


[System.Serializable]
public class ColorData
{
    public Color ShapeColor;
    public Color StrokeColor;
    public Color ShapeStrokeColor;
}


[System.Serializable]
public class SpriteColorCombo
{
    public Sprite sprite;
    public Color color;
}


[System.Serializable]
public class LetterToWord
{
    public string Letter;
    public string Word;
    public Sprite Sprite;
    public AudioClip Audio;
}
