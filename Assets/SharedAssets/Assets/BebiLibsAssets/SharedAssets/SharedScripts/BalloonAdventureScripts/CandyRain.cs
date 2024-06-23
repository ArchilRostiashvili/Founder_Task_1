using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BalloonAdventure
{    
    public class CandyRain : MonoBehaviour
    {
        public Sprite[] arraySprites;
    
        public ItemCandy[] arrayItemCandy;
    
        private DataSpawnPoint[] _arraySpawnPoints;
        private int _state = 0;
        private int _freeIndex;
    
        public float candyTime;
        private float _candyTimeValue;
        private bool _init = false;
    
        public DataTimer dataTimer;
        private void Init()
        {
            Vector2 size = new Vector2(Screen.width, Screen.height);
            size = Camera.main.ScreenToWorldPoint(size);
    
            float diff = 1.6f;
            int count = (int)(size.x * 2 / diff) - 1;
    
            _arraySpawnPoints = new DataSpawnPoint[count];
            for (int i = 1; i <= count; i++)
            {
                _arraySpawnPoints[i - 1] = DataSpawnPoint.Create(new Vector2(-size.x + diff * i, 0.0f));
            }
        }
    
        public void Play()
        {
            if (!_init)
            {
                this.Init();
                _init = true;
            }
    
            for (int i = 0; i < _arraySpawnPoints.Length; i++)
            {
                _arraySpawnPoints[i].time = 0.0f;
            }
            this.gameObject.SetActive(true);
            _freeIndex = -1;
            _state = 1;
            _candyTimeValue = this.candyTime;
            this.dataTimer.Reset();
        }
    
        public void Hide()
        {
            this.gameObject.SetActive(false);
            _freeIndex = -1;
            _state = 0;
        }
    
        // Update is called once per frame
        void Update()
        {
            if (_state == 0)
            {
                return;
            }
    
            if (_freeIndex == -1)
            {
                _freeIndex = this.FreeSpawnPlace();
            }
    
            for (int i = 0; i < _arraySpawnPoints.Length; i++)
            {
                _arraySpawnPoints[i].EnterFrame();
            }
    
            ItemCandy itemCandy;
            int count = 0;
            for (int i = 0; i < this.arrayItemCandy.Length; i++)
            {
                itemCandy = this.arrayItemCandy[i];
                itemCandy.EnterFrame();
                if (itemCandy.transform.position.y <= -9.0f)
                {
                    itemCandy.Hide();
                }
    
                if (!itemCandy.active)
                {
                    count++;
                }
            }
    
            this.dataTimer.EnterFrame();
            if (this.dataTimer.Done && _freeIndex != -1 && _state != 2)
            {
                itemCandy = this.GetFreeCandy();
                if (itemCandy != null)
                {
                    itemCandy.Activate(this.arraySprites[UnityEngine.Random.Range(0, this.arraySprites.Length)], new Vector3(_arraySpawnPoints[_freeIndex].p1.x, 8.4f, 0.0f));
                    this.FillSpawnPointTimes(_freeIndex);
                    this.dataTimer.Reset();
                    _freeIndex = -1;
                }
            }
    
            _candyTimeValue -= Time.deltaTime;
            if (_candyTimeValue <= 0.0f)
            {
                if (count == this.arrayItemCandy.Length && _state == 2)
                {
                    this.Hide();
                }
                else
                {
                    _state = 2;
                }
            }
        }
    
        private ItemCandy GetFreeCandy()
        {
            for (int i = 0; i < this.arrayItemCandy.Length; i++)
            {
                if (!this.arrayItemCandy[i].active)
                {
                    return this.arrayItemCandy[i];
                }
            }
            return null;
        }
    
        public int FreeSpawnPlace()
        {
            int index = UnityEngine.Random.Range(0, _arraySpawnPoints.Length);
            if (_arraySpawnPoints[index].time <= 0.0f)
            {
                return index;
            }
            else
            {
                return -1;
            }
        }
    
        //private float[] _arrayTimeFills = { 0.3f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 0.3f };
        private float[] _arrayTimeFills = { 0.2f, 0.5f, 1.0f, 0.5f, 0.2f };
        private void FillSpawnPointTimes(int index)
        {
            int size = _arrayTimeFills.Length / 2;
            int startPoint = index - size;
            int toPoint = index + size;
    
            float time = 0.2f;
            int count = -1;
            for (int i = startPoint; i <= toPoint; i++)
            {
                count++;
                if (0 <= i && i < _arraySpawnPoints.Length)
                {
                    _arraySpawnPoints[i].time = time * _arrayTimeFills[count];
                }
            }
        }
    }
}