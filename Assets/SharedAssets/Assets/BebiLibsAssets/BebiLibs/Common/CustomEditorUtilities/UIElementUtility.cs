using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomEditorUtilities
{


    public static class UIElementUtility
    {
        public readonly static Color32 LineGray = new Color32(120, 120, 120, 255);
        public readonly static Color32 LineColor = new Color(0.3490196f, 0.3490196f, 0.3490196f);
        private static float _FontSize = 10;



        public static VisualElement CreateVisualElement(this UnityEngine.Object target, float margin = 5, System.Action onChange = null)
        {
            Editor editor = Editor.CreateEditor(target);
            if (editor != null)
            {
                IMGUIContainer inspectorIMGUI = new IMGUIContainer(() =>
                {
                    EditorGUI.BeginChangeCheck();
                    editor.OnInspectorGUI();

                    if (EditorGUI.EndChangeCheck())
                    {
                        onChange?.Invoke();
                    }
                });
                inspectorIMGUI.SetMargin(margin);
                return inspectorIMGUI;
            }
            else
            {
                Debug.LogWarning("Editor.CreateEditor(target) == null");
                return new VisualElement();
            }
        }

        public static T AddHeaderLabel<T>(this T element, string labelText, float fontSize = 5, float margin = 5) where T : VisualElement
        {
            Label label = new Label(labelText);
            label.SetTextStyles(_FontSize + fontSize, FontStyle.Bold);
            label.SetTextAlign(TextAnchor.MiddleCenter);
            label.SetMargin(margin);
            var line = SimpleLineHorizontal(new Color(0.3490196f, 0.3490196f, 0.3490196f), 1);
            element.Add(label);
            element.Add(line);
            return element;
        }

        public static T AddLine<T>(this T element, out VisualElement line, LineDirection lineDirection = LineDirection.Horizontal, float width = 1) where T : VisualElement
        {
            if (lineDirection == LineDirection.Horizontal)
                line = SimpleLineHorizontal(LineColor, width);

            else
                line = SimpleLineVertical(LineColor, width);
            element.Add(line);
            return element;
        }

        public static T SetMargin<T>(this T element, float margin) where T : VisualElement
        {
            element.style.marginLeft = margin;
            element.style.marginRight = margin;
            element.style.marginTop = margin;
            element.style.marginBottom = margin;
            return element;
        }

        public static T SetMargin<T>(this T element, float marginLeft, float marginRight, float marginTop, float marginBottom) where T : VisualElement
        {
            element.style.marginLeft = marginLeft;
            element.style.marginRight = marginRight;
            element.style.marginTop = marginTop;
            element.style.marginBottom = marginBottom;
            return element;
        }

        public static T SetPadding<T>(this T element, float padding) where T : VisualElement
        {
            element.style.paddingLeft = padding;
            element.style.paddingRight = padding;
            element.style.paddingTop = padding;
            element.style.paddingBottom = padding;
            return element;
        }

        public static T SetPadding<T>(this T element, float paddingLeft, float paddingRight, float paddingTop, float paddingBottom) where T : VisualElement
        {
            element.style.paddingLeft = paddingLeft;
            element.style.paddingRight = paddingRight;
            element.style.paddingTop = paddingTop;
            element.style.paddingBottom = paddingBottom;
            return element;
        }

        public static T SetTextStyles<T>(this T element, float fontSize, FontStyle fontStyle = FontStyle.Normal) where T : VisualElement
        {
            element.style.fontSize = fontSize;
            element.style.unityFontStyleAndWeight = fontStyle;
            return element;
        }

        public static T SetFontStyles<T>(this T element, FontStyle fontStyle = FontStyle.Normal) where T : VisualElement
        {
            element.style.unityFontStyleAndWeight = fontStyle;
            return element;
        }

        public static T SetTextAlign<T>(this T element, TextAnchor anchor) where T : VisualElement
        {
            element.style.unityTextAlign = anchor;
            return element;
        }

        public static T SetTextColor<T>(this T element, Color color) where T : VisualElement
        {
            element.style.color = color;
            return element;
        }

        public static T SetBorderSize<T>(this T element, float borderWidth) where T : VisualElement
        {
            element.style.borderLeftWidth = borderWidth;
            element.style.borderRightWidth = borderWidth;
            element.style.borderTopWidth = borderWidth;
            element.style.borderBottomWidth = borderWidth;
            return element;
        }

        public static T SetBorderSize<T>(this T element, float borderLeftWidth, float borderRightWidth, float borderTopWidth, float borderBottomWidth) where T : VisualElement
        {
            element.style.borderLeftWidth = borderLeftWidth;
            element.style.borderRightWidth = borderRightWidth;
            element.style.borderTopWidth = borderTopWidth;
            element.style.borderBottomWidth = borderBottomWidth;
            return element;
        }

        public static T SetBorderColor<T>(this T element, Color borderColor) where T : VisualElement
        {
            element.style.borderLeftColor = borderColor;
            element.style.borderRightColor = borderColor;
            element.style.borderTopColor = borderColor;
            element.style.borderBottomColor = borderColor;
            return element;
        }

        public static T SetBackgroundColor<T>(this T element, Color backgroundColor) where T : VisualElement
        {
            element.style.backgroundColor = backgroundColor;
            return element;
        }

        public static T SetBorderColor<T>(this T element, float borderRadiuses) where T : VisualElement
        {
            element.style.borderTopLeftRadius = borderRadiuses;
            element.style.borderTopRightRadius = borderRadiuses;
            element.style.borderBottomLeftRadius = borderRadiuses;
            element.style.borderBottomRightRadius = borderRadiuses;
            return element;
        }

        public static T SetFlexBasis<T>(this T element, float flexBasis) where T : VisualElement
        {
            element.style.flexBasis = flexBasis;
            return element;
        }

        public static T SetFlexDirection<T>(this T element, FlexDirection flexDirection) where T : VisualElement
        {
            element.style.flexDirection = flexDirection;
            return element;
        }

        public static T SetFlexGrow<T>(this T element, float flexGrow) where T : VisualElement
        {
            element.style.flexGrow = flexGrow;
            return element;
        }

        public static T SetFlexShrink<T>(this T element, float flexShrink) where T : VisualElement
        {
            element.style.flexShrink = flexShrink;
            return element;
        }

        public static T SetFlexWrap<T>(this T element, Wrap flexWrap) where T : VisualElement
        {
            element.style.flexWrap = flexWrap;
            return element;
        }

        public static T SetJustifyContent<T>(this T element, Justify justifyContent) where T : VisualElement
        {
            element.style.justifyContent = justifyContent;
            return element;
        }

        public static T SetAlignItems<T>(this T element, Align alignItems) where T : VisualElement
        {
            element.style.alignItems = alignItems;
            return element;
        }

        public static VisualElement SimpleLineHorizontal(Color color, float width = 1f)
        {
            VisualElement line = new VisualElement();
            line.style.minHeight = width;
            line.SetBackgroundColor(color);
            return line;
        }

        public static VisualElement SimpleLineVertical(Color color, float width = 1f)
        {
            VisualElement line = new VisualElement();
            line.name = "SimpleLineHorizontal";
            line.style.minWidth = width;
            line.SetBackgroundColor(color);
            return line;
        }

        public static PropertyField CreateProperty(Object serializedObject, string propertyName, ref SerializedObject cashedObject)
        {
            cashedObject ??= new SerializedObject(serializedObject);
            SerializedProperty sProp = cashedObject.FindProperty(propertyName);
            sProp.isExpanded = false;
            PropertyField propertyField = new PropertyField(sProp);
            propertyField.Bind(cashedObject);
            return propertyField;
        }

        public static VisualElement SimpleLine(Color32 color, float width = 1f)
        {
            return SimpleLineHorizontal(color.ToColor(), width);
        }

        public static VisualElement SimpleLine(float width = 1f)
        {
            return SimpleLine(LineGray, width);
        }

        public static Color ToColor(this Color32 color32)
        {
            float d = 255.0f;
            return new Color(color32.r / d, color32.g / d, color32.b / d, color32.a / d);
        }


        public static VisualElement CreateReadOnlyTextField(string label, string text, float fontSize = 12, float margin = 5, float padding = 5)
        {
            TextField textField = new TextField(1000, true, false, '*').SetMargin(margin);

            if (!string.IsNullOrEmpty(label))
            {
                textField.label = label;
                textField.labelElement.style.alignSelf = Align.Center;
                textField.labelElement.style.minWidth = 100;
            }

            textField.isReadOnly = true;
            textField.value = text;
            textField.style.position = Position.Relative;
            textField.style.whiteSpace = WhiteSpace.Normal;

            VisualElement textInput = textField.Query<VisualElement>("unity-text-input");
            textInput.SetBorderSize(0);
            textInput.SetBorderColor(new Color(1, 1, 1, 0));
            textInput.SetBackgroundColor(new Color(0.1415094f, 0.1415094f, 0.1415094f, 1f));
            textInput.SetPadding(padding);
            textInput.SetTextStyles(fontSize);
            textInput.style.whiteSpace = WhiteSpace.Normal;
            return textField;
        }

        public static T SetID<T>(this T visualElement, string ID) where T : VisualElement
        {
            visualElement.name = ID;
            return visualElement;
        }

        public static Button AddButton<T>(this T visualElement, string buttonText, System.Action action) where T : VisualElement
        {
            Button button = new Button(action);
            button.text = buttonText;
            visualElement.Add(button);
            return button;
        }

        public static T AddClipboardListener<T>(this T visualElement, string valueToCopy) where T : VisualElement
        {
            visualElement.RegisterCallback((ClickEvent clickEvent) =>
           {
               EditorGUIUtility.systemCopyBuffer = valueToCopy;
               //Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Copied to clipboard");
           });
            return visualElement;
        }
    }

    public enum LineDirection
    {
        Horizontal,
        Vertical
    }
}

#endif
