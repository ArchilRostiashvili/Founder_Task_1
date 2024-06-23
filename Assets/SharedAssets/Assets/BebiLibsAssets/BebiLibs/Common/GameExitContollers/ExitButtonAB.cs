using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BebiLibs
{
    public class ExitButtonAB : AbstractExitButton
    {
        private const string _exitButtonPrefKey = "GameExitButtonTestKey";

        // [SerializeField] private AbstractExitButton _swipeButton;
        // [SerializeField] private AbstractExitButton _tapButton;
        [SerializeField] private AbstractExitButton _defaultButton;
        [SerializeField] private Canvas _buttonCanvas;
        private bool _isInitialized = false;

        private void Awake()
        {
            Initialize();
        }

        private void OnEnable()
        {
            if (_buttonCanvas != null)
            {
                _buttonCanvas.worldCamera = Camera.main;
            }

            Initialize();
            Show();
        }

        protected override void Initialize()
        {
            if (_isInitialized)
            {
                _defaultButton.SetListener(_gameExitEvent);
                return;
            }

            _isInitialized = true;

            _defaultButton.SetListener(_gameExitEvent);
        }

        public override void AddListener(UnityAction exitAction)
        {
            Initialize();
            _defaultButton.AddListener(exitAction);
        }

        public override void RemoveListener(UnityAction exitAction)
        {
            Initialize();
            _defaultButton.RemoveListener(exitAction);
        }

        public override void RemoveAllListeners()
        {
            Initialize();
            _defaultButton.RemoveAllListeners();
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            _defaultButton.Show();
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
            _defaultButton.Hide();
        }

    }
}
