using UnityEngine;
using BebiLibs.RegistrationSystem.Core;
using UnityEngine.Events;

namespace BebiLibs.RegistrationSystem.PopUp.Components.Sign
{
    [ExecuteInEditMode]
    public class SignButton : MonoBehaviour
    {
        [SerializeField] private Provider _providerName;
        [SerializeField] private ButtonScale _buttonScale;

        public UnityEvent OnClick => _buttonScale.onClick;
        public Provider provider => _providerName;

        public void SetButtonActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}
