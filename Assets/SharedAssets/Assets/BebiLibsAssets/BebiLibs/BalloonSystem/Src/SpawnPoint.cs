using UnityEngine;

[System.Serializable]
public class SpawnPoint
{
    public float p;
    public float time;

    public static SpawnPoint Create(float p)
    {
        SpawnPoint point = new SpawnPoint();
        point.p = p;
        point.time = 0.0f;

        return point;
    }

    public void EnterFrame()
    {
        if (0.0f < this.time)
        {
            this.time -= Time.deltaTime;
        }
    }
}
