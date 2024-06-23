using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BebiLibs
{
    public class ButtonOtherApp : MonoBehaviour
    {
        public Image Image_Icon;
        public GameObject GO_IconDownload;
        public GameObject GO_IconDone;
        public TextMeshProUGUI Text_Name;
        private DataOtherApp _dataOtherApp;

        public void SetData(DataOtherApp dataOtherApp)
        {
            if (dataOtherApp == null)
            {
                this.gameObject.SetActive(false);
                return;
            }

            _dataOtherApp = dataOtherApp;

            if (_dataOtherApp.IsInstalled)
            {
                this.GO_IconDownload.SetActive(false);
                this.GO_IconDone.SetActive(true);
            }
            else
            {
                this.GO_IconDownload.SetActive(true);
                this.GO_IconDone.SetActive(false);
            }

            this.gameObject.SetActive(true);
            this.Text_Name.text = dataOtherApp.app_name;
            this.Image_Icon.sprite = _dataOtherApp.sprite_icon;
            //this.Image_Icon.SetNativeSize();
            //this.Image_Icon.transform.localScale = Vector3.one * 0.8f;
        }

        public DataOtherApp CurrentData
        {
            get
            {
                return _dataOtherApp;
            }
        }
    }
}
