using System.Collections;
using System.Collections.Generic;
using BebiLibs;
using UnityEngine;
using UnityEngine.Rendering;
using static BalloonSystem;

public class ItemBalloon : MonoBehaviour
{
    public enum BalloonType { Color, Smile, Star };

    public BalloonType type;

    public string audioClip;

    public ItemParticleSystem PS_Blow;

    public GameObject GO_Content;
    //public SpriteRenderer SR_Body_Stroke;
    public SpriteRenderer SR_Body;
    //public SpriteRenderer SR_Blik;
    public SpriteRenderer SR_Icon;
    public SortingGroup SG;
    public Collider2D colliderBalloon;

    public bool setParticleColor;

    [SerializeField] private ItemSpriteSizer _iconSpriteSizer;
    [SerializeField] private Vector3 _initialPosition;

    private float _speedX;
    private float _speedY;
    private int _state = 0;
    private Vector2 _currentPosition;
    private float _t;
    private float _defaultX;
    private float _defaultY;


    private static float _rotationPower = 10.0f;
    private static float _rotationSpeed = 1.3f;
    public static float _swingPower = 1.0f;
    public static float _swingSpeed = 0.5f;

    public void SetBodyData(Color colorBody)
    {
        this.SR_Body.color = colorBody;
        if (this.setParticleColor)
        {
            this.PS_Blow?.SetColor(colorBody);
        }
    }

    public void SetBalloonIcon(Sprite sp)
    {
        if (this.SR_Icon != null)
        {
            this.SR_Icon.sprite = sp;
        }
    }

    public void SetBalloonIconWithSizer(Sprite sprite)
    {
        if (_iconSpriteSizer == null || SR_Icon == null)
            return;

        SR_Icon.gameObject.SetActive(false);

        _iconSpriteSizer.SetSprite(sprite);
        _iconSpriteSizer.gameObject.SetActive(true);
    }

    public void SetBalloonPopSounds(string audioClip)
    {
        this.audioClip = audioClip;
    }

    public void Activate(Vector2 p, float speed, Direction direction)
    {
        this.colliderBalloon.enabled = true;
        _initialPosition = transform.position;
        
        _currentPosition = p;
        _t = UnityEngine.Random.Range(0.0f, 2.0f);
        _defaultX = _currentPosition.x;
        _defaultY = _currentPosition.y;
        _currentPosition.x = _defaultX + Mathf.Sin(_swingSpeed * _t) * _swingPower;
        this.transform.localPosition = _currentPosition;
        this.transform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Sin(_t * _rotationSpeed) * _rotationPower);

        this.gameObject.SetActive(true);
        this.GO_Content.SetActive(true);
        this.PS_Blow?.Hide();

        this.transform.localScale = Vector3.one * UnityEngine.Random.Range(0.92f, 1.0f);

        _speedX = speed;
        _speedY = speed;

        _state = 1;
    }

    public void Hide()
    {
        this.colliderBalloon.enabled = false;
        this.gameObject.SetActive(false);
        _state = 0;
    }

    public void EnterFrame(int direction)
    {
        if (_state == 1)
        {
            _t += Time.deltaTime * _swingSpeed;
            if (direction == 1)
            {
                _currentPosition.x = _defaultX + Mathf.Sin(_swingSpeed * _t) * _swingPower;
                _currentPosition.y += _speedY * Time.deltaTime * BalloonSystem.balloonSpeedScale;
            }
            else
            if (direction == 2)
            {
                _currentPosition.y = _defaultY + Mathf.Sin(_swingSpeed * _t) * _swingPower;
                _currentPosition.x -= _speedX * Time.deltaTime;
            }

            this.transform.localPosition = _currentPosition;

            this.transform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Sin(_t * _rotationSpeed) * _rotationPower);
        }
        else
        if (_state == 2)
        {
            _t -= Time.deltaTime;
            if (_t <= 0.0f)
            {
                _state = 3;
            }
        }
    }

    public void Blow()
    {
        _state = 2;
        _t = 2.0f;
        this.GO_Content.SetActive(false);

        this.colliderBalloon.enabled = false;
        if (this.PS_Blow != null)
        {
            this.PS_Blow?.Play();
        }

        ManagerSounds.PlayEffect(this.audioClip);
    }

    public void ResetPosition()
    {
        transform.position = _initialPosition;
    }

    public int State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
        }
    }
}
