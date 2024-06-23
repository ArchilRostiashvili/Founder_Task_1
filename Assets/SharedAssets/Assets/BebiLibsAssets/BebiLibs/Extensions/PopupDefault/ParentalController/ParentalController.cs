using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BebiLibs.Analytics;
using I2.Loc;
using BebiLibs.PopupManagementSystem;

namespace BebiLibs
{
    public class ParentalController : PopUpBase
    {
        private static ParentalController _Instance;
        private static Action _ParentalPassedEvent;
        private static Action<CancellationReason> _ParentalCanceledEvent;

        public ItemParticleSystem PS;
        public Color Color_Glow1;
        public Color Color_Glow2;

        public Sprite SP_Idle;
        public Sprite SP_Correct;
        public Sprite SP_Wrong;

        public Image Image_Arrow;

        public GameObject Back;
        public GameObject[] arrayArrows;
        public List<ButtonAnswer> arrayAnswers;
        public int[] arrayValues;
        public TextMeshProUGUI Text_Question;
        private int _index;

        private Coroutine _c;

        private PopUpAction _popUpAction;
        private float _popupScale = 1.0f;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private bool _IsParentalActive = true;

        private void Awake()
        {
            Init();
        }

        override public void Init()
        {
            _Instance = this;
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float scale = screenWidth / screenHeight;
            //Common.DebugLog("scale = " + scale);
            if (scale < 1.61f)
            {
                _popupScale = 0.56f;
                TR_Content.localScale = Vector3.one * _popupScale;
            }
            else
            {
                //TR_Content.localScale = Vector3.one * 0.9f;
            }
        }

        public static void SetParentalActiveForDebug(bool value)
        {
            PopupManager.GetPopup<ParentalController>((popup) =>
            {
                popup.Hide();
                _Instance = popup;
                _Instance._IsParentalActive = value;
            });
        }

        public static void Activate(Vector2 p, Action parentalPassedEvent = null, string correctArrow = null, Action<CancellationReason> parentalCanceledEvent = null)
        {
            PopupManager.GetPopup<ParentalController>((popup) =>
            {
                OnInstantiate(popup, p, parentalPassedEvent, correctArrow, parentalCanceledEvent);
            });
        }

        private static void OnInstantiate(ParentalController parentalController, Vector2 p, Action parentalPassedEvent, string correctArrow, Action<CancellationReason> parentalCanceledEvent)
        {
            _ParentalPassedEvent = parentalPassedEvent;
            _ParentalCanceledEvent = parentalCanceledEvent;
            _Instance = parentalController;
            if (_Instance != null)
            {
                if (!_Instance._IsParentalActive)
                {
                    parentalPassedEvent?.Invoke();
                    return;
                }

                _Instance.Show(p, correctArrow);
            }
            else
            {
                parentalPassedEvent?.Invoke();
            }
        }

        public void Show(Vector2 p, string correctArrow)
        {
            //ManagerAnalyticsCustom.SetScene("p_p");//pop_parent
            _canvas.worldCamera = Camera.main;
            gameObject.SetActive(true);
            TR_Content.DOScale(_popupScale, 0.2f);

            List<int> array = new List<int>();
            for (int i = 1; i <= 9; i++)
            {
                array.Add(i);
            }

            ShuffleButtons(ref arrayAnswers);

            arrayValues = new int[3];
            string value = "";
            for (int i = 0; i < 4; i++)
            {
                int index = UnityEngine.Random.Range(0, array.Count);
                arrayAnswers[i].SetValue(array[index]);
                arrayAnswers[i].Image_Back.sprite = SP_Idle;
                if (i < 3)
                {
                    arrayValues[i] = array[index];
                    value += array[index];
                }
                array.RemoveAt(index);
            }

            _index = 0;
            PS.Hide();
            string formatString = LocalizationManager.GetTranslation("TEXT_TYPE_TO_CONTINUE");
            Text_Question.text = string.Format(formatString, "<color=#c1ff3d>" + value + "<color=#FFFFFF>");
            GetArrow(p, correctArrow);
            EnableButtons(true);

            ManagerSounds.PlayEffect("fx_page11");

            Image_Arrow.DOKill();
            Image_Back.DOKill();

            Image_Arrow.color = Color.white;
            Image_Back.color = Color.white;

            if (_c != null)
            {
                StopCoroutine(_c);
                _c = null;
            }

            _c = StartCoroutine(DelayClose(8.0f, PopUpAction.TIME_OUT));
        }

