using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static BebiLibs.DelayOperation.AutoDelayAction;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.DelayOperation
{

    public class AutoDelayAction : MonoBehaviour
    {
        [System.Serializable]
        public abstract class BaseDelay
        {
            public abstract IEnumerator DelayOperation();
        }

        [System.Serializable]
        public class FrameTimeDelay : BaseDelay
        {
            //[Range(0, 1000)]
            [SerializeReference] public int frameTime;
            public override IEnumerator DelayOperation()
            {
                for (int i = 0; i < this.frameTime; i++)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        [System.Serializable]
        public class TimeSecondDelay : BaseDelay
        {
            //[Range(0, 1000)]
            [SerializeReference] public float frameTime;
            public override IEnumerator DelayOperation()
            {
                yield return new WaitForSeconds(this.frameTime);
            }
        }

        [System.Serializable]
        public class NoDelay : BaseDelay
        {
            public override IEnumerator DelayOperation()
            {
                yield return null;
            }
        }

        public enum DelayActivator
        {
            None, Start, OnEnable
        }

        [SerializeField] private DelayActivator delayActivator = DelayActivator.None;
        [SerializeReference] private BaseDelay baseDelay = new NoDelay();
        [Header("Action After Delay Finish")]
        public UnityEvent OnDelayEnd;

        private void Start()
        {
            if (this.delayActivator == DelayActivator.Start)
            {
                this.ActivateDelay();
            }
        }

        private void OnEnable()
        {
            if (this.delayActivator == DelayActivator.OnEnable)
            {
                this.ActivateDelay();
            }
        }

        public void ActivateDelay()
        {
            if (this.gameObject.activeSelf && this.gameObject.activeInHierarchy && baseDelay != null)
            {
                this.StartCoroutine(DelayFunction());
            }

            IEnumerator DelayFunction()
            {
                yield return this.baseDelay.DelayOperation();
                this.OnDelayEnd?.Invoke();
            }
        }

        internal void SetBaseType(BaseDelay baseDelay)
        {
            this.baseDelay = baseDelay;
        }

        private void OnDisable()
        {
            this.StopAllCoroutines();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AutoDelayAction))]
    public class AutoDelayActionEditor : Editor
    {
        private AutoDelayAction _autoDelayAction;

        private void OnEnable()
        {
            _autoDelayAction = (AutoDelayAction)target;
        }

        string[] opstions = new string[3] { "NoDelay", "FrameCount", "TimeSeconds" };
        int selectedIndex = 0;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            this.selectedIndex = EditorGUILayout.Popup("Select Delay Type", this.selectedIndex, this.opstions);

            if (GUILayout.Button("Update"))
            {
                this.SetData(this.selectedIndex);
            }
        }

        public void SetData(int index)
        {
            BaseDelay baseDelay = index switch
            {
                0 => new NoDelay(),
                1 => new FrameTimeDelay(),
                2 => new TimeSecondDelay(),
                _ => new NoDelay(),
            };
            _autoDelayAction.SetBaseType(baseDelay);
            EditorUtility.SetDirty(_autoDelayAction);
        }

    }
#endif
}
