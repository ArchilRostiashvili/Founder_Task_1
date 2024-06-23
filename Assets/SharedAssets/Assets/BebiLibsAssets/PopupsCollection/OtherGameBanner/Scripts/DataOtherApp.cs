using SimpleJSON;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.U2D;

namespace BebiLibs
{
    [System.Serializable]
    public class DataOtherApp
    {
        public string app_id;
        public string test_id;

        private int _triesTotal;
        private int _tries;

        public string app_name;
        public int version;
        public string id_ios;
        public string id_ios_scheme;
        public string id_android;

        public Sprite sprite_mainbutton;
        public Sprite sprite_icon;
        public Sprite sprite_banner;

        public bool isValid;

        private bool _installed;
        private AssetBundle _bundle;

        public static DataOtherApp Create(JSONNode dataNode)
        {
            DataOtherApp data = new DataOtherApp();
            data.app_id = dataNode["appid"];
            data._installed = false;

            data._triesTotal = dataNode["try"].AsInt;
            data._tries = PlayerPrefs.GetInt(data.app_id + "_try", -1);
            if (data._tries == -1)
            {
                data._tries = data._triesTotal;
            }

            JSONArray arrayNodes = dataNode["t"].AsArray;
            int total = 0;
            for (int i = 0; i < arrayNodes.Count; i++)
            {
                total += arrayNodes[i]["p"].AsInt;
            }

            int value = UnityEngine.Random.Range(0, total);
            int count = 0;
            for (int i = 0; i < arrayNodes.Count; i++)
            {
                count += arrayNodes[i]["p"].AsInt;
                if (value < count)
                {
                    if (data.test_id != arrayNodes[i]["id"])
                    {
                        data.test_id = arrayNodes[i]["id"];
                    }
                    break;
                }
            }

            return data;
        }

        public bool TryShow()
        {
            bool bl = false;

            if (0 < _tries)
            {
                bl = true;
            }

            _tries--;
            if (_tries <= 0)
            {
                _tries = 0;
            }

            PlayerPrefs.SetInt(this.app_id + "_try", _tries);

            return bl;
        }

        public void TryReset()
        {
            _tries = _triesTotal;
            PlayerPrefs.SetInt(this.app_id + "_try", _tries);
        }

        public void SetData(JSONNode dataNode)
        {
            this.app_name = dataNode["app_name"];
            this.id_ios = dataNode["id_ios"];
            this.id_ios_scheme = dataNode["id_ios_scheme"];
            this.id_android = dataNode["id_android"];
        }

        public string TotalID
        {
            get
            {
                return this.app_id + "_" + this.test_id;
            }
        }

        public void SetVersion(int v)
        {
            this.version = v;
        }

        public void Installed(bool bl)
        {
            _installed = bl;
        }

        public bool IsInstalled
        {
            get
            {
                return _installed;
            }
        }

        public void LoadBundle(MonoBehaviour reff, Action callBack)
        {
            //             string platform = "";
            // #if UNITY_ANDROID
            //             platform = "android";
            // #elif UNITY_IOS
            //         platform = "ios"; 
            // #endif
            //             _bundle = null;

            //reff.StartCoroutine(GetAssetBundle(this.version, ManagerAppConfig.GetDefaultInstance().IapBannersURl + "ad/" + platform + "/" + this.app_id + "_" + this.test_id + ".zip"));
            // IEnumerator GetAssetBundle(int v, string url)
            // {
            //     //Common.DebugLog("main_icon_url = " + main_icon_url);
            //     UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url, (uint)v, 0);
            //     yield return www.SendWebRequest();

            //     if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            //     {
            //         Common.DebugLog(www.error);
            //     }
            //     else
            //     {
            //         _bundle = DownloadHandlerAssetBundle.GetContent(www);
            //         if (_bundle != null)
            //         {
            //             SpriteAtlas sa = _bundle.LoadAsset<SpriteAtlas>("sprites");
            //             if (sa != null)
            //             {
            //                 this.sprite_mainbutton = sa.GetSprite("btn_" + this.app_id + "_" + this.test_id);
            //                 this.sprite_icon = sa.GetSprite("icon_" + this.app_id + "_" + this.test_id);
            //                 this.sprite_banner = sa.GetSprite("banner_" + this.app_id + "_" + this.test_id);

            //                 if (this.sprite_mainbutton == null || this.sprite_icon == null || this.sprite_banner == null)
            //                 {
            //                     this.isValid = false;
            //                     callBack?.Invoke();
            //                 }
            //                 else
            //                 {
            //                     this.isValid = true;
            //                     callBack?.Invoke();
            //                 }
            //             }
            //         }
            //     }
            // }
        }
    }
}
