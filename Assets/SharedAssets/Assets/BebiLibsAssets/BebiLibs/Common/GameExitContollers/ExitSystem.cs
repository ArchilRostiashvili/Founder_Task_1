using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BebiLibs
{
    public class ExitSystem : AbstractExitButton
    {
        public SpriteRenderer SR_Back;
        public TextMeshPro Text_Instruction;

        //private static ExitSystem _instance;

        [SerializeField]
        private ExitButton_AB _exitButton_AB;
        public Transform TR_X;

        public Collider2D buttonCollider2D;
        private Touch _touch;
        private int _state = 0;
        private Vector3 _diffPos;

        public float _defaultAlpha = 0.5f;
        public static bool IsInteractive;


        // public static void SetCallBack_Exit(System.Action CallBack_Exit)
        // {
        //     ManagerSounds.PlayEffect("fx_page16");
        //     _callBack_Exit = CallBack_Exit;
        // }

        private void Start()
        {
            _state = 0;
            Text_Instruction.alpha = 0.0f;
            SR_Back.color = new Color(1.0f, 1.0f, 1.0f, _defaultAlpha);
            TR_X.localPosition = Vector3.zero;
            //_instance = this;
        }

        public bool GetIsDrag()
        {
            if (_exitButton_AB != null)
                return _exitButton_AB.GetIsDrag();
            return true;
        }
        public override void Show()
        {
            gameObject.SetActive(true);
            ExitSystem.IsInteractive = true;
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
            ExitSystem.IsInteractive = false;
        }

        // public static void Blink()
        // {
        //     if (_instance != null)
        //     {
        //         _instance.transform.DOScale(1.16f, 0.3f).SetLoops(4, LoopType.Yoyo);
        //         _instance.Text_Instruction.DOKill();
        //         _instance.Text_Instruction.alpha = 1.0f;
        //         //_instance.Text_Instruction.DOFade(1.0f, 0.3f).SetLoops(5, LoopType.Yoyo);
        //     }
        // }

        private void Update()
        {
            if (!ExitSystem.IsInteractive)
            {
                return;
            }

#if UNITY_EDITOR
            /*
            bool canTouch = true;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                canTouch = false;
            }
            */
            if (Input.GetMouseButtonDown(0) && _state == 0)// && canTouch)
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (buttonCollider2D.OverlapPoint(mouseWorldPosition))
                {
                    SR_Back.DOFade(1.0f, 0.05f);
                    Text_Instruction.DOKill();
                    Text_Instruction.DOFade(1.0f, 0.05f);
                    _state = 1;
                    TR_X.DOKill();
                    _diffPos = mouseWorldPosition - TR_X.position;
                }
            }
            else if (Input.GetMouseButton(0) && _state == 1)// && canTouch)
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition = transform.InverseTransformPoint(mouseWorldPosition);

                Vector3 p = mouseWorldPosition + _diffPos;
                p.y = TR_X.localPosition.y;
                p.z = 0.0f;

                if (2.48f < p.x)
                {
                    p.x = 2.48f;
                }
                else
                if (p.x < 0.0f)
                {
                    p.x = 0.0f;
                }

                TR_X.localPosition = p;
            }
            else if (Input.GetMouseButtonUp(0) && _state == 1)
            {
                _state = 0;

                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition = transform.InverseTransformPoint(mouseWorldPosition);

                float xValue = mouseWorldPosition.x + _diffPos.x;

                if (2.48f <= xValue)
                {
                    DoExit();
                    return;
                }
                else
                {
                    SR_Back.DOFade(_defaultAlpha, 0.05f);
                    Text_Instruction.DOKill();
                    Text_Instruction.DOFade(0.0f, 0.05f);
                    TR_X.DOLocalMoveX(0.0f, 0.05f);
                }
            }
#else
            Touch touch;
            for (int i = 0; i < Input.touchCount; i++)
            {
                touch = Input.GetTouch(i);
                /*
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId) && touch.phase != TouchPhase.Ended)
                {
                    continue;
                }
                */
                if (touch.phase == TouchPhase.Began && _state == 0)
                {
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    if (buttonCollider2D.OverlapPoint(mouseWorldPosition))
                    {
                        _touch = touch;
                        SR_Back.DOFade(1.0f, 0.05f);
                        Text_Instruction.DOKill();
                        Text_Instruction.DOFade(1.0f, 0.05f);
                        _state = 1;
                        TR_X.DOKill();
                        _diffPos = mouseWorldPosition - TR_X.position;
                    }
                }
                else
                if (_state == 1 && touch.phase == TouchPhase.Ended && touch.fingerId == _touch.fingerId)
                {
                    _state = 0;

                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mouseWorldPosition = transform.InverseTransformPoint(mouseWorldPosition);

                    float xValue = mouseWorldPosition.x + _diffPos.x;
                    if (2.48f <= xValue)
                    {
                        DoExit();
                        return;
                    }
                    else
                    {
                        SR_Back.DOKill();
                        SR_Back.DOFade(_defaultAlpha, 0.05f);
                        Text_Instruction.DOKill();
                        Text_Instruction.DOFade(0.0f, 0.05f);
                        TR_X.DOLocalMoveX(0.0f, 0.05f);
                    }
                }
                else
                if (_state == 1 && touch.phase != TouchPhase.Ended && touch.fingerId == _touch.fingerId)
                {
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    mouseWorldPosition = transform.InverseTransformPoint(mouseWorldPosition);

                    Vector3 p = mouseWorldPosition + _diffPos;
                    p.y = TR_X.localPosition.y;
                    p.z = 0.0f;

                    if (2.48f < p.x)
                    {
                        p.x = 2.48f;
                    }
                    else
                    if (p.x < 0.0f)
                    {
                        p.x = 0.0f;
                    }

                    TR_X.localPosition = p;
                }

            }
#endif

        }

        public void DoExit()
        {
            ManagerSounds.PlayEffect("fx_page16");
            _state = 0;
            TR_X.DOKill();
            Text_Instruction.DOKill();
            Text_Instruction.alpha = 0.0f;
            SR_Back.color = new Color(1.0f, 1.0f, 1.0f, _defaultAlpha);
            TR_X.localPosition = Vector3.zero;
            _gameExitEvent?.Invoke();
        }
    }
}