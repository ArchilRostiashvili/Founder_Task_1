using IndieStudio.EnglishTracingBook.Utility;
using System.Collections.Generic;
using UnityEngine;

/*
 * English Tracing Book Package
 *
 * @license		    Unity Asset Store EULA https://unity3d.com/legal/as_terms
 * @author		    Indie Studio - Baraa Nasser
 * @Website		    https://indiestd.com
 * @Asset Store     https://assetstore.unity.com/publishers/9268
 * @Unity Connect   https://connect.unity.com/u/5822191d090915001dbaf653/column
 * @email		    info@indiestd.com
 *
 */

namespace IndieStudio.EnglishTracingBook.Game
{
    [DisallowMultipleComponent]
    public class ShapesManager : MonoBehaviour
    {
        /// <summary>
        /// The shapes list.
        /// </summary>
        public List<Shape> shapes = new List<Shape>();

        /// <summary>
        /// The collected stars in the shapes.
        /// </summary>
        private int collectedStars;

        /// <summary>
        /// The shape label (example Letter or Number).
        /// </summary>
        public string shapeLabel = "Shape";

        /// <summary>
        /// The shape prefix used for DataManager only (example Lowercase or Uppercase or Number).
        /// </summary>
        public string shapePrefix = "Shape";

        /// <summary>
        /// The name of the scene.
        /// </summary>
        public string sceneName = "Album";

        /// <summary>
        /// The title of the album scene.
        /// </summary>
        public Sprite albumTitle;

        /// <summary>
        /// The shape scale factor in the preview/album scene.
        /// </summary>
        public float albumShapeScaleFactor = 0.7f;

        /// <summary>
        /// Line width factor for shapes/compound shapes in
        /// the Album/Game scenes
        /// </summary>
        public float AlbumShapeLineWidth = 1.03f;
        public float GameShapeLineWidth = 1.31f;
        public float AlbumCompoundShapeLineWidth = 0.28f;
        public float GameCompoundShapeLineWidth = 0.28f;

        /// <summary>
        /// The last selected group.
        /// </summary>
        [HideInInspector]
        public int lastSelectedGroup;

        /// <summary>
        /// The tracing mode [ Line or Fill ]
        /// </summary>
        public TracingMode tracingMode;

        /// <summary>
        /// Wehther to enable the test mode or not
        /// All shapes will be unlocked for testing
        /// Set this flag to false on production
        /// </summary>
        public bool testMode = false;

        /// <summary>
        /// Whether to enable shape boundary/bounds limit or not.
        /// (If enabled, then leaveing shape's boundary/bounds will be wrong)
        /// Shape's Collider gameobject used to define the boundary
        /// </summary>
        public bool enableTracingLimit = true;

        /// <summary>
        /// The name of the shapes manager.
        /// </summary>
        public static string shapesManagerReference = "";

        /// <summary>
        /// The list of the shapes managers in the scene.
        /// </summary>
        public static Dictionary<string, ShapesManager> shapesManagers = new Dictionary<string, ShapesManager>();

        void Awake()
        {
            if (shapesManagers.ContainsKey(this.gameObject.name))
            {
                Debug.Log("Destroy " + this.gameObject.name);
                Destroy(this.gameObject);
            }
            else
            {
                //init values
                shapesManagers.Add(this.gameObject.name, this);

                this.RemoveEmptyShapes();

                for (int i = 0; i < this.shapes.Count; i++)
                {
                    this.shapes[i].ID = i;
                    this.shapes[i].isLocked = DataManager.IsShapeLocked(this.shapes[i].ID, this);

                    if (this.shapes[i].ID == 0)
                    {
                        this.shapes[i].isLocked = false;
                    }

                    if (this.shapes[i].prefab != null)
                    {
                        if (this.shapes[i].prefab.GetComponent<IndieStudio.EnglishTracingBook.Game.Shape>() != null)
                            this.shapes[i].prefab.GetComponent<IndieStudio.EnglishTracingBook.Game.Shape>().ID = this.shapes[i].ID;
                    }
                }

                //DontDestroyOnLoad(this.gameObject);
                this.lastSelectedGroup = 0;
            }
        }

