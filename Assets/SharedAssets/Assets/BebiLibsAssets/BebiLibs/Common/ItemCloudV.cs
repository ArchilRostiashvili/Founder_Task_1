using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCloudV : MonoBehaviour
{
    public Transform TR;

    public float speed;
    private float _defSpeed;
    public int direction;
    public Vector3 position;
    public bool active;

    public Vector3 defaultPosition;
    [SerializeField]
    private bool _ShouldRescale =true;
    public void Init()
    {
        this.defaultPosition = this.transform.localPosition;
    }

    public void SpeedMultiplier(float mult)
    {
        if(mult == 1)
            this.speed = _defSpeed;
        else
            this.speed*=mult;
    }

    [SerializeField]
    private SpriteRenderer _renderer;
    [SerializeField]
    [Range(0.1f, 5)]
    private float _speed;

    private CloudsContainer.Direction _direction;
    private Vector3 _rightBoundPos;
    private Vector3 _leftBoundPos;

    private Vector3 Dir => _direction == CloudsContainer.Direction.Left ? Vector3.left : Vector3.right;

    public void Init(CloudsContainer.Direction dir)
    {
        _direction = dir;
        _rightBoundPos = new Vector3(Utils.ScreenBounds2D.extents.x + _renderer.bounds.size.x / 2, this.transform.position.y);
        _leftBoundPos = new Vector3(-Utils.ScreenBounds2D.extents.x - _renderer.bounds.size.x / 2, this.transform.position.y);
    }

    public void Move()
    {
        this.transform.position += this.Dir * Time.deltaTime * _speed;

        if (transform.position.x < _leftBoundPos.x)
        {
            this.ResetPosition();
        }
    }

    private void ResetPosition()
    {
        this.transform.position = _rightBoundPos;
    }

    public void Activate(Vector3 p, float speed, int direction)
    {
        this.position = p;
        this.transform.localPosition = this.position;

        this.gameObject.SetActive(true);
        _defSpeed = speed;
        this.direction = direction;
        this.active = true;
        if(_ShouldRescale)
        {
            this.speed = speed;
            this.TR.localScale = Vector3.one * UnityEngine.Random.Range(1.0f, 1.3f);
        }
        else if(this.speed  == 0)
        {
            this.speed = speed;
        }
    }

    public void SetColor(bool anim, Color color)
    {
        if (anim)
        {
            this.TR.GetComponent<SpriteRenderer>().DOKill();
            this.TR.GetComponent<SpriteRenderer>().DOColor(color, 0.8f);
        }
        else
        {
            this.TR.GetComponent<SpriteRenderer>().DOKill();
            this.TR.GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void SetSprite(Sprite sp)
    {
        this.TR.GetComponent<SpriteRenderer>().sprite = sp;
    }

    public void Hide()
    {
        this.active = false;
        this.gameObject.SetActive(false);
    }

    public void EnterFrame()
    {
        if (this.active)
        {
            this.position.x += this.direction * this.speed * Time.deltaTime;
            this.transform.localPosition = this.position;
        }
    }
}
