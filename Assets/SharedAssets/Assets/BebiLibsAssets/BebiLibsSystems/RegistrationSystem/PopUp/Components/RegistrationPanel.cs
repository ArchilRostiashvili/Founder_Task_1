using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.RegistrationSystem.PopUp.Components
{
    public abstract class RegistrationPanel : MonoBehaviour
    {
        protected PopUp_Registration _popUpRegistration;

        [SerializeField] private Transform _content;
        public Transform TR_Content => _content;

        public virtual void Init(PopUp_Registration popUp_Registration)
        {
            _popUpRegistration = popUp_Registration;
        }

        public abstract void SetLoaderActive(bool value);

        public virtual void Show(bool activatedFromButton, bool refreshUI, bool isSignIn)
        {
            _content.localScale = Vector3.one;
            _content.gameObject.SetActive(true);
        }

        public virtual void Hide(bool refreshUI)
        {
            _content.localScale = Vector3.one;
            _content.gameObject.SetActive(false);
        }

        public virtual void Trigger_ButtonClick_Exit()
        {
            _popUpRegistration.HideFromButton(true);
        }
    }
}