        private void GetArrow(Vector2 p, string correctArrow)
        {
            for (int i = 0; i < arrayArrows.Length; i++)
            {
                arrayArrows[i].SetActive(false);
            }

            float width = Screen.width;
            float height = Screen.height;
            float xScale = p.x / (width * 7.68f / height);
            float yScale = p.y / 7.68f;

            int column;

            if (xScale < -0.6f)
            {
                column = 0;
            }
            else
            if (xScale < -0.2f)
            {
                column = 1;
            }
            else
            if (xScale < 0.2f)
            {
                column = 2;
            }
            else
            if (xScale < 0.6f)
            {
                column = 3;
            }
            else
            {
                column = 4;
            }


            int row;

            if (yScale < -0.6f)
            {
                row = 0;
            }
            else
            if (yScale < -0.2f)
            {
                row = 1;
            }
            else
            if (yScale < 0.2f)
            {
                row = 2;
            }
            else
            if (yScale < 0.6f)
            {
                row = 3;
            }
            else
            {
                row = 4;
            }

            Transform tr;
            if (correctArrow != null && correctArrow != "")
            {
                tr = Back.transform.Find("Back_Arrow_" + correctArrow);
            }
            else
            if (row == 2 && column == 2)
            {
                tr = Back.transform.Find("Back_Arrow_" + 4 + "_" + 2);
            }
            else
            if (1 <= row && row <= 3)
            {
                if (column < 2)
                {
                    column = 0;
                }
                else
                {
                    column = 4;
                }
                tr = Back.transform.Find("Back_Arrow_" + row + "_" + column);
            }
            else
            {
                tr = Back.transform.Find("Back_Arrow_" + row + "_" + column);
            }

            if (tr != null)
            {
                tr.gameObject.SetActive(true);
                Image_Arrow = tr.GetComponent<Image>();
                tr = tr.Find("Point");
                Vector2 diff = tr.position - TR_Content.position;
                TR_Content.position = p - diff;
            }
        }

        private static System.Random _rng;

        public void ShuffleButtons(ref List<ButtonAnswer> list)
        {
            if (_rng == null)
            {
                _rng = new System.Random();
            }

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                ButtonAnswer value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public void Hide()
        {
            Image_Arrow.DOKill();
            Image_Back.DOKill();
            this.DOKill();
            gameObject.SetActive(false);

            switch (_popUpAction)
            {
                case PopUpAction.NONE:
                    _ParentalCanceledEvent?.Invoke(CancellationReason.OUTSIDE_CLICK);
                    break;
                case PopUpAction.PASSED:
                    _ParentalPassedEvent?.Invoke();
                    break;
                case PopUpAction.FAILED:
                    _ParentalCanceledEvent?.Invoke(CancellationReason.WRONG_COMBINATION);
                    break;
                case PopUpAction.TIME_OUT:
                    _ParentalCanceledEvent?.Invoke(CancellationReason.TIME_OUT);
                    break;
            }
        }

        public void Trigger_TouchBack()
        {
            ManagerSounds.PlayEffect("fx_page14");
            if (_c != null)
            {
                StopCoroutine(_c);
                _c = null;
            }
            Hide();
        }

        public void Trigger_ButtonClick(ButtonAnswer button)
        {
            if (_c != null)
            {
                StopCoroutine(_c);
                _c = null;
            }

            if (button.answerValue == arrayValues[_index])
            {
                button.Image_Back.sprite = SP_Correct;

                _index++;
                if (_index == 3)
                {
                    ManagerSounds.PlayEffect("fx_successhigh5");
                    _c = StartCoroutine(DelayClose(0.65f, PopUpAction.PASSED));
                    EnableButtons(false);

                    Image_Arrow.DOKill();
                    Image_Back.DOKill();
                    this.DOKill();

                    PS.Play();
                    Image_Arrow.color = Color.white;
                    Image_Back.color = Color.white;



                    Sequence s = DOTween.Sequence();
                    s.AppendCallback(() =>
                    {
                        Image_Arrow.DOColor(Color_Glow1, 0.1f).SetLoops(2, LoopType.Yoyo);
                        Image_Back.DOColor(Color_Glow1, 0.1f).SetLoops(2, LoopType.Yoyo);
                    });
                    s.AppendInterval(0.22f);
                    s.AppendCallback(() =>
                    {
                        Image_Arrow.DOColor(Color_Glow2, 0.1f).SetLoops(2, LoopType.Yoyo);
                        Image_Back.DOColor(Color_Glow2, 0.1f).SetLoops(2, LoopType.Yoyo);
                    });
                    s.SetId(this);
                }
                else
                {
                    ManagerSounds.PlayEffect("fx_page17");
                }
            }
            else
            {
                ManagerSounds.PlayEffect("fx_parentalfail");
                _c = StartCoroutine(DelayClose(0.5f, PopUpAction.FAILED));
                EnableButtons(false);
                button.Image_Back.sprite = SP_Wrong;
            }
        }

        private void EnableButtons(bool bl)
        {
            for (int i = 0; i < arrayAnswers.Count; i++)
            {
                arrayAnswers[i].Enable(bl);
            }
        }

        private IEnumerator DelayClose(float time, PopUpAction popUpAction)
        {
            _popUpAction = popUpAction;
            yield return new WaitForSeconds(time);

            if (_popUpAction == PopUpAction.TIME_OUT)
            {
                ManagerSounds.PlayEffect("fx_parentalfail");
            }
            Hide();
        }
    }
}
