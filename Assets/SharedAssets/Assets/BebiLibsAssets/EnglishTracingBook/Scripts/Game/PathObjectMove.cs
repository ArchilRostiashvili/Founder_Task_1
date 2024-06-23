using IndieStudio.EnglishTracingBook.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    public class PathObjectMove : MonoBehaviour
    {
        /// <summary>
        /// The curve reference.
        /// </summary>
        public Curve curve;

        /// <summary>
        /// The image component
        /// </summary>
        public Image image;

        /// <summary>
        /// The transform scale component
        /// </summary>
        public TransformScale transformScale;

        /// <summary>
        /// Whether to move on start or not
        /// </summary>
        public bool moveOnStart;

        /// <summary>
        /// Whether to make the object look at the target or not
        /// </summary>
        public bool allowLook = false;

        /// <summary>
        /// The target point to move.
        /// </summary>
        [HideInInspector]
        public Vector2 target;

        /// <summary>
        /// The speed of the movement.
        /// </summary>
        [Range(0, 10)]
        public float speed = 3f;

        /// <summary>
        /// The look or rotation angle speed.
        /// </summary>
        [Range(0, 10)]
        public float lookSpeed = 5;

        /// <summary>
        /// Whether object reached the target point or not.
        /// </summary>
        [HideInInspector]
        public bool reachedTarget;

        /// <summary>
        /// The minimum reach offset.
        /// </summary>
        [Range(0, 1)]
        public float minReachOffset = 0.1f;

        /// <summary>
        /// The follow offset.
        /// </summary>
        public Vector2 followOffset = Vector2.zero;

        /// <summary>
        /// The on reach new point event.
        /// </summary>
        private UnityEvent onReachNewPointEvent;

        /// <summary>
        /// The index of the target vector point.
        /// </summary>
        private int current;

        /// <summary>
        /// The path object reference.
        /// </summary>
        private Transform pathObject;

        /// <summary>
        /// Whether the movement is loop or not.
        /// </summary>
        public bool loop = true;

        /// <summary>
        /// Whether to reverse the movement or not.
        /// </summary>
        private bool reverse;

        /// <summary>
        /// Whether move process is running or not in the Update.
        /// </summary>
        private bool isRunning;

        // Use this for initialization
        void Start()
        {
            this.Init();
        }

        // Update is called once per frame
        void Update()
        {
            if (!this.isRunning || this.reachedTarget || this.curve == null)
            {
                return;
            }

            this.Follow();
        }

        //Init and Setting up references
        public void Init()
        {
            if (this.curve == null)
            {
                this.curve = this.GetComponent<Curve>();
            }

            if (this.pathObject == null)
            {
                this.pathObject = this.GetComponent<Transform>();
            }

            if (this.image == null)
            {
                this.image = this.GetComponent<Image>();
            }

            if (this.transformScale == null)
            {
                this.transformScale = this.GetComponent<TransformScale>();
            }

            this.image.enabled = false;

            this.minReachOffset = Mathf.Abs(this.minReachOffset);
            this.reachedTarget = false;

            if (this.onReachNewPointEvent == null)
            {
                this.onReachNewPointEvent = new UnityEvent();
            }

            this.onReachNewPointEvent.AddListener(() => this.NextPoint());

            this.current = 0;

            if (!this.moveOnStart)
            {
                this.Stop();
            }
            else
            {
                this.Move();
            }
        }

        /// <summary>
        /// Follow the target
        /// </summary>
        private void Follow()
        {
            //follow the target's postion
            this.transform.position = Vector2.MoveTowards(this.transform.position, this.target, this.speed * Time.deltaTime);

            if (this.allowLook)
            {
                //look at the arget
                Vector3 eulerAngle = this.transform.eulerAngles;
                Vector2 direction = Vector2.zero;
                direction.x = this.transform.position.x - this.target.x;
                direction.y = this.transform.position.y - this.target.y;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

                eulerAngle.z = Mathf.LerpAngle(eulerAngle.z, angle, this.lookSpeed * Time.smoothDeltaTime);
                this.transform.eulerAngles = eulerAngle;
            }

            //check whether reached the target or not
            if (this.NearByTarget())
            {
                this.reachedTarget = true;
                this.onReachNewPointEvent.Invoke();
            }
        }

        /// <summary>
        /// Reverse the movement.
        /// </summary>
        public void Reverse()
        {
            this.reverse = !this.reverse;
            this.NextPoint();
        }

        /// <summary>
        /// Determines whether this instance is reversed or not.
        /// </summary>
        /// <returns><c>true</c> if this instance is reversed; otherwise, <c>false</c>.</returns>
        public bool IsReversed()
        {
            return this.reverse;
        }

        /// <summary>
        /// Moves to start point.
        /// </summary>
        public void MoveToStart()
        {
            if (this.curve != null)
            {
                if (this.curve.points.Count != 0)
                    this.transform.position = this.curve.points[0].position;

                if (this.curve.points.Count == 1)
                {
                    this.transformScale.Run();
                }
                else if (this.curve.points.Count > 1)
                {
                    this.transformScale.Stop();
                }
            }

            this.MoveTo(0);
        }

        /// <summary>
        /// Move this instance.
        /// </summary>
        public void Move()
        {
            this.image.enabled = true;
            this.isRunning = true;
        }


        /// <summary>
        /// Moves to point by index.
        /// </summary>
        /// <param name="index">Index.</param>
        private void MoveTo(int index)
        {
            if (this.curve == null)
            {
                return;
            }

            if (index > -1 && index < this.curve.GetBezierPointsCount())
            {
                this.Reset();
                this.SetTarget(this.current);
                this.Move();
            }
        }

        /// <summary>
        /// Set current target by index
        /// </summary>
        public void SetTarget(int index)
        {
            this.target = this.curve.GetBezierPoints()[index];
        }

        /// <summary>
        /// Stop this instance.
        /// </summary>
        public void Stop()
        {
            this.image.enabled = false;
            this.isRunning = false;
            this.reachedTarget = false;
            this.current = 0;
        }

        /// <summary>
        /// Move to the next point.
        /// </summary>
        private void NextPoint()
        {
            if (this.reverse)
            {
                this.current--;
            }
            else
            {
                this.current++;
            }

            /*
            //Destroy path object on reach the end of the path
            if (curve.destoryObjectsOnReachEnd && current == curve.GetBezierPointsCount())
            {
                Destroy(pathObject.gameObject);
                return;
            }*/

            if (this.loop)
            {
                if (this.reverse)
                {
                    if (this.current == -1)
                    {
                        this.current = this.curve.GetBezierPointsCount() - 1;
                    }
                }
                else
                {
                    if (this.current == this.curve.GetBezierPointsCount())
                    {
                        //set position to the first point
                        //transform.position = curve.GetFirstPoint().position;
                        this.current = 0;
                    }
                }

            }

            this.MoveTo(this.current);
        }


        /// <summary>
        /// Whether object is nearby the target point or not.
        /// </summary>
        /// <returns><c>true</c>, if the target was close, <c>false</c> otherwise.</returns>
        private bool NearByTarget()
        {
            return Vector2.Distance(this.target, this.transform.position) <= this.TotalOffset();
        }

        /// <summary>
        /// The total offset between the object and the target point.
        /// </summary>
        /// <returns>The offset.</returns>
        private float TotalOffset()
        {
            return this.minReachOffset + this.followOffset.magnitude;
        }

        /// <summary>
        /// Reset this instance.
        /// </summary>
        private void Reset()
        {
            this.isRunning = false;
            this.reachedTarget = false;
        }
    }
}