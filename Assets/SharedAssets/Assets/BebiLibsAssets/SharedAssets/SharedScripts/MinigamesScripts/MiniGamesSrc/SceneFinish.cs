using BebiLibs;
using BebiLibs.Analytics;
using BebiLibs.ModulesGameSystem;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFinish : MonoBehaviour
{
    public GameObject GO_Content;

    public System.Action CallBack_OnGameLoad;
    public System.Action<bool> CallBack_OnGameEnds;

    public BalloonSystem SC_BalloonSystem;
    public GameObject GO_ButtonToMap;
    public SpriteRenderer SP_Back;
    public Coroutine _cAuto;

    public void Activate()
    {
        SP_Back.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        SP_Back.DOFade(40.0f / 255.0f, 0.1f);

        GO_Content.SetActive(true);
        Win();

        ManagerSounds.PlayEffect("fx_children_chear2");

        GO_ButtonToMap.SetActive(false);
        ManagerTime.Delay(1.0f, () =>
        {
            GO_ButtonToMap.SetActive(true);
        });

        _cAuto = ManagerTime.Delay(11.0f, () =>
        {
            //ManagerAnalyticsCustom.LogEvent("ftm", "t", 1);
            CallBack_OnGameEnds?.Invoke(false);
        });
    }

    public void Hide(int animType)
    {
        if (animType == 1)
        {
            GO_ButtonToMap.SetActive(false);
            SP_Back.DOFade(0.0f, 0.1f);
            HideWin();
            ManagerTime.Delay(1.0f, () =>
            {
                GO_Content.SetActive(false);
            });
        }
        else
        {
            GO_ButtonToMap.SetActive(false);
            SP_Back.DOFade(0.0f, 0.1f);
            HideWin();
            ManagerTime.Delay(1.0f, () =>
            {
                GO_Content.SetActive(false);
            });
        }
    }

    public void Win()
    {
        SC_BalloonSystem.Activate();
    }

    public void HideWin()
    {
        SC_BalloonSystem.Hide(0);
    }

    public void Trigger_ButtonClick_ToMap()
    {
        if (_cAuto != null)
        {
            ManagerTime.Instance.StopCoroutine(_cAuto);
            _cAuto = null;
        }

        //ManagerAnalyticsCustom.LogEvent("ftm", "t", 0);
        ManagerSounds.PlayEffect("fx_correct9");
        CallBack_OnGameEnds?.Invoke(true);
    }
}
