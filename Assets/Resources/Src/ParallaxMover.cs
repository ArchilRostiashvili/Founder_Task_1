using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxMover : MonoBehaviour
{
    [SerializeField] Transform _pointA;
    [SerializeField] Transform _pointB;
    [SerializeField] float _movementSpeedMin;
    [SerializeField] float _movementSpeedMax;

    private float _movementSpeed;
    private float _lastScrollX;

    private void Start()
    {
        _movementSpeed = Random.Range(_movementSpeedMin, _movementSpeedMax);
    }

    private void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x - _movementSpeed * Time.deltaTime, transform.localPosition.y, transform.localPosition.z);
        if (transform.localPosition.x < _pointB.localPosition.x)
        {
            transform.localPosition = new Vector3(_pointA.localPosition.x, transform.localPosition.y, _pointA.localPosition.z);
        }

        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -9999f, _pointA.localPosition.x), transform.localPosition.y, transform.localPosition.z);
    }
}
