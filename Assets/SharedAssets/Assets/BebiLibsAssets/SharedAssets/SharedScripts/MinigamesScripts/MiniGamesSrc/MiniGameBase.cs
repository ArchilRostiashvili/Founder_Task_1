using BebiLibs;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameBase : MonoBehaviour
{
    public Camera gameCamera;
    public Action<string> CallBack_OnGameEnd;

    public ItemParticleSystem SC_ParticleTrail;

    protected ItemDrag _itemDragForHelp;

    private Vector2 _touchDown;
    private static MiniGameBase _instance;
    public ChildHelper SC_ChildHelper;
    public static ChildHelper childHelper;

    public static MiniGameBase Instance
    {
        get
        {
            return _instance;
        }
    }

    public virtual void Init()
    {
        _instance = this;
        childHelper = SC_ChildHelper;
    }

    public static void TouchDownProduct(Vector2 point, ItemDrag itemDrag)
    {
        if (_instance == null)
        {
            return;
        }

        _instance._itemDragForHelp = null;
        if (_instance.SC_ParticleTrail != null)
        {
            _instance.SC_ParticleTrail.transform.position = point;
            _instance.SC_ParticleTrail.Play();
        }

        _instance._touchDown = point;

        childHelper.Upgrade();
        childHelper.Playing();
    }

    public static int TouchUpProduct(ItemDrag itemDrag)
    {
        if (_instance == null)
        {
            return -1;
        }

        _instance.TouchClear();
        childHelper.Playing();

        if (_instance.SC_ParticleTrail != null)
        {
            _instance.SC_ParticleTrail.Stop(true);
        }
        _instance._itemDragForHelp = null;
        float diff = (_instance._touchDown - (Vector2)itemDrag.transform.position).magnitude;
        if (Mathf.Abs(diff) < 0.4f)
        {
            _instance._itemDragForHelp = itemDrag;
            childHelper.Trigger_Help();

            return 0;
        }
        else
        {
            return 1;
        }
    }


    public static void TouchUpSimpleProduct()
    {
        if (_instance == null)
        {
            return;
        }

        _instance.TouchClear();

        childHelper.Playing();

        if (_instance.SC_ParticleTrail != null)
        {
            _instance.SC_ParticleTrail.Stop(true);
        }
        _instance._itemDragForHelp = null;
    }

    public static void TouchMoveProduct(Vector2 point, ItemDrag itemDrag = null)
    {
        if (_instance == null)
        {
            return;
        }

        if (_instance.SC_ParticleTrail != null)
        {
            _instance.SC_ParticleTrail.transform.position = point;
        }
        _instance.TouchTry(itemDrag);
    }

    private Collider2D[] _arrayTouchingColliders = new Collider2D[20];
    private ItemPlaceBase _currentTouchingContainer;
    private ContactFilter2D _contactFilter = new ContactFilter2D();

    public bool isDistanceBasedTouch = false;

    private List<ItemPlaceBase> _touchSet = new List<ItemPlaceBase>();
    private Dictionary<ItemDrag, ItemPlaceBase> _touchedItems = new Dictionary<ItemDrag, ItemPlaceBase>();

    public void TouchTry(ItemDrag itemDrag)
    {
        if (itemDrag == null)
        {
            return;
        }

        ItemPlaceBase container = null;
        int colliderCount = itemDrag.colliderProduct.OverlapCollider(_contactFilter, _arrayTouchingColliders);
        Vector3 vTouch = itemDrag.transform.position;
        float magnitude = 1000000000.0f;
        _touchSet.Clear();
        for (int i = 0; i < colliderCount; i++)
        {
            if (_arrayTouchingColliders[i] == null) continue;

            ItemPlaceBase containerTemp = _arrayTouchingColliders[i].gameObject.GetComponent<ItemPlaceBase>();
            if (containerTemp != null)
            {
                if (this.isDistanceBasedTouch)
                {
                    Vector2 diff = vTouch - containerTemp.transform.position;
                    if (Mathf.Abs(diff.magnitude) < magnitude)
                    {
                        magnitude = Mathf.Abs(diff.magnitude);
                        container = containerTemp;
                    }
                }
                else
                {
                    container = containerTemp;
                }

                if (!_touchSet.Contains(container))
                {
                    _touchSet.Add(container);
                }

                if (!_touchedItems.ContainsKey(itemDrag))
                {
                    _touchedItems.Add(itemDrag, container);
                    container.Touching();
                }
                // else if(!_touchedItems[itemDrag].Contains(container))
                // {
                //     _touchedItems[itemDrag].Add(container);
                //     //container.Touching();
                // }
            }
        }



        if (_touchedItems.ContainsKey(itemDrag))
        {
            if (!_touchSet.Contains(_touchedItems[itemDrag]))
            {
                _touchedItems[itemDrag].TouchingOut();
                _touchedItems.Remove(itemDrag);
            }
            // List<ItemPlaceBase> arrayItems = _touchedItems[itemDrag];
            // for(int i = arrayItems.Count - 1; i >= 0; i--)
            // {
            //     if(!_touchSet.Contains(arrayItems[i]))
            //     {
            //         arrayItems[i].TouchingOut();
            //         arrayItems.RemoveAt(i);
            //     }
            // }
            // if(arrayItems.Count == 0)
            // {
            //     _touchedItems.Remove(itemDrag);
            // }
        }


        // for(int i = _touchedItems.Count - 1; i >= 0; i--)
        // {
        //     if(!_touchSet.Contains(_touchedItems[i]))
        //     {
        //         _touchedItems[i].TouchingOut();
        //         _touchedItems.RemoveAt(i);
        //     }
        // }


        // if((count == 1 || this.isDistanceBasedTouch) && _touchPairs.ContainsKey(itemDrag))
        // {
        //     _touchPairs[itemDrag].Touching();
        //     if(_currentTouchingContainer != container && _currentTouchingContainer != null)
        //     {
        //         _currentTouchingContainer.TouchingOut();
        //     }
        //     _currentTouchingContainer = container;
        // }
        // else
        // {
        //     if(_currentTouchingContainer != null)
        //     {
        //         _currentTouchingContainer.TouchingOut();
        //         _currentTouchingContainer = null;
        //     }
        // }
    }

    public void TouchClear()
    {
        // if(_currentTouchingContainer != null)
        // {
        //     _currentTouchingContainer.TouchingOut();
        //     _currentTouchingContainer = null;
        // }


        foreach (var item in _touchedItems)
        {
            item.Value.TouchingOut();
            // for(int i = 0; i < item.Value.Count; i++)
            // {
            //     item.Value[i].TouchingOut();
            // }
        }

        _touchSet.Clear();
        _touchedItems.Clear();

        // for(int i = 0; i < _touchedItems.Count; i++)
        // {
        //     _touchedItems[i].TouchingOut();
        // }
        // _touchedItems.Clear();
        // _touchSet.Clear();
    }

    public void Trigger_ButtonClick_Back()
    {
        this.CallBack_OnGameEnd?.Invoke("Back");
    }

    public void DisableCamera()
    {
        if (gameCamera != null && gameCamera.gameObject != null)
        {
            gameCamera.gameObject.SetActive(false);
        }
    }

    public virtual void RemoveGame()
    {
        _itemDragForHelp = null;
        //Instance = null;
        DOTween.KillAll();
        ManagerTime.Instance.StopAllCoroutines();
        this.StopAllCoroutines();
        this.gameObject.SetActive(false);
        ManagerSounds.UnloadTempAudio();
    }

    private static System.Random _rng;
    public static void Shuffle<T>(ref List<T> list)
    {
        if (_rng == null)
        {
            _rng = new System.Random();
        }

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = _rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void Shuffle<T>(ref T[] array)
    {
        if (_rng == null)
        {
            _rng = new System.Random();
        }

        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = _rng.Next(n + 1);
            T value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }

    public static T RandomElement<T>(T[] list)
    {
        return (T)list[UnityEngine.Random.Range(0, list.Length)];
    }

    public static T RandomElement<T>(List<T> list)
    {
        return (T)list[UnityEngine.Random.Range(0, list.Count)];
    }

    public virtual void TriggerGame()
    {

    }

    public virtual void TouchDownProductDetails(Vector2 point)
    {

    }

    public virtual void TouchUpProductDetails(ItemDrag itemDrag)
    {

    }

    public static List<string> arrayGoodJobSounds = new List<string> { "fx_tx_good_job", "fx_tx_great", "fx_tx_nice", "fx_tx_smart", "fx_tx_well_done", "fx_tx_wow" };
    public static int indexGoodJob = 0;
    public static void PlayGoodJob(string name = null)
    {
        if (name != null)
        {
            ManagerSounds.PlayEffect(name);
            return;
        }
        ManagerSounds.PlayEffect(arrayGoodJobSounds[indexGoodJob]);
        indexGoodJob++;
        if (arrayGoodJobSounds.Count <= indexGoodJob)
        {
            indexGoodJob = 0;
            Shuffle(ref arrayGoodJobSounds);
        }
    }

    public static List<string> arrayGoodJobSoundsSimple = new List<string> { "fx_tx_good_job", "fx_tx_great", "fx_tx_well_done", "fx_tx_wow" };
    public static int indexGoodJobSimple = 0;

    public static void PlayGoodJobSimple()
    {
        ManagerSounds.PlayEffect(arrayGoodJobSoundsSimple[indexGoodJobSimple]);
        indexGoodJobSimple++;
        if (arrayGoodJobSoundsSimple.Count <= indexGoodJobSimple)
        {
            indexGoodJobSimple = 0;
            Shuffle(ref arrayGoodJobSoundsSimple);
        }
    }

    public virtual IEnumerator StartMiniGame()
    {
        yield break;
    }

    public virtual IEnumerator WaitForAssetPreload()
    {
        yield break;
    }
}
