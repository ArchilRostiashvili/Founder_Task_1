using System.Collections.Generic;
using UnityEngine;
using static BebiAnimations.Libs.Core.BebiAnimation;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiAnimations.Libs.Core
{
    public class BebiAnimator : MonoBehaviour
    {
        [SerializeField] private List<BebiAnimation> _actionCliptList = new List<BebiAnimation>();
        [SerializeField] private Initialization _autoInitialization = Initialization.ON_AWAKE;
        internal List<BebiAnimation> ActionClipList => _actionCliptList;

        private void Awake()
        {
            if (_autoInitialization == Initialization.ON_AWAKE)
            {
                Initialize();
            }
        }

        private void Start()
        {
            if (_autoInitialization == Initialization.ON_START)
            {
                Initialize();
            }
        }

        private void OnEnable()
        {
            if (_autoInitialization == Initialization.ON_ENABLE)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            for (int i = 0; i < _actionCliptList.Count; i++)
            {
                _actionCliptList[i].Initialize();
            }
            BebiAnimatorManager.AddAnimator(this);
        }

        public void PlayNoReturn(string actionID) => Play(actionID, true);

        public bool Play(string actionID, bool playAnim = true, System.Action animCompleteEvent = null, System.Action<AnimationAction> actionStartEvent = null, System.Action<AnimationAction> actionEndEvent = null)
        {
            if (TryGetActionClip(actionID, out BebiAnimation actionClip))
            {
                actionClip.Play(playAnim, animCompleteEvent, actionStartEvent, actionEndEvent);
                return true;
            }
            else
            {
                animCompleteEvent?.Invoke();
                LogActionNotFountWarning("Unable to play", actionID);
                return false;
            }
        }

        public bool Revert(string actionID)
        {
            if (TryGetActionClip(actionID, out BebiAnimation actionClip))
            {
                actionClip.Revert();
                return true;
            }
            else
            {
                LogActionNotFountWarning("Unable to play", actionID);
                return false;
            }
        }

        public float GetAnimationDuration(string actionID)
        {
            if (TryGetActionClip(actionID, out BebiAnimation actionClip))
            {
                return actionClip.TotalAnimationTime;
            }
            else
            {
                return 0f;
            }
        }

        public void Stop(string actionID)
        {
            if (TryGetActionClip(actionID, out BebiAnimation actionClip))
            {
                actionClip.Stop();
            }
            else
            {
                LogActionNotFountWarning("Unable to stop", actionID);
            }
        }

        private void LogActionNotFountWarning(string error, string actionID)
        {
            Debug.LogWarning($"{error} {nameof(BebiAnimation)} with {nameof(actionID)}: {actionID} not fount in {nameof(_actionCliptList).Replace("_", "")}");
        }

        public bool TryGetActionClip(string actionID, out BebiAnimation clip)
        {
            clip = _actionCliptList.Find(x => x.ActionID == actionID);
            return clip != null;
        }

        public void EnterFrame()
        {
            for (int i = 0; i < _actionCliptList.Count; i++)
            {
                _actionCliptList[i].EnterFrame();
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BebiAnimator))]
    public class BebiAnimatorEditor : Editor
    {
        private BebiAnimator _actionAnimator;
        private string[] _options = new string[0];
        private int _selectedIndex;
        private List<BebiAnimation> _actionClips;

        private void OnEnable()
        {
            _actionAnimator = (BebiAnimator)target;
            _actionClips = _actionAnimator.ActionClipList;

            _options = new string[_actionClips.Count];
            for (int i = 0; i < _options.Length; i++)
            {
                _options[i] = _actionClips[i].ActionID;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                DisplayActions();
            }

        }

        private void DisplayActions()
        {
            if (_options.Length == 0)
                return;

            _selectedIndex = EditorGUILayout.Popup(_selectedIndex, _options);

            if (GUILayout.Button("Play"))
            {
                _actionAnimator.Play(_actionClips[_selectedIndex].ActionID);
            }

            if (GUILayout.Button("Stop"))
            {
                _actionAnimator.Stop(_actionClips[_selectedIndex].ActionID);
            }
        }
    }
#endif

}