        private void OnDestroy()
        {
            if (shapesManagers.ContainsKey(this.gameObject.name))
            {
                shapesManagers.Remove(this.gameObject.name);
            }

            shapesManagerReference = string.Empty;
        }

        void Start()
        {
            this.SetUpTracingMode();
        }

        private void SetUpTracingMode()
        {
            //Load value from PlayerPrefs
            if (DataManager.GetTracingModeValue() == 0)
            {
                this.tracingMode = TracingMode.FILL;
            }
            else
            {
                this.tracingMode = TracingMode.LINE;
            }
        }



        /// <summary>
        /// Get the stars rate of the shapes.
        /// </summary>
        /// <returns>The stars rate from 3.</returns>
        public int GetStarsRate()
        {
            return Mathf.FloorToInt(this.collectedStars / (this.shapes.Count * 3.0f) * 3.0f);
        }

        /// <summary>
        /// Get the collected stars.
        /// </summary>
        /// <returns>The collected stars.</returns>
        public int GetCollectedStars()
        {
            return this.collectedStars;
        }

        /// <summary>
        /// Set the collected stars.
        /// </summary>
        /// <param name="collectedStars">Collected stars.</param>
        public void SetCollectedStars(int collectedStars)
        {
            this.collectedStars = collectedStars;
        }

        /// <summary>
        /// Get the current shape.
        /// </summary>
        /// <returns>The current shape.</returns>
        public Shape GetCurrentShape()
        {
            return this.shapes[Shape.selectedShapeID];
        }

        public Shape GetNextShape()
        {
            if (Shape.selectedShapeID + 1 < this.shapes.Count)
            {
                return this.shapes[Shape.selectedShapeID + 1];
            }
            else
            {
                return this.shapes[0];
            }
        }

        /// <summary>
        /// Get the current/ selected shapes manager by user
        /// </summary>
        /// <returns></returns>
        public static ShapesManager GetCurrentShapesManager()
        {
            var shapeManager = ShapesManager.shapesManagers[ShapesManager.shapesManagerReference];
            if (shapeManager == null) Debug.LogError("Tracing shape manager is null with id " + ShapesManager.shapesManagerReference);
            return shapeManager;
        }

        [System.Serializable]
        public class Shape
        {
            /// <summary>
            /// Whether to show the contents/shapes (used for Editor).
            /// </summary>
            public bool showContents = true;

            /// <summary>
            /// The prefab of the shape used in Album/Game scenes.
            /// </summary>
            public GameObject prefab;

            /// <summary>
            /// The picture of the shape.
            /// </summary>
            public Sprite picture;

            /// <summary>
            /// The audio clip of the shape , used for spelling.
            /// </summary>
            public AudioClip clip;

            /// <summary>
            /// The stars time period.
            /// 0 - 14 seconds : 3 stars , 15 - 29 : 2 stars , otherwisee 1 star
            /// </summary>
            public int starsTimePeriod = 15;


            /// <summary>
            /// The ID of the shape.
            /// </summary>
            [HideInInspector]
            public int ID = -1;

            /// <summary>
            /// Whether the shape is locked or not.
            /// </summary>
            [HideInInspector]
            public bool isLocked = true;

            /// <summary>
            /// The ID selected/current shape.
            /// </summary>
            public static int selectedShapeID;

            public void Reset()
            {
                if (this.ID == 0)
                {
                    this.isLocked = false;
                }
                else
                {
                    this.isLocked = true;
                }
            }
        }

        public void RemoveEmptyShapes()
        {
            for (int i = 0; i < this.shapes.Count; i++)
            {
                if (this.shapes[i].prefab == null)
                {
                    this.shapes.RemoveAt(i);
                }
            }
        }

        public enum TracingMode
        {
            FILL,
            LINE
        }
    }
}