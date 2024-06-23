using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.RocketAdventure
{
    [CreateAssetMenu(fileName = "Patterns", menuName = "Modules/MiniGames/Patterns", order = 0)]
    public class ElementData : ScriptableObject
    {
        public string AssetName;
        public List<Pattern> arrayPatterns = new List<Pattern>();
    }

    [System.Serializable]
    public class Pattern
    {
        public Vector2 createPosition;
        public Vector2 CreateLocalScale;
        public List<Shape> arrayShapes = new List<Shape>();
        [Tooltip("Heigher is Harder")]
        [Range(0, 10)] public float Difficulty = 1;
    }


    [System.Serializable]
    public class Shape
    {
        public List<Vector2> arrayPoints = new List<Vector2>();
        [Range(0.7f, 1.8f)]
        public float defaultSize;
        public Shape()
        {
            this.arrayPoints = new List<Vector2>(3);
        }
    }
}

