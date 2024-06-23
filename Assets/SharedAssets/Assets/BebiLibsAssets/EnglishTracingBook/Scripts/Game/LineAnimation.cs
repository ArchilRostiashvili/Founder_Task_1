using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public class LineAnimation : MonoBehaviour
    {
        /// <summary>
        /// List of points of the line to animate line through them
        /// </summary>
        private List<Vector3> points;

        /// <summary>
        /// Whether to animate the line or not
        /// </summary>
        private bool animateLine = false;

        /// <summary>
        /// An index used as a pointer on the points in list
        /// </summary>
        private int currentPointIndex;

        /// <summary>
        /// Current Point (where we are) , Traget Point (move to this point)
        /// </summary>
        private Vector3 currentPoint, targetPoint;

        /// <summary>
        /// Line Reference (contains Line Renderer, Line Z Poition ..etc )
        /// </summary>
        private Line line;

        /// <summary>
        /// Animation Speed
        /// </summary>
        private readonly float speed = 500;

        /// <summary>
        /// An unity event invoked when the animation is done or finished
        /// </summary>
        private UnityEvent animationDoneEvent;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!this.animateLine)
            {
                return;
            }

            if (this.currentPointIndex < this.points.Count - 1)
            {
                if (Vector2.Distance(this.currentPoint, this.targetPoint) > 0.1f)
                {
                    //move until you reach target point
                    this.currentPoint = Vector2.MoveTowards(this.currentPoint, this.targetPoint, Time.deltaTime * this.speed);
                    this.currentPoint.z = this.line.pointZPosition;

                    //update point position in line renderer
                    this.line.lineRenderer.SetPosition(this.currentPointIndex + 1, this.currentPoint);
                }
                else
                {
                    //set last point in the line renderer as the target point position
                    this.line.lineRenderer.SetPosition(this.currentPointIndex + 1, this.targetPoint);

                    //move to next point

                    this.currentPoint = this.targetPoint;
                    this.currentPointIndex++;

                    if (this.currentPointIndex + 1 < this.points.Count)
                    {
                        this.line.AddPoint(this.targetPoint);
                        this.targetPoint = this.points[this.currentPointIndex + 1];
                        this.targetPoint.z = this.line.pointZPosition;
                    }

                }

            }
            else
            {
                //stop animation (already we are done)
                this.animateLine = false;

                if (this.animationDoneEvent != null)
                    this.animationDoneEvent.Invoke();
            }
        }

        public void Run(List<Vector3> points, Line line, UnityEvent animationDoneEvent = null)
        {
            // init values , and start line animation 

            this.points = points;
            this.line = line;
            this.animationDoneEvent = animationDoneEvent;

            line.AddPoint(points[0]);
            line.AddPoint(points[0]);

            this.currentPoint = points[0];
            this.targetPoint = points[1];

            this.currentPoint.z = this.targetPoint.z = line.pointZPosition;

            this.currentPointIndex = 0;

            this.animateLine = true;
        }
    }
}