using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemCloud : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;
    [SerializeField]
    [Range(0.1f, 5)]
    private float _speed = 0.1f;

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

}