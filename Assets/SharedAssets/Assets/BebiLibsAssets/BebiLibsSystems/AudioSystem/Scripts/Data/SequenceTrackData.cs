using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.AudioSystem
{
    [System.Serializable]
    public class SequenceTrackData
    {
        [field: SerializeField] public AudioTrackSO AudioTrack { get; private set; }
        [field: SerializeField] public float AdditionalDelay { get; private set; } = 0f;
        [field: SerializeField] public bool IsPlaceHolder { get; private set; } = false;

        public SequenceTrackData(AudioTrackSO audioTrack)
        {
            AudioTrack = audioTrack;
        }

        public void SetAudioTrack(AudioTrackSO audioTrack)
        {
            if (IsPlaceHolder)
            {
                AudioTrack = audioTrack;
            }
            else
            {
                Debug.LogError("Cannot Modify AudiTrack, This SequenceTrackData is not a placeholder!");
            }
        }
    }


    // [CustomPropertyDrawer(typeof(SequenceTrackData))]
    // public class SequenceTrackDataDrawer : PropertyDrawer
    // {
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         EditorGUI.BeginProperty(position, label, property);

    //         property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);



    //         SerializedProperty audioTrackProperty = property.FindPropertyRelative("<AudioTrack>k__BackingField");
    //         SerializedProperty isPlaceHolderProperty = property.FindPropertyRelative("<IsPlaceHolder>k__BackingField");
    //         SerializedProperty additionalDelayProperty = property.FindPropertyRelative("<AdditionalDelay>k__BackingField");

    //         // Rect audioTrackRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
    //         // Rect isPlaceHolderRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
    //         // Rect additionalDelayRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);

    //         // EditorGUI.PropertyField(audioTrackRect, audioTrackProperty, new GUIContent("Audio Track"));
    //         // EditorGUI.PropertyField(isPlaceHolderRect, isPlaceHolderProperty, new GUIContent("Is Place Holder"));
    //         // EditorGUI.PropertyField(additionalDelayRect, additionalDelayProperty, new GUIContent("Additional Delay"));

    //         EditorGUI.EndProperty();
    //     }
    // }
}
