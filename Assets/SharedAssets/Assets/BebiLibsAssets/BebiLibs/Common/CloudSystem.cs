using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSystem : MonoBehaviour
{
    public ItemCloudV[] arrayClouds;

    public Transform TR_Point1;
    public Transform TR_Point2;

    public float speedMinCloud;
    public float speedMaxCloud;

    public int direction = -1;
    public int cloudsState;
    private Vector2 _size;
    public System.Action<float> OnSpeedMultiplier;
    public Color colorClouds;

    public bool auto;

    private void Start()
    {

        if (this.auto)
        {
            this.Init();
            this.Activate();
            this.SetColor(this.colorClouds, false);
        }
    }

    public void ChangeSpeed(float speedMult)
    {
        this.OnSpeedMultiplier?.Invoke(speedMult);
    }
    public void Init()
    {
        for (int i = 0; i < this.arrayClouds.Length; i++)
        {
            this.arrayClouds[i].Init();
            this.OnSpeedMultiplier += this.arrayClouds[i].SpeedMultiplier;
            this.arrayClouds[i].Hide();
        }

        Vector2 size = new Vector2(Screen.width, Screen.height);
        _size = Camera.main.ScreenToWorldPoint(size);

        this.cloudsState = 0;
    }

    public void Activate()
    {
        for (int i = 0; i < this.arrayClouds.Length; i++)
        {
            this.arrayClouds[i].Activate(this.arrayClouds[i].defaultPosition, UnityEngine.Random.Range(this.speedMinCloud, this.speedMaxCloud), this.direction);
        }
        this.cloudsState = 1;
    }

    public void Hide()
    {
        for (int i = 0; i < this.arrayClouds.Length; i++)
        {
            this.arrayClouds[i].Hide();
        }
        this.cloudsState = 0;
    }

    public void SetColor(Color color, bool anim)
    {
        for (int i = 0; i < this.arrayClouds.Length; i++)
        {
            this.arrayClouds[i].SetColor(anim, color);
        }
    }

    public void EnterFrame()
    {
        if (this.cloudsState != 0)
        {
            for (int i = 0; i < this.arrayClouds.Length; i++)
            {
                this.arrayClouds[i].EnterFrame();
                if (this.arrayClouds[i].position.x < this.TR_Point1.localPosition.x)
                {
                    this.arrayClouds[i].Activate((new Vector3(this.TR_Point2.localPosition.x, this.arrayClouds[i].defaultPosition.y)), UnityEngine.Random.Range(this.speedMinCloud, this.speedMaxCloud), this.direction);
                }
            }
        }
    }

    private void Update()
    {
        this.EnterFrame();
    }
}
