using BebiLibs;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BalloonSystem : MonoBehaviour
{
    public enum Direction { UP, DOWN, LEFT, RIGHT };

    public static float balloonSpeedScale = 1.0f;

    [HideInInspector]
    public int poppedBalloonCount;

    public float speedMin;
    public float speedMax;

    public DataTimer spawnTimer;
    public SpawnPoint[] arraySpawnPoints;

    public string[] arraySoundsSimple;
    public string soundStar;
    public Color[] arrayColors;

    public List<ItemBalloon> arrayBalloonLibs;
    
    public bool UseFixedNumberForSpawnsCount;
    
    public int SpawnCount;

    private List<ItemBalloon> _arrayBalloonLive;
    private int _freeIndex;
    private int _state = 0;
    private Vector2 _wp;
    private Bounds _bounds;
    private bool _init = false;

    private void Start()
    {
        //Init();
        //Activate();
    }

    public void Init()
    {
        gameObject.SetActive(true);
        Vector2 ScreenSize = ScreenUtils.GetScreenSize(Camera.main);
        ScreenSize = Camera.main.ScreenToWorldPoint(ScreenSize);
        ScreenSize.x -= Camera.main.transform.position.x;

        float diff = 1.6f;
        int count = (int)(ScreenSize.x * 2 / diff) - 1;

        arraySpawnPoints = new SpawnPoint[count];
        for (int i = 1; i <= count; i++)
        {
            arraySpawnPoints[i - 1] = SpawnPoint.Create(-ScreenSize.x + diff * i);
        }

        _bounds = ScreenUtils.ScreenBounds2D;
        _init = true;
    }

    public void Activate()
    {
        Init();

        poppedBalloonCount = 0;
        _freeIndex = -1;

        gameObject.SetActive(true);

        _arrayBalloonLive = new List<ItemBalloon>();
        _state = 1;

        CreateBunch();
    }

    protected void Update()
    {
        if (!_init)
        {
            return;
        }

        if (_state == 0)
        {
            return;
        }

        ItemBalloon itemBallon = null;
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            _wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            itemBallon = HitPoint(_wp);
        }
#else
        Touch touch;
        for (int i = 0; i < Input.touchCount; i++)
        {
            touch = Input.GetTouch(i);
            _wp = Camera.main.ScreenToWorldPoint(touch.position);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                itemBallon = HitPoint(_wp);
            }
        }
