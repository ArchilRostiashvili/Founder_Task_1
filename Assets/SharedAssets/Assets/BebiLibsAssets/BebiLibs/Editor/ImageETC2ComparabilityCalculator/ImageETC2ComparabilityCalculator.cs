using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

namespace ImageETC2ComparabilityCalculatorEditor
{
    public class ImageETC2ComparabilityCalculator : EditorWindow
    {

        [MenuItem("BebiLibs/Helper/ImageETC2ComparabilityCalculator")]
        private static void ShowWindow()
        {
            var window = GetWindow<ImageETC2ComparabilityCalculator>();
            window.titleContent = new GUIContent("ImageETC2ComparabilityCalculator");
            window.Show();
        }

        private void OnGUI()
        {
            DrawCalculateButton();
        }

        private void DrawCalculateButton()
        {
            if (GUILayout.Button("Calculate"))
            {
                var textures = Selection.assetGUIDs
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<Texture2D>)
                    .OrderBy(x => x.name)
                    .ToList();

                foreach (var item in textures)
                {
                    Debug.Log(item.name + " " + CalculateDimentions(item));
                }
            }
        }

        public static bool GetImageSize(Texture2D asset, out int width, out int height)
        {
            if (asset != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                if (importer != null)
                {
                    object[] args = new object[2] { 0, 0 };
                    MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                    mi.Invoke(importer, args);

                    width = (int)args[0];
                    height = (int)args[1];

                    return true;
                }
            }

            height = width = 0;
            return false;
        }

        //calculate texture size witch will be divisible by 4 
        private Vector2Int CalculateDimentions(Texture2D texture2D)
        {
            if (!GetImageSize(texture2D, out var originalWidth, out var originalHeight))
            {
                Debug.Log("Error");
                return Vector2Int.zero;
            }

            var width = texture2D.width;
            var height = texture2D.height;

            float ratio = width > height ? (float)width / originalWidth : (float)height / originalHeight;

            // Debug.Log(ratio);
            // Debug.Log(width + " " + height);
            // Debug.Log(originalWidth + " " + originalHeight);
            // Debug.Log(Math.Round(originalWidth * ratio) + " " + Math.Round(originalHeight * ratio));

            var widthMod = width % 4;
            var heightMod = height % 4;

            width = GetSizeDevisableByFour(width, widthMod);
            height = GetSizeDevisableByFour(height, heightMod);

            return new Vector2Int((int)Mathf.Round(width / ratio), (int)Math.Round(height / ratio));
        }

        private static int GetSizeDevisableByFour(int size, int mod)
        {
            if (mod != 0)
            {
                if (4 - mod > 2)
                {
                    size += 4 - mod;
                }
                else
                {
                    size -= mod;
                }
            }
            return size;
        }
    }
}
