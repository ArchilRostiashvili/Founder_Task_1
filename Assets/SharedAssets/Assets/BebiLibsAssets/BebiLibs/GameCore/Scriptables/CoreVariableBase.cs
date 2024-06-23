using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    public abstract class CoreVariableBase<T> : CoreVariableBase
    {
        [UnityEngine.Serialization.FormerlySerializedAs("value")]
        [UnityEngine.Serialization.FormerlySerializedAs("_runtimeValue")]
        [SerializeField] private T _defaultValue;

        [System.NonSerialized] private T _runtimeValue;

#if UNITY_EDITOR
        [SerializeField] private T _runtimeValueDisplay;
#endif

        protected virtual void OnValidate()
        {
            UpdateRuntimeValue();
        }

        protected virtual void OnEnable()
        {
            UpdateRuntimeValue();
        }

        protected virtual void UpdateRuntimeValue()
        {
            _runtimeValue = _defaultValue;
            SetDisplayRuntimeValue();
        }

        public virtual T Value
        {
            get => _runtimeValue;

            set
            {
                _runtimeValue = value;
                SetDisplayRuntimeValue();
            }
        }

        public T GetDefaultValue() => _defaultValue;
        public virtual void SetDefaultValue(T value, bool updateRuntimeValue = true)
        {
            _runtimeValue = updateRuntimeValue ? value : _runtimeValue;
            SetDisplayRuntimeValue();
            _defaultValue = value;
        }

        private void SetDisplayRuntimeValue()
        {
#if UNITY_EDITOR
            _runtimeValueDisplay = _runtimeValue;
#endif
        }

        public static implicit operator T(CoreVariableBase<T> d) => d.Value;
    }

    public abstract class CoreVariableBase : ScriptableObject
    {

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(CoreVariableBase), true)]
    public class CoreVariableBaseEditor : Editor
    {
        private CoreVariableBase _coreVariableBase;
        private SerializedProperty runtimeValue;

        private void OnEnable()
        {
            _coreVariableBase = (CoreVariableBase)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }
    }
#endif
}
