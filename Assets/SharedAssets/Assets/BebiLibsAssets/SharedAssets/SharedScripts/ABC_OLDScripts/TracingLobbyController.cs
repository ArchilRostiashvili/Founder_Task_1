using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;


namespace ABC_OLD
{
    public class TracingLobbyController : MonoBehaviour
    {
        public GameObject GO_Lobby_iPad;
        public GameObject GO_Lobby_iPhone;
        private GameObject _activeLobby;
        public System.Action CallBack_ExitGame;

        void Start()
        {
            //ManagerSounds.PlayEffect("fx_tx_welcome", 0.0f, 0.4f);
            ManagerSounds.PlayBackground("music_tropic", 1f, 0, 0.05f);

            _activeLobby = GetActiveLobbyPanel();
            _activeLobby.SetActive(true);
        }

        public void DisableTracingLobby()
        {
            _activeLobby.SetActive(false);
        }

        public void ActivateTracingLobby()
        {
            _activeLobby.SetActive(true);
        }

        public GameObject GetActiveLobbyPanel()
        {
            MobileDeviceType deviceType = GetDeviceType();
            if (deviceType == MobileDeviceType.TABLET)
            {
                GameObject.DestroyImmediate(GO_Lobby_iPhone.gameObject);
                return GO_Lobby_iPad;
            }
            else if (deviceType == MobileDeviceType.PHONE)
            {
                GameObject.DestroyImmediate(GO_Lobby_iPad.gameObject);
                return GO_Lobby_iPhone;
            }
            return null;
        }

        public void Trigger_ButtonClick_Exit()
        {
            ManagerSounds.PlayEffect("fx_page14");
            CallBack_ExitGame?.Invoke();
        }


        public static MobileDeviceType GetDeviceType()
        {
            int width = Screen.width;
            int height = Screen.height;
            float aspectRatio = Mathf.Max(width, height) / Mathf.Min(width, height);
            bool isTablet = DeviceDiagonalSizeInInches() > 6.5f && aspectRatio < 2f;
            //bool isTablet = DeviceDiagonalSizeInInches() > 6.5f;
            if (isTablet)
            {
                return MobileDeviceType.TABLET;
            }
            else
            {
                return MobileDeviceType.PHONE;
            }
        }

        private static float DeviceDiagonalSizeInInches()
        {
            float screenWidth = Screen.width / Screen.dpi;
            float screenHeight = Screen.height / Screen.dpi;
            float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
            return diagonalInches;
        }
    }
}