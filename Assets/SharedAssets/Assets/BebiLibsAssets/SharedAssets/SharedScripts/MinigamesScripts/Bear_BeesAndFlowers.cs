using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;


public class Bear_BeesAndFlowers : MonoBehaviour
{
    [SerializeField]
    private Transform _shadow;
    [SerializeField]
    private SpriteRenderer _visualForBounds;
    [SerializeField]
    [Range(0.5f, 2)]
    private float _jumpDuration;
    [SerializeField]
    [Range(3f, 5f)]
    private float _jumpPower;
    [SerializeField]
    [Range(3, 7)]
    private float _lookInScreenDuration;
    [SerializeField]
    [Range(25, 90)]
    private float _rotationValue;
    [SerializeField]
    private float _minTimer;
    [SerializeField]
    private float _maxTimer;

    private readonly Vector3 _finalScale = Vector3.one * 0.54f;
    private Vector3 _jumpPosition;
    private Vector3 _jumpPositionLeft;
    [ReadOnly]
    [SerializeField]
    private float _timer;
    [ReadOnly]
    [SerializeField]
    private bool _hasJumped;
    private bool _isInScreen;

    private Vector3 RotationAngle => Vector3.forward * _rotationValue;
    private string JumpSfx => GlobalSfx.BearJump;
    private string LookInScreenSfx => GlobalSfx.BearLook;

    public bool HasJumped => _hasJumped;

    public void Init()
    {
        this.gameObject.Enable();

        Bounds screenBounds = Utils.ScreenBounds2D;
        Bounds safeScreenBounds = Utils.ScreenBounds2DSafe;
        Bounds bearBounds = _visualForBounds.bounds;

        _jumpPosition = new Vector3(screenBounds.extents.x + bearBounds.extents.x + 0.4f, this.transform.position.y + 2.43f);
        _jumpPositionLeft = new Vector3(safeScreenBounds.extents.x + bearBounds.extents.x + 0.4f, this.transform.position.y + 2.43f);

        _isInScreen = true;
        this.UpdateTimer();
    }

    void Update()
    {
        this.ManageTiming();
    }

    private void ManageTiming()
    {
        if (!_isInScreen)
            _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            this.UpdateTimer();
            this.LookIntoScreen();
        }
    }

    public void JumpFromScreen()
    {
        _shadow.gameObject.Disable();

        this.Jump(_jumpPosition, _jumpPower, 2, _jumpDuration)
        //.InsertCallback(0, () => ManagerSounds.PlayEffect(this.JumpSfx))
        .Join(this.transform.DOScale(_finalScale, _jumpDuration))
        .OnComplete(() =>
        {
            _hasJumped = true;
            _isInScreen = false;
            this.transform.position = _jumpPosition;
        });

    }

    public int side = 0;

    public void Activate()
    {
        _shadow.gameObject.Disable();
        _hasJumped = true;
        _isInScreen = false;
        this.transform.position = _jumpPosition;
    }

    private void LookIntoScreen()
    {
        _isInScreen = true;
        float duration = 0.6f;
        int dir = Utils.GetRandomBoolean() ? 1 : -1;
        if (this.side != 0)
        {
            dir = this.side;
        }

        _rotationValue *= dir;

        Vector3 position = dir > 0 ? _jumpPosition : -_jumpPositionLeft;
        this.transform.position = new Vector3(position.x, this.transform.position.y, this.transform.position.z);

        DOTween.Sequence()
        .Append(this.transform.DORotate(this.RotationAngle, duration).SetEase(Ease.InSine))
        .AppendInterval(duration * 0.01f)
        .AppendCallback(() => ManagerSounds.PlayEffect(this.LookInScreenSfx))
        .AppendInterval(_lookInScreenDuration)
        .Append(this.transform.DORotateQuaternion(Quaternion.identity, duration * 0.8f))
        .OnComplete(() => _isInScreen = false);
    }

    private void UpdateTimer()
    {
        _timer = Random.Range(_minTimer, _maxTimer);
    }

    private Sequence Jump(Vector3 endValue, float jumpPower, int numJumps, float duration)
    {
        Sequence s = DOTween.Sequence();

        bool snapping = false;
        float startPosY = 0;

        this.StartCoroutine(routine());

        Tween yTween = DOTween.To(() => transform.position, y => transform.position = y, new Vector3(0, jumpPower, 0), duration / (numJumps * 2))
        .SetOptions(AxisConstraint.Y, snapping).SetEase(Ease.OutQuad).SetRelative()
        .SetLoops(numJumps * 2, LoopType.Yoyo)
        .OnStart(() => startPosY = transform.position.y);


        s.Append(DOTween.To(() => transform.position, x => transform.position = x, new Vector3(endValue.x, 0, 0), duration)
                .SetOptions(AxisConstraint.X, snapping).SetEase(Ease.Linear)
            ).Join(yTween)
            .SetTarget(transform).SetEase(DOTween.defaultEaseType);

        return s;

        IEnumerator routine()
        {
            yield return new WaitForSeconds(duration / (numJumps * 2));
            ManagerSounds.PlayEffect(this.JumpSfx);
        }

    }

}