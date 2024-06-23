using UnityEngine;
namespace BalloonAdventure
{
    [System.Serializable]
    public class DataSpawnPoint
    {
        public Vector2 p1;
        public Vector2 p2;
        public float h;
        public float time;

        public float minSpeedY;
        public float maxSpeedY;

        public static DataSpawnPoint Create(Vector2 p1)
        {
            DataSpawnPoint point = new DataSpawnPoint();
            point.p1 = p1;
            point.time = 0.0f;

            return point;
        }

        public float GetSpeedScale()
        {
            return UnityEngine.Random.Range(this.minSpeedY, this.maxSpeedY);
        }

        public static DataSpawnPoint Create(Vector2 p1, float minSpeedY, float maxSpeedY)
        {
            DataSpawnPoint point = new DataSpawnPoint();
            point.minSpeedY = minSpeedY;
            point.maxSpeedY = maxSpeedY;
            point.p1 = p1;
            point.time = 0.0f;

            return point;
        }

        public static DataSpawnPoint Create(Vector2 p1, Vector2 p2)
        {
            DataSpawnPoint point = new DataSpawnPoint();
            point.p1 = p1;
            point.p2 = p2;
            point.time = 0.0f;

            float x = p2.x - p1.x;
            float y = p2.y - p1.y;

            point.h = (float)Mathf.Sqrt(x * x + y * y);
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
}