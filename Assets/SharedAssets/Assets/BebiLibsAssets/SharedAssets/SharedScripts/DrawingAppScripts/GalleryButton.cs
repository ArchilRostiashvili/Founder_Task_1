using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BebiLibs;
using BebiLibs.Popups.Gallery;

namespace DrawingApp
{
    public class GalleryButton : MonoBehaviour
    {
        [SerializeField] private GameObject GO_NotificationElement;
        [SerializeField] private TMP_Text Text_Notification;
        [SerializeField] private bool _isNotificationActive = true;
        [SerializeField] private bool _useDefaultSound = true;

        private ButtonScale _buttonScale;

        private void Awake()
        {
            _buttonScale = this.GetComponent<ButtonScale>();
            _buttonScale.onClick.RemoveAllListeners();
            _buttonScale.onClick.AddListener(() => OpenGalleryButtonClick(_buttonScale.transform));
        }

        private void OnEnable()
        {
            UpdateNotification();
        }

        private void Start()
        {
            if (_isNotificationActive == false) return;
            UpdateNotification();
        }

        public void UpdateNotification()
        {
            if (_isNotificationActive == false) return;
            int notificationCount = GalleryPopup.GetNotificationCount();
            if (notificationCount == 0)
            {
                GO_NotificationElement.gameObject.SetActive(false);
            }
            else
            {
                GO_NotificationElement.gameObject.SetActive(true);
                Text_Notification.text = notificationCount.ToString();
            }
        }

        public void OpenGalleryButtonClick(Transform buttonPosition)
        {
            ParentalController.Activate(buttonPosition.position, () =>
            {
                GalleryPopup.Activate(false, _useDefaultSound);
                UpdateNotification();
            });
        }
    }
}
