using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;
using BebiLibs.Analytics;

public class PopUp_ShareForGift : PopUpBase
{
    private static PopUp_ShareForGift _instance;

    public ButtonScale[] arrayButtons;
    public Transform TR_ContentPrefab;

    private NativeShare _nativeShare;
    override public void Init()
    {
        base.Init();

        _instance = this;
    }


    override public void Show(bool anim)
    {
        SharedAnalyticsManager.SetScene("p_sfg");//pop_shareforgift
        AnalyticsManager.LogEvent("p_sfg_s");//pop_shareforgift_show
        base.Show(anim);
        _shareClicked = false;
        this.EnableButtons(true);
    }

    private bool _shareClicked;
    private double _timeStamp;
    public void Trigger_ButtonClick_Share(GameObject go)
    {
        //ManagerAnalyticsCustom.LogEvent("p_sfg_c", "id", go.name.Split('_')[1]);//pop_shareforgift_click

        this.EnableButtons(false);

        _timeStamp = Common.TimeNow_Seconds();

        _shareClicked = true;
        //SceneMap scene = (SceneMap)SceneController.GetScene(AppScenes.MAP);

        _nativeShare.Share();
    }

    void OnApplicationFocus(bool active)
    {
        if (active)
        {
            //your app is NO LONGER in the background
            if (_shareClicked)
            {
                double diff = Common.TimeNow_Seconds() - _timeStamp;
                if (diff <= 6.0f)
                {

                }
                else
                {
                    ManagerSounds.PlayEffect("fx_tx_great");
                    ManagerSounds.PlayEffect("fx_successhigh2");
                    // _instance._itemGame.dataGameSO.Unlock();
                    // _instance._itemGame.Unlock(false);

                    this.Hide(false);
                }
            }
        }
        else
        {

        }
    }

    private void EnableButtons(bool bl)
    {
        for (int i = 0; i < this.arrayButtons.Length; i++)
        {
            this.arrayButtons[i].buttonEnabled = bl;
        }
    }

    override public void Hide(bool anim)
    {
        base.Hide(anim);
    }

    override public void Trigger_ButtonClick_Close()
    {
        base.Trigger_ButtonClick_Close();
    }
}
