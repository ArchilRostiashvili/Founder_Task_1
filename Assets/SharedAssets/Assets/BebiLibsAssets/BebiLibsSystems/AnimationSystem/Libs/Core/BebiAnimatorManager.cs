using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiAnimations.Libs.Core
{
    public class BebiAnimatorManager : MonoBehaviour
    {
        private static List<BebiAnimator> _animatorsList = new List<BebiAnimator>();
        private static BebiAnimatorManager _manager;

        public static BebiAnimatorManager Create()
        {
            if (_manager == null)
            {
                GameObject go = new GameObject("BebiAnimatorManager");
                _manager = go.AddComponent<BebiAnimatorManager>();
                DontDestroyOnLoad(go);
            }

            return _manager;
        }
        /*
        void Start()
        {
            GameObject robotPrefab = (GameObject)Resources.Load("Robot");
            Instantiate(robotPrefab);
        }
        */
        public static void AddAnimator(BebiAnimator animator)
        {
            Create();
            if (_animatorsList.Find(x => x == animator) == null)
            {
                _animatorsList.Add(animator);
            }
        }

        private void Update()
        {
            //Debug.Log("_animatorsList.Count = " + _animatorsList.Count);
            for (int i = _animatorsList.Count - 1; i >= 0; i--)
            {
                if (_animatorsList[i] == null)
                {
                    _animatorsList.RemoveAt(i);
                }
                else
                {
                    _animatorsList[i].EnterFrame();
                }
            }
        }
    }
}
