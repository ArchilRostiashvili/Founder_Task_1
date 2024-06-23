using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using IndieStudio.EnglishTracingBook.Utility;

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
    [ExecuteInEditMode]
    public class Curve : MonoBehaviour
    {
        /// <summary>
        /// The points list of the curve.
        /// </summary>
        public List<Transform> points = new List<Transform>();

        /// <summary>
        /// The bezier curve points.
        /// </summary>
        private List<Vector3> bezierPoints = new List<Vector3>();

        /// <summary>
        /// The smoothness of the curve
        /// </summary>
        [Range(0, 0.5f)]
        public float smoothness = 0.3f;

        /// <summary>
        /// The radius of the collide of the point in the curve
        /// </summary>
        [Range(0, 10)]
        public float pointColliderRadius = 1;

        /// <summary>
        /// The center point of polygon
        /// </summary>
        [HideInInspector]
        public Vector3 centroid;

        /// <summary>
        /// The bezier instance.
        /// </summary>
        private static readonly Bezier bezier = new Bezier();

        /// <summary>
        /// Whether the curve is initialized or not
        /// </summary>
        private bool initialized = false;

        void Awake()
        {
            //Disable all points gameobjects in the curve
            this.DisablePoints();
        }

        void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                this.Init();
#endif
            this.InitPoints();
        }

        /// <summary>
        /// Init Curve
        /// </summary>
        public void Init()
        {
            if (this.initialized)
            {
                return;
            }

            this.initialized = true;
            this.CalculateBezierPoints();
            this.centroid = CommonUtil.GetCentroid(this.bezierPoints);
        }

        /// <summary>
        /// Re Init The Curve
        /// </summary>
        public void ReInit()
        {
            this.initialized = false;
            this.Init();
        }

        /// <summary>
        /// Return initialized as true/false
        /// </summary>
        /// <returns></returns>
        public bool IsInitialized()
        {
            return this.initialized;
        }

        /// <summary>
        /// Enable first point , disable others
        /// Set the position of the numbers of the Tracing Path relative to first , last points in the curve
        /// </summary>
        private void InitPoints()
        {
            TracingPath tracingPath = this.GetComponentInParent<TracingPath>();
            for (int i = 0; i < this.points.Count; i++)
            {
                if (i == 0)
                {
                    if (Application.isPlaying)
                    {
                        //enable first point
                        this.SetPointActiveValue(true, i);
                    }

                    //set first number in the first point
                    if (tracingPath.firstNumber != null)
                        tracingPath.firstNumber.position = this.points[i].position;
                }
                else
                {
                    //disable others
                    if (Application.isPlaying)
                    {
                        this.SetPointActiveValue(false, i);
                    }

                    //set second number in the last point
                    if (i == this.points.Count - 1)
                    {
                        if (tracingPath.secondNumber != null)
                            tracingPath.secondNumber.position = this.points[i].position;
                    }
                }
            }
        }

        /// <summary>
        /// Get the bezier points.
        /// </summary>
        /// <returns>The bezier list points.</returns>
        public List<Vector3> GetBezierPoints()
        {
            return this.bezierPoints;
        }

        /// <summary>
        /// Get the count of bezier points.
        /// </summary>
        /// <returns>The bezier points count.</returns>
        public int GetBezierPointsCount()
        {
            return this.bezierPoints.Count;
        }

        /// <summary>
        /// Calculate the bezier points.
        /// </summary>
        private void CalculateBezierPoints()
        {
            this.bezierPoints.Clear();

            //add not null points
            List<Transform> tempList = new List<Transform>();
            for (int i = 0; i < this.points.Count; i++)
            {
                if (this.points[i] != null)
                {
                    tempList.Add(this.points[i]);
                }
            }

            this.points = tempList;
            tempList = null;

            for (int i = 0; i < this.points.Count; i++)
            {
                this.bezierPoints.Add(this.points[i].position);
            }

            if (this.bezierPoints.Count > 2)
            {
                bezier.Interpolate(this.bezierPoints, this.smoothness);
                this.bezierPoints = bezier.GetDrawingPoints2();
            }
        }

        /// <summary>
        /// Get the first point in the curve
        /// </summary>
        /// <returns></returns>
        public Transform GetFirstPoint()
        {
            if (this.points.Count != 0)
                return this.points[0];

            return null;
        }

        /// <summary>
        /// Get the last point in the curve
        /// </summary>
        /// <returns></returns>
        public Transform GetLastPoint()
        {
            if (this.points.Count != 0)
                return this.points[this.points.Count - 1];

            return null;
        }

        /// <summary>
        /// Disable All Points
        /// </summary>
        public void DisablePoints()
        {
            for (int i = 0; i < this.points.Count; i++)
            {
                this.SetPointActiveValue(false, i);
            }
        }

        /// <summary>
        /// Set Point as Activiated or Deactivated
        /// </summary>
        /// <param name="status">Activiated or Deactivated</param>
        /// <param name="pointIndex">Index of the point</param>
        public void SetPointActiveValue(bool value, int pointIndex)
        {
            if (pointIndex > -1 && pointIndex < this.points.Count)
            {
                this.points[pointIndex].gameObject.SetActive(value);
            }
        }

