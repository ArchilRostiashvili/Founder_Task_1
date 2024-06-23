using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSun_BeesAndFlowers : MonoBehaviour
{
    private const string _idleName = "idle";
    private const string _zoomingName = "zoom";
    private const string _laughName = "laugh";
    private const string _satisfyName = "satisfy";

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private float _minTimer;
    [SerializeField]
    private float _maxTimer;

    //[SerializeField]
    private float _counter;
    //[SerializeField]
    private float _timer;

    public void Init()
    {
        this.GoZooming();
        this.UpdateTimer();
    }

    void Update()
    {
        this.ManageFunnyAnimations();
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    this.Laugh();
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    this.Satisfy();
        //}
    }

    private void ManageFunnyAnimations()
    {
        _counter += Time.deltaTime;

        if(_counter >= _timer)
        {
            this.RunFunnyAnimation();
            this.UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        _counter = 0;
        _timer = Random.Range(_minTimer, _maxTimer);
    }

    private void RunFunnyAnimation()
    {
        if(Utils.GetRandomBoolean())
            this.Satisfy();
        else
            this.Laugh();

    }

    private void GoIdle()
    {
        _animator.SetTrigger(_idleName);
    }

    private void GoZooming()
    {
        _animator.SetTrigger(_zoomingName);
    }

    private void Laugh()
    {
        _animator.SetTrigger(_laughName);
    }

    public void Satisfy()
    {
        _animator.SetTrigger(_satisfyName);
    }

}