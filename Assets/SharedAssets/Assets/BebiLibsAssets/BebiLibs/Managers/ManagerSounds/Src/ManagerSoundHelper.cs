using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace BebiLibs
{
    public class ManagerSoundHelper : MonoBehaviour
    {
        public AudioClipPack audioClips;
        public ManagerSounds managerSounds;

        public void CopyToContainer()
        {
            this.audioClips.Clear();
            for (int i = 0; i < this.managerSounds.dataSoundsLib.arraySimpleClips.Count; i++)
            {
                this.audioClips.Add(this.managerSounds.dataSoundsLib.arraySimpleClips[i]);
            }
            EditorUtility.SetDirty(this.audioClips);
        }

    }

    [CustomEditor(typeof(ManagerSoundHelper))]
    public class ManagerSoundHelperEditor : Editor
    {
        private ManagerSoundHelper _managerSoundHalper;

        private void OnEnable()
        {
            _managerSoundHalper = (ManagerSoundHelper)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("CopyToContainer"))
            {
                _managerSoundHalper.CopyToContainer();
            }
        }
    }

}
#endif