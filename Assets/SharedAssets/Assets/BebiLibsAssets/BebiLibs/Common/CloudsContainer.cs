using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CloudsContainer : MonoBehaviour
{

    public enum Direction { Left, Right }

    [SerializeField]
    private Direction _direction;
    [SerializeField]
    private ItemCloud[] _clouds;

    public void Init()
    {
        this.Stop();
        _clouds.MyForeach(cloud =>
        {
            cloud.Init(_direction);
        });
    }

    void Update()
    {
        _clouds.MyForeach(cloud =>
        {
            cloud.Move();
        });
    }

    public void Move()
    {
        this.enabled = true;
    }

    public void Stop()
    {
        this.enabled = false;
    }

}