using BebiLibs;
using BebiLibs.RegistrationSystem.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.RegistrationSystem
{
    [CreateAssetMenu(fileName = "RegistrationEventMediator", menuName = "BebiLibs/RegistrationSystem/RegistrationEventMediator", order = 0)]
    public class RegistrationEventMediatorSO : ScriptableObject
    {
        public event Action<Provider> SignInButtonClickEvent;
        public event Action BindButtonClickEvent;
        public event Action SignOutButtonClickEvent;
        public event Action DeleteButtonClickEvent;
        public event Action<bool> RegistrationPopupCloseEvent;
        public event Action PopupOpenEvent;
        public event Action<UserCredential> CreateAccountButtonClickEvent;
        public event Action<UserCredential> ManualSignInButtonClickEvent;
        public event Action<UserCredential> ForgetPasswordButtonClickEvent;



        public void InvokeSingInAction(Provider provider) => SignInButtonClickEvent?.Invoke(provider);
        public void InvokeDeleteAction() => DeleteButtonClickEvent?.Invoke();
        public void InvokeBindAction() => BindButtonClickEvent?.Invoke();
        public void InvokeSignOutAction() => SignOutButtonClickEvent?.Invoke();
        public void InvokeRegistrationPopupCloseAction(bool isFromButton) => RegistrationPopupCloseEvent?.Invoke(isFromButton);
        public void InvokePopupOpenAction() => PopupOpenEvent?.Invoke();

        public void InvokeNextButtonClickEvent(UserCredential userCredential) => CreateAccountButtonClickEvent?.Invoke(userCredential);
        public void InvokeManualSignInButtonClickEvent(UserCredential userCredential) => ManualSignInButtonClickEvent?.Invoke(userCredential);
        public void InvokeForgetPasswordButtonClickEvent(UserCredential userCredential) => ForgetPasswordButtonClickEvent?.Invoke(userCredential);
    }
}
