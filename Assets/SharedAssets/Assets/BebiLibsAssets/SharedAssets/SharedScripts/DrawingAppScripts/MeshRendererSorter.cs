using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
namespace DrawingApp
{    
    #if UNITY_EDITOR
    using UnityEditor;
    #endif
    
    public class MeshRendererSorter : MonoBehaviour
    {
        public MeshRenderer meshRenderer;
        [HideInInspector]
        public int selectedIndex;
        [HideInInspector]
        public int sortingOrder;
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(MeshRendererSorter))]
    public class MeshRendererSorterEditor : Editor
    {
        private MeshRendererSorter _meshRendererSorter;
        private SortingLayer[] layers;
        private string[] options;
    
        private void OnEnable()
        {
            layers = SortingLayer.layers;
            options = new string[layers.Length];
    
            for (int i = 0; i < layers.Length; i++)
            {
                options[i] = layers[i].name;
            }
    
            _meshRendererSorter = (MeshRendererSorter)target;
    
            if (_meshRendererSorter.meshRenderer == null)
            {
                MeshRenderer newRenderer = _meshRendererSorter.GetComponent<MeshRenderer>();
                if (newRenderer != null)
                {
                    _meshRendererSorter.meshRenderer = newRenderer;
                }
            }
    
        }
    
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
    
            EditorGUI.BeginChangeCheck();
            _meshRendererSorter.selectedIndex = EditorGUILayout.Popup("Sorting Layer", _meshRendererSorter.selectedIndex, options);
            if (EditorGUI.EndChangeCheck())
            {
                if (_meshRendererSorter.meshRenderer != null)
                {
                    _meshRendererSorter.meshRenderer.sortingLayerName = options[_meshRendererSorter.selectedIndex];
                }
            }
    
            EditorGUI.BeginChangeCheck();
            _meshRendererSorter.sortingOrder = EditorGUILayout.IntField("Sorting Order", _meshRendererSorter.sortingOrder);
            if (EditorGUI.EndChangeCheck())
            {
                if (_meshRendererSorter.meshRenderer != null)
                {
                    _meshRendererSorter.meshRenderer.sortingOrder = _meshRendererSorter.sortingOrder;
                }
            }
    
            if (_meshRendererSorter.meshRenderer == null)
            {
                EditorGUILayout.HelpBox("MeshRenderer is not beeing assigned", MessageType.Warning);
            }
    
        }
    }
    #endif
}