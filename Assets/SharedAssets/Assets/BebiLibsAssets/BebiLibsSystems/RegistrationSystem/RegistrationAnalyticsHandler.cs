using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs.RegistrationSystem;
using BebiLibs.Analytics;
using BebiLibs.Analytics.GameEventLogger;
using BebiLibs.RegistrationSystem.Core;
using System;

namespace BebiLibs.RegistrationSystem
{
    public class RegistrationAnalyticsHandler : AnalyticsHelperBase
    {

        private static RegistrationEventData _lastSingEvent;
        private static RegistrationEventData _lastBindEvent;
        [SerializeField] private GameUserDataSO _gameUserDataSO;
        PersistentInteger logdInUserCount = new PersistentInteger("RegistrationLogedInUserCount", 0);

        private void OnEnable()
        {
            ManagerRegistration.CallBack_SignInSuccessfull -= this.OnUserSignInComplete;
            ManagerRegistration.CallBack_SignInSuccessfull += this.OnUserSignInComplete;

            ManagerRegistration.CallBack_BindSuccess -= this.OnBindSuccessfull;
            ManagerRegistration.CallBack_BindSuccess += this.OnBindSuccessfull;
        }

        private void OnDisable()
        {
            ManagerRegistration.CallBack_SignInSuccessfull -= this.OnUserSignInComplete;
            ManagerRegistration.CallBack_BindSuccess -= this.OnBindSuccessfull;
        }


        public static void SignInShow(RegistrationEventData eventData)
        {
            AnalyticsManager.LogEvent(eventData.SignPrefix + "_show", "bid", eventData.Sender);
        }

        public static void SignInClick(RegistrationEventData eventData)
        {
            _lastSingEvent = eventData;
            GameParameterBuilder gameParameterBuilder = new GameParameterBuilder();
            gameParameterBuilder.Add("bid", eventData.Sender);
            gameParameterBuilder.Add("type", eventData.Provider.ToString());
            AnalyticsManager.LogEvent(eventData.SignPrefix + "_click", gameParameterBuilder);
        }

        internal static void NextButtonClick(RegistrationEventData eventData)
        {
            GameParameterBuilder gameParameterBuilder = new GameParameterBuilder();
            gameParameterBuilder.Add("bid", eventData.Sender);
            gameParameterBuilder.Add("type", eventData.Provider.ToString());
            AnalyticsManager.LogEvent(eventData.SignPrefix + "_next_click", gameParameterBuilder);
        }

        internal static void SendForgotPasswordButtonClickEvent(RegistrationEventData eventData)
        {
            GameParameterBuilder gameParameterBuilder = new GameParameterBuilder();
            gameParameterBuilder.Add("bid", eventData.Sender);
            gameParameterBuilder.Add("type", eventData.Provider.ToString());
            AnalyticsManager.LogEvent("forgot_password", gameParameterBuilder);
        }

        public static void SignInClose(RegistrationEventData eventData)
        {
            GameParameterBuilder gameParameterBuilder = new GameParameterBuilder();
            gameParameterBuilder.Add("bid", eventData.Sender);
            AnalyticsManager.LogEvent(eventData.SignPrefix + "_close", gameParameterBuilder);
        }

        public static void ShowProfilePopUp(RegistrationEventData eventData, GameUserDataSO gameUserDataSO)
        {
            if (!gameUserDataSO.isUserSignedIn) return;
            GameParameterBuilder gameParameterBuilder = new GameParameterBuilder();
            gameParameterBuilder.Add("bid", eventData.Sender);
            gameParameterBuilder.Add("type", eventData.Provider.ToString());
            gameParameterBuilder.Add("sub", gameUserDataSO.userID);
            AnalyticsManager.LogEvent(eventData.SignPrefix + "_show", gameParameterBuilder);
        }

        public static void CloseProfilePopUp(RegistrationEventData eventData, GameUserDataSO gameUserDataSO)
        {
            if (!gameUserDataSO.isUserSignedIn) return;
            GameParameterBuilder gameParameterBuilder = new GameParameterBuilder();
            gameParameterBuilder.Add("bid", eventData.Sender);
            gameParameterBuilder.Add("type", eventData.Provider.ToString());
            gameParameterBuilder.Add("sub", gameUserDataSO.userID);
            AnalyticsManager.LogEvent(eventData.SignPrefix + "_close", gameParameterBuilder);
        }

        public static void SignUserOut(RegistrationEventData eventData, GameUserDataSO gameUserDataSO)
        {
            if (!gameUserDataSO.isUserSignedIn) return;
            GameParameterBuilder gameParameterBuilder = new GameParameterBuilder();
            gameParameterBuilder.Add("bid", eventData.Sender);
            gameParameterBuilder.Add("type", eventData.Provider.ToString());
            gameParameterBuilder.Add("sub", gameUserDataSO.userID);
            AnalyticsManager.LogEvent("sign_out", gameParameterBuilder);
        }

        public static void BindButtonClick(RegistrationEventData eventData, GameUserDataSO gameUserDataSO)
        {
            _lastBindEvent = eventData;
            if (!gameUserDataSO.isUserSignedIn) return;
            GameParameterBuilder gameParameterBuilder = new GameParameterBuilder();
            gameParameterBuilder.Add("bid", eventData.Sender);
            gameParameterBuilder.Add("type", eventData.Provider.ToString());
            gameParameterBuilder.Add("sub", gameUserDataSO.userID);
            AnalyticsManager.LogEvent("bind_click", gameParameterBuilder);
        }

        public static void DeleteUserAccountButtonClick(RegistrationEventData eventData, GameUserDataSO gameUserDataSO)
        {
            _lastBindEvent = eventData;
            if (!gameUserDataSO.isUserSignedIn) return;
            GameParameterBuilder gameParameterBuilder = new GameParameterBuilder();
            gameParameterBuilder.Add("bid", eventData.Sender);
            gameParameterBuilder.Add("type", eventData.Provider.ToString());
            gameParameterBuilder.Add("sub", gameUserDataSO.userID);
            AnalyticsManager.LogEvent("delete_account_click", gameParameterBuilder);
        }


        public void OnBindSuccessfull()
        {
            if (_lastBindEvent == null || !_gameUserDataSO.isUserSignedIn) return;
            GameParameterBuilder gameParameterBuilder = new GameParameterBuilder();
            gameParameterBuilder.Add("bid", _lastBindEvent.Sender);
            gameParameterBuilder.Add("type", _lastBindEvent.Provider.ToString());
            gameParameterBuilder.Add("sub", _gameUserDataSO.userID);
            AnalyticsManager.LogEvent("bind", gameParameterBuilder);
        }

        public void OnUserSignInComplete()
        {
            if (_gameUserDataSO.isUserSignedIn)
            {
                this.HandleUserSignIn();
            }
        }

        public void HandleUserSignIn()
        {
            if (_lastSingEvent == null || !_gameUserDataSO.isUserSignedIn) return;
            this.logdInUserCount.SetValue(this.logdInUserCount + 1);

            GameParameterBuilder gameParameterBuilder = new GameParameterBuilder();
            gameParameterBuilder.Add("bid", _lastSingEvent.Sender);
            gameParameterBuilder.Add("type", _lastSingEvent.Provider.ToString());
            gameParameterBuilder.Add("sub", _gameUserDataSO.userID);
            AnalyticsManager.LogEvent(_lastSingEvent.SignPrefix + "_done", gameParameterBuilder);
            AnalyticsManager.SetProperty("sub", _gameUserDataSO.userID);
            AnalyticsManager.SetProperty("accounts", this.logdInUserCount.GetValue().ToString());
        }


    }
}
