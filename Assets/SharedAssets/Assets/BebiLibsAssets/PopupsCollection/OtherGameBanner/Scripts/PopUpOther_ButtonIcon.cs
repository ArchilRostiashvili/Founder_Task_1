using System.Collections;
using System.Collections.Generic;
using BebiLibs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BebiLibs
{
    public class PopUpOther_ButtonIcon : MonoBehaviour
    {
        public string appID;
        public Image Image_Icon;
        public Image Image_Back;
        public GameObject GO_IconDownload;
        public GameObject GO_IconDone;
        public Image Image_Arrow;
        public TextMeshProUGUI Text_Name;
        private DataOtherApp _dataOtherApp;

        public void SetData(DataOtherApp dataOtherApp)
        {
            _dataOtherApp = dataOtherApp;
            this.Image_Icon.sprite = _dataOtherApp.sprite_icon;
            this.Text_Name.text = _dataOtherApp.app_name;
            this.gameObject.SetActive(true);

            this.Refresh();
        }

        public void Refresh()
        {
            if (_dataOtherApp == null)
            {
                return;
            }
            
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
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public DataOtherApp CurrentData
        {
            get
            {
                return _dataOtherApp;
            }
        }

        public void Select(bool bl)
        {
            if (bl)
            {
                this.Image_Back.color = Color.white;
                this.Image_Back.transform.localScale = Vector3.one * 1.1f;
                this.Image_Arrow.gameObject.SetActive(true);
            }
            else
            {
                this.Image_Back.color = Color.black;
                this.Image_Back.transform.localScale = Vector3.one;// * 0.955f;
                this.Image_Arrow.gameObject.SetActive(false);
            }
        }
    }
}
