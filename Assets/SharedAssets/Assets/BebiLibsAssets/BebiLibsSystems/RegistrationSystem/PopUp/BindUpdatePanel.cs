using UnityEngine;
using BebiLibs.RegistrationSystem;
using TMPro;
using UnityEngine.UI;
using BebiLibs.RegistrationSystem.Core;

namespace BebiLibs
{
    public class BindUpdatePanel : ErrorMessagePanel
    {

        [Header("Panel Components")]
        [SerializeField] private GameUserDataSO _userData;
        [SerializeField] private ProviderIconDataSO _providerIconDataSO;

        [SerializeField] protected Image Image_ProviderIcon;
        [SerializeField] private TMP_Text Text_NewUser;

        public override void Trigger_ButtonClick_ButtonSubmit()
        {
            base.Trigger_ButtonClick_ButtonSubmit();
        }

        public override string ProcessContentText()
        {
            UpdateProviderImage();
            UpdateBindSuccessText(Text_NewUser);
            return "";
        }

        private void UpdateProviderImage()
        {
            if (_providerIconDataSO.TryGetIcon(_userData.singInProvider, out Sprite sprite))
            {
                Image_ProviderIcon.sprite = sprite;
                Image_ProviderIcon.gameObject.SetActive(true);
            }
            else
            {
                Image_ProviderIcon.gameObject.SetActive(false);
            }
        }

        private void UpdateBindSuccessText(TMP_Text text)
        {
            string userName = !string.IsNullOrEmpty(_userData.userName) && _userData.userName.Length > 1 ? _userData.userName : "";
            string userText = !string.IsNullOrEmpty(_userData.userEmail) && _userData.userEmail.Length > 1 ? _userData.userEmail : userName;
            text.gameObject.SetActive(true);
            text.text = userText;
        }

    }
}
