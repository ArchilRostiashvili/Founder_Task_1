using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BebiLibs
{
    public class TapExitButton : AbstractExitButton
    {
        [SerializeField] private ButtonScale _buttonScale;

        public override void AddListener(UnityAction exitAction)
        {
            _buttonScale.onClick.AddListener(exitAction);
        }

        public override void RemoveListener(UnityAction exitAction)
        {
            _buttonScale.onClick.RemoveListener(exitAction);
        }

        public override void RemoveAllListeners()
        {
            _buttonScale.onClick.RemoveAllListeners();
        }

        public void OnButtonClick()
        {
            ManagerSounds.PlayEffect("fx_page16");
            _gameExitEvent?.Invoke();
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

    }
}
