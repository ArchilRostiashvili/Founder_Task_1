using System;
using System.Linq;
using UnityEngine;

public static class StringUtils
{
    public static string FirstCharToUpper(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
        }
        else
        {
            return input[0].ToString().ToUpper() + input[1..].ToLower();
        }
    }

    public static string ReplaceMultipleWithOne(this string value, string replace, string replaceWith)
    {
        if (string.IsNullOrEmpty(value)) return value;

        var split = value.Split(replace).Where(x => x != "");
        if (split.Count() <= 1) return value;

        string second = string.Join(replaceWith, split);
        var rep = second.Split(replaceWith).Where(x => x != "");
        string join = string.Join(replaceWith, rep);
        return join;
    }

    public static string GetFirstNCharacters(this string value, int n)
    {
        if (string.IsNullOrEmpty(value)) return value;
        if (value.Length <= n) return value;
        return value.Substring(0, n);
    }

    // Substring if given string is greater than given length, otherwise fill with given character, default is space
    public static string SubstringOrFill(this string value, int length, char fill = ' ')
    {
        if (string.IsNullOrEmpty(value)) return value;
        if (value.Length <= length) return value.PadRight(length, fill);
        return value.Substring(0, length);
    }

    public static string Colorize(this string text, string color, bool bold = false)
    {
        return
        "<color=" + color + ">" +
        (bold ? "<b>" : "") +
        text +
        (bold ? "</b>" : "") +
        "</color>";
    }
}