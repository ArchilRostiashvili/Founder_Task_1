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
    [ExecuteInEditMode]
    public class Line : MonoBehaviour
    {
        /// <summary>
        /// The line renderer reference.
        /// </summary>
        public LineRenderer lineRenderer;

        /// <summary>
        /// The material of the line.
        /// </summary>
        public Material material;

        /// <summary>
        /// The color gradient of the line.
        /// </summary>
        public Gradient color;

        /// <summary>
        /// The width of the line.
        /// </summary>
        [Range(0, 10)]
        public float width = 1f;

        /// <summary>
        /// The minimum offset between points.
        /// </summary>
        [Range(0, 10)]
        public float offsetBetweenPoints = 6f;

        /// <summary>
        /// The point Z position.
        /// </summary>
        [Range(-20, 20)]
        public float pointZPosition;

        /// <summary>
        /// The sorting order of line
        /// </summary>
        public int sortingOrder;

        /// <summary>
        /// The points of the line.
        /// </summary>
        [HideInInspector]
        public List<Vector3> points = new List<Vector3>();

        [HideInInspector]
        public Vector3 CenterPoint;

        //Drawing points list of the bezier path
        private List<Vector3> bezierDrawingPoints;

        /// <summary>
        /// Bezier instance
        /// </summary>
        public static Bezier bezier;

        void Awake()
        {
            this.Init();
        }

        void Update()
        {
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        public void Init()
        {
            if (bezier == null)
            {
                bezier = new Bezier();
            }

            if (this.material == null)
            {
                this.material = new Material(Shader.Find("Sprites/Default"));
            }

            this.points = new List<Vector3>();
            this.lineRenderer = this.GetComponent<LineRenderer>();
            this.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            if (this.lineRenderer != null)
            {
                this.lineRenderer.material = this.material;
                this.lineRenderer.widthMultiplier = this.width;
            }
        }

        /// <summary>
        /// Add the point to the line.
        /// </summary>
        /// <param name="point">Point.</param>
        public void AddPoint(Vector3 point)
        {
            if (this.lineRenderer != null)
            {
                point.z = this.pointZPosition;
                if (this.points.Count > 1)
                {
                    if (Vector3.Distance(point, this.points[this.points.Count - 1]) < this.offsetBetweenPoints)
                    {
                        return;//skip the point
                    }
                }

                this.points.Add(point);
                this.lineRenderer.positionCount = this.points.Count;
                this.lineRenderer.SetPosition(this.points.Count - 1, point);
                CenterPoint = this.points[this.points.Count / 2];
            }
        }

        /// <summary>
        /// Set the material of the line.
        /// </summary>
        /// <param name="material">Material.</param>
        public void SetMaterial(Material material)
        {
            this.material = material;
            if (this.lineRenderer != null)
            {
                this.lineRenderer.material = this.material;
            }
        }

        /// <summary>
        /// Sets the width of the line.
        /// </summary>
        /// <param name="width">Line width.</param>
        public void SetWidth(float width)
        {
            if (this.lineRenderer != null)
            {
                this.lineRenderer.widthMultiplier = width;
            }
            this.width = width;
        }

        /// <summary>
        /// Set the color of the line.
        /// </summary>
        /// <param name="value">Value.</param>
        public void SetColor(Gradient color)
        {

            if (this.lineRenderer != null)
            {
                this.lineRenderer.colorGradient = color;
            }

            this.color = color;
        }

        /// <summary>
        /// Set the sorting order of the line.
        /// </summary>
        /// <param name="sortingOrder">Sorting order.</param>
        public void SetSortingOrder(int sortingOrder)
        {

            if (this.lineRenderer != null)
            {
                this.lineRenderer.sortingOrder = sortingOrder;
            }
            this.sortingOrder = sortingOrder;
        }

        /// <summary>
        /// Bezier interpolate to smooth the line's curve.
        /// </summary>
        public void BezierInterpolate(float somothness)
        {
            if (this.points.Count < 2)
                return;

            bezier.Interpolate(this.points, somothness);
            this.bezierDrawingPoints = bezier.GetDrawingPoints2();
            this.lineRenderer.positionCount = this.bezierDrawingPoints.Count;
            this.lineRenderer.SetPositions(this.bezierDrawingPoints.ToArray());
        }

        /// <summary>
        /// Get the points count of the line.
        /// </summary>
        /// <returns>The points count.</returns>
        public int GetPointsCount()
        {
            return this.points.Count;
        }

        public void Reset()
        {
            this.lineRenderer.positionCount = 0;
            this.points.Clear();
        }
    }
}