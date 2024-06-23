using System.Collections;
using System.Collections.Generic;
using BebiLibs;
using UnityEngine;

public class ItemBalloonStart : MonoBehaviour
{
    public int state = 0;

    public float speedY;
    public float swingPower;
    public float swingSpeed;

    public ParticleSystem PS_Blow;

    public SpriteRenderer SR_Main;
    private Vector2 _v;
    private float _t;
    private float _defaultX;
    private Color _color;

    public void Hide()
    {
        this.gameObject.SetActive(false);
        this.SR_Main.gameObject.SetActive(false);
        this.state = 0;
    }

    private float _valueX;
    private float _valueY;
    private float _speedX;
    private float _speedY;
    private float accelX;
    private float accelY;
    public float gravityX;
    public float gravityY;


    public void Activate(Vector2 startPosition, Vector2 direction, float startAngle, float power)
    {
        this.state = 1;

        _valueX = startPosition.x;
        _valueY = startPosition.y;

        _speedX = direction.x * power;
        _speedY = direction.y * power;
    }



    public void SetData(Sprite sp, Vector2 p, float speed, float delay, float swingStart)
    {
        this.speedY = speed;
        _defaultX = p.x;
        _v = p;
        this.SR_Main.sprite = sp;
        this.gameObject.SetActive(true);
        this.SR_Main.gameObject.SetActive(true);

        this.transform.position = p;

        if (0.0f < delay)
        {
            _t = delay;
            this.state = 3;
        }
        else
        {
            this.state = 1;
            _t = swingStart;
            //Common.DebugLog("_t = " + _t);
        }


        int index = int.Parse(sp.name.Split('_')[1]);
        if (index == 1)
        {
            _color = new Color(31.0f / 255.0f, 184.0f / 255.0f, 182.0f / 255.0f);
        }
        else
        if (index == 2)
        {
            _color = new Color(235.0f / 255.0f, 42.0f / 255.0f, 124.0f / 255.0f);
        }
        else
        if (index == 3)
        {
            _color = new Color(247.0f / 255.0f, 173.0f / 255.0f, 68.0f / 255.0f);
        }
        else
        if (index == 4)
        {
            _color = new Color(239.0f / 255.0f, 94.0f / 255.0f, 65.0f / 255.0f);
        }
        else
        if (index == 5)
        {
            _color = new Color(31.0f / 255.0f, 184.0f / 255.0f, 184.0f / 255.0f);
        }
    }


    public void EnterFrame()
    {
        //return;
        if (this.state == 1)
        {
            _v.y += this.speedY * Time.deltaTime;
            _v.x = _defaultX + Mathf.Sin(this.swingSpeed * _t) * this.swingPower;
            this.transform.position = _v;
            this.transform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Sin(_t * 1.3f) * 10.0f);
            _t += Time.deltaTime * this.swingSpeed;

            if (12.0f < _v.y)
            {
                this.state = 0;
                this.SR_Main.gameObject.SetActive(false);
                return;
            }
        }
        else
        if (this.state == 2)
        {
            _t -= Time.deltaTime;
            if (_t <= 0.0f)
            {
                _t = 0.0f;
                this.PS_Blow.gameObject.SetActive(false);
                this.state = 0;
            }
        }
        else
        if (this.state == 3)
        {
            _t -= Time.deltaTime;
            if (_t <= 0.0f)
            {
                _t = 0.0f;
                this.state = 1;
            }
        }
    }

    public void Blow()
    {
        this.state = 2;
        this.SR_Main.gameObject.SetActive(false);

        _t = 2.0f;

        ParticleSystem.MainModule m = this.PS_Blow.main;
        m.startColor = _color;
        this.PS_Blow.gameObject.SetActive(true);
        this.PS_Blow.Play();

        ManagerSounds.PlayEffect("fx_balloons" + UnityEngine.Random.Range(1, 4));
    }
}





