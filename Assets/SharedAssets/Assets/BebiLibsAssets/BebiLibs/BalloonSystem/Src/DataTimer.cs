using UnityEngine;

[System.Serializable]
public class DataTimer
{
    public string timerID;
    public float TimeMin;
    public float TimeMax;


    public float _timeValue;

    public void Reset(float scale = 1.0f)
    {
        _timeValue = UnityEngine.Random.Range(this.TimeMin * scale, this.TimeMax * scale);
    }
    public void EnterFrame()
    {
        _timeValue -= Time.deltaTime;
    }

    public bool Done
    {
        get
        {
            if (_timeValue <= 0.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