#if UNITY_EDITOR

        [HideInInspector]
        public bool showContents = true;
        [HideInInspector]
        public Color curveColor = Colors.transparent;

        void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                return;
            }
            this.CalculateBezierPoints();
            this.DrawBezierCurve();
            this.InitPoints();
        }

        private void DrawBezierCurve()
        {
            if (this.curveColor == Colors.transparent)
            {
                this.curveColor = Colors.redColor;
                //curveColor = new Color(Random.Range(0, 255) / 255.0f, Random.Range(0, 255) / 255.0f, Random.Range(0, 255) / 255.0f, 1);
                DirtyUtil.MarkSceneDirty();
            }

            for (int i = 0; i < this.bezierPoints.Count; i++)
            {

                for (int j = 0; j < this.points.Count; j++)
                {
                    if (this.points[j].position == this.bezierPoints[i])
                    {

                        if (i == 0 || i == this.bezierPoints.Count - 1)
                        {
                            Gizmos.color = Colors.greenColor;
                        }
                        else
                        {
                            Gizmos.color = this.curveColor;
                        }
                        if (!Application.isPlaying)
                        {
                            Gizmos.DrawWireSphere(this.bezierPoints[i], 0.25f);
                        }
                        break;
                    }
                }
                Gizmos.color = this.curveColor;

                if (i + 1 < this.bezierPoints.Count)
                {
                    Gizmos.DrawLine(this.bezierPoints[i], this.bezierPoints[i + 1]);
                }
            }
        }

        public void CreateNewPoint()
        {
            GameObject newOb = new GameObject("Point" + this.points.Count);
            newOb.tag = "Point";
            newOb.transform.parent = this.transform;
            if (this.points.Count != 0)
            {
                newOb.transform.position = this.points[this.points.Count - 1].position + Vector3.zero * 0.35f; //use Vector3.one for offset
            }
            else
            {
                newOb.transform.position = Vector3.zero;
            }

            this.points.Add(newOb.transform);
            CircleCollider2D col = newOb.AddComponent<CircleCollider2D>();
            col.radius = this.pointColliderRadius;
            col.isTrigger = true;
            Selection.activeObject = newOb;
        }

        public static void CreateNewCurve()
        {
            GameObject newCurve = new GameObject("Curve");
            newCurve.AddComponent<Curve>();
            newCurve.transform.SetParent(Selection.activeGameObject.transform);
            newCurve.transform.localPosition = Vector3.zero;
            newCurve.transform.localScale = Vector3.one;

            Selection.activeGameObject = newCurve;
            DirtyUtil.MarkSceneDirty();
        }

        [MenuItem("Tools/English Tracing Book/Game Scene/Selected Curve/New Point #c", false, 0)]
        static void CreateNewPointMenu()
        {
            if (Selection.activeGameObject.GetComponent<Curve>() != null)
            {
                Selection.activeGameObject.GetComponent<Curve>().CreateNewPoint();
            }
            else if (Selection.activeGameObject.GetComponentInParent<Curve>() != null)
            {
                Selection.activeGameObject.GetComponentInParent<Curve>().CreateNewPoint();
            }

            DirtyUtil.MarkSceneDirty();
        }

        [MenuItem("Tools/English Tracing Book/Game Scene/Selected Curve/New Point #c", true, 0)]
        static bool CreateNewPointMenuValidate()
        {
            bool curveFound = false;

            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.GetComponent<Curve>() != null || Selection.activeGameObject.GetComponentInParent<Curve>() != null)
                {
                    curveFound = true;
                }
            }
            return !Application.isPlaying && curveFound;
        }
#endif
    }
}