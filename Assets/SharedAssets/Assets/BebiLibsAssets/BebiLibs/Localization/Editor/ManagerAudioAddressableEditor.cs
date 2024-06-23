#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.Localization
{
#if UNITY_EDITOR
    [CustomEditor(typeof(AddressableAudioManager))]
    public class ManagerAudioAddressableEditor : Editor
    {
        private AddressableAudioManager _managerAudioAddressable;

        private void OnEnable()
        {
            _managerAudioAddressable = (AddressableAudioManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }
    }
#endif
}