#endif

        for (int i = _arrayBalloonLive.Count - 1; i >= 0; i--)
        {
            itemBallon = _arrayBalloonLive[i];
            itemBallon.EnterFrame(1);
            if (itemBallon.transform.localPosition.y > _bounds.max.x + 2f)
            {
                _arrayBalloonLive.RemoveAt(i);
                BalloonBack(itemBallon, 1);
            }
            else
            if (itemBallon.State == 3)
            {
                _arrayBalloonLive.RemoveAt(i);
                BalloonBack(itemBallon, 2);
            }
        }

        if (_state != 1)
        {
            return;
        }

        if (_freeIndex == -1)
        {
            _freeIndex = FreeSpawnPlace();
        }


        spawnTimer.EnterFrame();

        for (int i = 0; i < arraySpawnPoints.Length; i++)
        {
            arraySpawnPoints[i].EnterFrame();
        }

        if (spawnTimer.Done && _freeIndex != -1)
        {
            ItemBalloon itemBalloon = GetRandomBalloon();
            if (itemBalloon == null)
            {
                return;
            }

            if (itemBalloon.type == ItemBalloon.BalloonType.Color)
            {
                int indexColor = UnityEngine.Random.Range(0, arrayColors.Length);
                itemBalloon.SetBodyData(arrayColors[indexColor]);

                string audio = arraySoundsSimple[UnityEngine.Random.Range(0, arraySoundsSimple.Length)];
                itemBalloon.SetBalloonPopSounds(audio);
            }
            else
            if (itemBalloon.type == ItemBalloon.BalloonType.Smile)
            {
                int indexColor = UnityEngine.Random.Range(0, arrayColors.Length);
                itemBalloon.SetBodyData(arrayColors[indexColor]);

                string audio = arraySoundsSimple[UnityEngine.Random.Range(0, arraySoundsSimple.Length)];
                itemBalloon.SetBalloonPopSounds(audio);
            }
            else
            if (itemBalloon.type == ItemBalloon.BalloonType.Star)
            {
                string audio = arraySoundsSimple[UnityEngine.Random.Range(0, arraySoundsSimple.Length)];
                itemBalloon.SetBalloonPopSounds(audio);
            }


            float speed = UnityEngine.Random.Range(speedMin, speedMax);

            itemBalloon.Activate(new Vector2(arraySpawnPoints[_freeIndex].p, _bounds.min.y - 2f), speed, Direction.UP);
            _arrayBalloonLive.Add(itemBalloon);

            spawnTimer.Reset(1.0f);
            FillSpawnPointTimes(_freeIndex);
            _freeIndex = -1;
        }
    }

    private void CreateBunch()
    {
        for (int i = 0; i < arraySpawnPoints.Length; i++)
        {
            if (i % 2 == 0)
            {
                continue;
            }

            ItemBalloon itemBalloon = GetRandomBalloon();
            if (itemBalloon == null)
            {
                return;
            }

            if (itemBalloon.type == ItemBalloon.BalloonType.Color)
            {
                int indexColor = UnityEngine.Random.Range(0, arrayColors.Length);
                itemBalloon.SetBodyData(arrayColors[indexColor]);

                string audio = arraySoundsSimple[UnityEngine.Random.Range(0, arraySoundsSimple.Length)];
                itemBalloon.SetBalloonPopSounds(audio);
            }
            else
            if (itemBalloon.type == ItemBalloon.BalloonType.Smile)
            {
                int indexColor = UnityEngine.Random.Range(0, arrayColors.Length);
                itemBalloon.SetBodyData(arrayColors[indexColor]);

                string audio = arraySoundsSimple[UnityEngine.Random.Range(0, arraySoundsSimple.Length)];
                itemBalloon.SetBalloonPopSounds(audio);
            }
            else
            if (itemBalloon.type == ItemBalloon.BalloonType.Star)
            {
                string audio = arraySoundsSimple[UnityEngine.Random.Range(0, arraySoundsSimple.Length)];
                itemBalloon.SetBalloonPopSounds(audio);
            }

            float speed = UnityEngine.Random.Range(speedMin, speedMax) * 1.4f;

            itemBalloon.Activate(new Vector2(arraySpawnPoints[i].p, _bounds.min.y - 2f), speed, Direction.UP);
            _arrayBalloonLive.Add(itemBalloon);

            //FillSpawnPointTimes(i);
        }
        spawnTimer.Reset(1.0f);
    }

    private ItemBalloon GetRandomBalloon()
    {
        if (0 < arrayBalloonLibs.Count)
        {
            int index = UnityEngine.Random.Range(0, arrayBalloonLibs.Count);
            ItemBalloon itemBalloon = arrayBalloonLibs[index];
            arrayBalloonLibs.RemoveAt(index);
            return itemBalloon;
        }
        else
        {
            return null;
        }
    }

    private void BalloonBack(ItemBalloon itemBallon, int type)
    {
        itemBallon.Hide();
        arrayBalloonLibs.Add(itemBallon);
    }

    public void ResetAllBalloonPositions()
    {
        for (int i = 0; i < arrayBalloonLibs.Count; i++)
        {
            arrayBalloonLibs[i].ResetPosition();
        }
    }

    //private float[] _arrayTimeFills = { 0.3f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 0.3f };
    private float[] _arrayTimeFills = { 0.2f, 0.5f, 1.0f, 0.5f, 0.2f };
    private void FillSpawnPointTimes(int index)
    {

        int size = _arrayTimeFills.Length / 2;
        int startPoint = index - size;
        int toPoint = index + size;

        float time = 0.8f;

        LineSolver solver = new LineSolver(0.277777777777778f, -1.33333333333333f);
        time = solver.Solve(Camera.main.orthographicSize, 0.8f, 1.2f);

        int count = -1;
        for (int i = startPoint; i <= toPoint; i++)
        {
            count++;
            if (0 <= i && i < arraySpawnPoints.Length)
            {
                arraySpawnPoints[i].time = time * _arrayTimeFills[count];
            }
        }
    }

    private int FreeSpawnPlace()
    {
        int index = UnityEngine.Random.Range(0, arraySpawnPoints.Length);
        if (arraySpawnPoints[index].time <= 0.0f)
        {
            return index;
        }
        else
        {
            return -1;
        }
    }

    private ItemBalloon HitPoint(Vector2 p)
    {
        Collider2D[] array = Physics2D.OverlapPointAll(p);
        ItemBalloon itemBallon = null;
        for (int i = 0; i < array.Length; i++)
        {
            itemBallon = array[i].GetComponent<ItemBalloon>();
            if (itemBallon != null && itemBallon.State == 1)
            {
                poppedBalloonCount++;
                itemBallon.Blow();
                return itemBallon;
            }
        }
        return null;
    }

    public void DisableSpawn()
    {
        _state = 99;
    }

    public void Hide(int animType)
    {
        _state = 0;
        gameObject.SetActive(false);
        if (_arrayBalloonLive != null)
        {
            for (int i = _arrayBalloonLive.Count - 1; i >= 0; i--)
            {
                BalloonBack(_arrayBalloonLive[i], 0);
            }
        }
    }

}
