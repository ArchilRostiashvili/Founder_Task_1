using System;
using Bebi.Helpers;
using FarmLife.Data.LobbySession;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmLife
{
    public class FarmParallaxController : MonoBehaviour
    {
        [SerializeField] private InteractionsController _interactionsController;
        [SerializeField] private GameLoader _gameLoader;
        [SerializeField] private WorldspaceScroll _worldSpaceScroll;
        [SerializeField] private Collider2D _parallaxCollider;

        public void Init()
        {
            _interactionsController.Activate();
            _gameLoader.Init();
            _worldSpaceScroll.GetViewBounds();
            _worldSpaceScroll.IdleTimePassedEvent = OnIdleTimePassed;
        }

        private void OnIdleTimePassed()
        {
            _gameLoader.LoadRandomGame();
        }

        public void SetEvents(Action<string> action)
        {
            _gameLoader.SetEvent(action);
        }

        public void ActivateGameLoader()
            => _gameLoader.CanTrackTime(true);

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
            => gameObject.SetActive(true);

        public void Reset()
            => _gameLoader.CameraZoom.ResetCamera();

        public void EnableScrolling(bool enable)
        {
            _worldSpaceScroll.enabled = enable;
            _parallaxCollider.enabled = enable;
        }

        public void EnableButtons(bool enable)
            => _gameLoader.EnableButtons(enable);

        public void SetSessionData(LobbySessionData lobbySessionData)
            => _worldSpaceScroll.SetLobbySession(lobbySessionData);

        public void Activate(LobbySessionData lobbySessionData)
            => _worldSpaceScroll.Activate(lobbySessionData);
    }
}