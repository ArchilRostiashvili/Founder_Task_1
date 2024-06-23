
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace BebiLibs
{
    [CustomPropertyDrawer(typeof(TextureViewer))]
    public class TextureDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded && property.objectReferenceValue != null && attribute is TextureViewer labelAttribute)
            {
                return base.GetPropertyHeight(property, label) + labelAttribute.Height + labelAttribute.Spacing;
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue != null && !property.isExpanded)
            {
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
            }

            if (property.isExpanded && property.objectReferenceValue != null && attribute is TextureViewer labelAttribute)
            {
                float textureHeight = labelAttribute.Height;
                float spacing = labelAttribute.Spacing;

                float ratio;
                float textureWidth;
                Texture2D texture = null;
                Sprite sprite = null;
                if (labelAttribute.IsSprite)
                {
                    sprite = (Sprite)property.objectReferenceValue;
                    Rect spriteRect = sprite.textureRect;
                    ratio = (float)spriteRect.height / (float)spriteRect.width;
                }
                else
                {
                    texture = ((Sprite)property.objectReferenceValue).texture;
                    ratio = (float)texture.height / (float)texture.width;
                }

                textureWidth = textureHeight / ratio;

                Rect defaultPosition = new Rect(position.x, position.y, position.width, position.height - textureHeight - spacing);
                property.isExpanded = EditorGUI.Foldout(defaultPosition, property.isExpanded, GUIContent.none);
                EditorGUI.PropertyField(defaultPosition, property, label);
                Rect textureRect = new Rect(position.x + position.width - textureWidth, position.y + position.height - textureHeight + labelAttribute.Spacing, textureWidth, textureHeight);

                if (labelAttribute.IsSprite)
                {
                    DrawTexturePreview(textureRect, sprite);
                }
                else
                {
                    EditorGUI.DrawPreviewTexture(textureRect, texture);
                }

            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }


        private void DrawTexturePreview(Rect position, Sprite sprite)
        {
            Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
            Vector2 size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

            Rect coords = sprite.textureRect;
            coords.x /= fullSize.x;
            coords.width /= fullSize.x;
            coords.y /= fullSize.y;
            coords.height /= fullSize.y;

            // Vector2 ratio;
            // ratio.x = position.width / size.x;
            // ratio.y = position.height / size.y;
            // float minRatio = Mathf.Min(ratio.x, ratio.y);

            //Vector2 center = position.center;
            //position.width = size.x * minRatio;
            //position.height = size.y * minRatio;
            //position.center = center;

            GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
        }
    }
}
#endif