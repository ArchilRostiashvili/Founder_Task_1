using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    public class SharedSceneResources : MonoBehaviour
    {
        private static SharedSceneResources _instance;

        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        private EventSystem _evenetSystem;
        [SerializeField]
        private Physics2DRaycaster _physics2DRaycaster;

        [SerializeField] private Canvas[] _arrayCanvases;

        private static Physics2DRaycaster _Physics2DRaycaster;
        private static Camera _MainCamera;
        private static EventSystem _EventSystem;

        public static void SetPhysicsRaycasterActive(bool value)
        {
            if (_Physics2DRaycaster != null)
            {
                _Physics2DRaycaster.enabled = value;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _MainCamera = _mainCamera;
                _EventSystem = _evenetSystem;
                _Physics2DRaycaster = _physics2DRaycaster;
                UpdateCanvases(_MainCamera);
                _instance = this;
            }
            else
            {
                UpdateCanvases(_MainCamera);
                _mainCamera.gameObject.SetActive(false);
                _evenetSystem.gameObject.SetActive(false);
                if (_physics2DRaycaster != null) _physics2DRaycaster.enabled = false;
                GameObject.Destroy(this);
            }
        }

        private void OnDestroy()
        {
            //physics2DRaycaster = _instance._physics2DRaycaster;
            //SetMainComponentsActive(true, _instance._mainCamera);
        }

        private void UpdateCanvases(Camera activeCamera)
        {
            for (int i = 0; i < _arrayCanvases.Length; i++)
            {
                if (_arrayCanvases[i] != null)
                {
                    _arrayCanvases[i].worldCamera = activeCamera;
                }
            }
        }
    }
}