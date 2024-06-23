using UnityEngine;
using BebiLibs;

public class AnalyticsInteraction : GenericSingletonClass<AnalyticsInteraction>
{
    public static double LastActiveTimeStamp;
    private static double _IdleTimeInSeconds;
    private static int _IdleTimeCount;

    public static long IdleSeconds => (long)_IdleTimeInSeconds;
    public static int IdleCount => _IdleTimeCount;

    protected override void OnInstanceAwake()
    {
        enabled = false;
    }

    private void AnyClick()
    {
        double sinceActiveSeconds = Time.realtimeSinceStartupAsDouble - LastActiveTimeStamp;

        if (sinceActiveSeconds > 10f)
        {
            _IdleTimeInSeconds += sinceActiveSeconds;
            _IdleTimeCount++;
        }

        LastActiveTimeStamp = Time.realtimeSinceStartupAsDouble;
    }


    public void Update()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                AnyClick();
            }
            else if (Input.GetTouch(i).phase == TouchPhase.Moved)
            {
                LastActiveTimeStamp = Time.realtimeSinceStartupAsDouble;
            }
        }
    }

    public static void Activate()
    {
        LastActiveTimeStamp = Time.realtimeSinceStartupAsDouble;
        _IdleTimeInSeconds = 0;
        _IdleTimeCount = 0;
        Instance.enabled = true;
    }

    public static void Deactivate()
    {
        Instance.enabled = false;
    }
}