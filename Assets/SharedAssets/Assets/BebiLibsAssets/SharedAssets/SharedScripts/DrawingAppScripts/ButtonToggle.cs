using UnityEngine;
using UnityEngine.Events;
namespace DrawingApp
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class ButtonToggle : MonoBehaviour
    {
        public UnityEvent onEvent = new UnityEvent();
        public UnityEvent offEvent = new UnityEvent();

        public UnityEvent<bool> autoEvent = new UnityEvent<bool>();

        [SerializeField] private bool _isOn;
        [SerializeField] private bool _defaultValue = false;
        [SerializeField] private bool _autoInvoke = true;


        private void Awake()
        {
            _defaultValue = _isOn;
        }

        private void Start()
        {
            InvokeEvent(_autoInvoke);
        }

        private void InvokeEvent(bool callAutoInvoke)
        {
            if (_isOn)
            {
                onEvent.Invoke();
            }
            else
            {
                offEvent.Invoke();
            }

            if (callAutoInvoke)
            {
                autoEvent?.Invoke(_isOn);
            }
        }

        public void SetActiveState(bool isOn)
        {
            _isOn = isOn;
            InvokeEvent(_autoInvoke);
        }

        public void SetToDefault()
        {
            _isOn = _defaultValue;
            InvokeEvent(_autoInvoke);
        }

        public void Toggle()
        {
            _isOn = !_isOn;
            InvokeEvent(true);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ButtonToggle))]
    public class ButtonToggleEditor : Editor
    {
        private ButtonToggle _buttonToggle;

        private void OnEnable()
        {
            _buttonToggle = (ButtonToggle)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Toggle"))
            {
                _buttonToggle.Toggle();
            }
        }
    }
#endif
}