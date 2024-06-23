using System;
using UnityEngine;

public static class TMPFormatterUtility
{
    public static string WrapInColorString(string inputText, Color color)
    {
        return $"{GetColorStartMarkup(color)}{inputText}{GetColorEndMarkup()}";
    }

    public static string GetColorString(Color color)
    {
        return "#" + ColorUtility.ToHtmlStringRGBA(color);
    }

    public static string GetColorStartMarkup(Color color)
    {
        return $"<color={GetColorString(color)}>";
    }

    public static string GetColorEndMarkup()
    {
        return "</color>";
    }

    public static string GetLinkMarkup(string linkUD)
    {
        return $"<link=\"{linkUD}\">";
    }

    public static string GetLinkEndMarkup()
    {
        return "</link>";
    }
}