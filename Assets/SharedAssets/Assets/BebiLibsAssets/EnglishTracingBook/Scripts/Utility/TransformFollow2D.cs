using System;
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

namespace IndieStudio.EnglishTracingBook.Utility
{
    [DisallowMultipleComponent]
    public class TransformFollow2D : MonoBehaviour
    {
        /// <summary>
        /// Whether follow is running or not.
        /// </summary>
        public bool isRunning;

        /// <summary>
        /// The target transform.
        /// </summary>
        public Transform target;

        /// <summary>
        /// Target as Vector3
        /// </summary>
        public Vector3 targetVector;

        /// <summary>
        /// The follow speed.
        /// </summary>
        public float speed = 1;

        /// <summary>
        /// The follow method.
        /// </summary>
        public FollowMethod followMethod = FollowMethod.LERP;

        /// <summary>
        /// The mode of the target / Vector3 or Transform
        /// </summary>
        public TargetMode targetMode = TargetMode.TRANSFORM;

        /// <summary>
        /// The follow offset.
        /// </summary>
        public Vector2 followOffset = Vector2.zero;

        /// <summary>
        /// Whether to follow x , follow y coordinates.
        /// </summary>
        public bool followX = true, followY = true;

        /// <summary>
        /// Whether to apply follow sign.
        /// </summary>
        public bool applyFollowSign;

        /// <summary>
        /// Whether the follow is continuous or not.
        /// </summary>
        public bool continuousFollow;

        /// <summary>
        /// Whether reached target or not.
        /// </summary>
        public bool reachedTarget;

        /// <summary>
        /// A temp vector position.
        /// </summary>
        private Vector2 tempPosition;

        /// <summary>
        /// A temp vector.
        /// </summary>
        private Vector3 tempVector;

        /// <summary>
        /// The minimum reach offset.
        /// </summary>
        private float minReachOffset = 0.01f;

        /// <summary>
        /// On reach target unity event.
        /// </summary>
        public UnityEvent onReachTargeEvent;

        void Start()
        {
            this.minReachOffset = Math.Abs(this.minReachOffset);
            this.reachedTarget = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!this.isRunning)
            {
                return;
            }

            if (this.targetMode == TargetMode.TRANSFORM && this.target == null)
            {
                return;
            }

            if (this.reachedTarget && !this.continuousFollow)
            {
                return;
            }

            //initial postion
            this.tempVector = this.transform.position;

            if (this.followX)
            {
                this.tempVector.x = this.GetValue(this.transform.position.x, this.targetMode == TargetMode.TRANSFORM ? this.target.position.x : this.targetVector.x + (Mathf.Sign(this.transform.localScale.x) * this.followOffset.x));
            }
            else
            {
                this.tempVector.x = this.targetMode == TargetMode.TRANSFORM ? this.target.position.x : this.targetVector.x;
            }

            if (this.followY)
            {
                this.tempVector.y = this.GetValue(this.transform.position.y, this.targetMode == TargetMode.TRANSFORM ? this.target.position.y : this.targetVector.y + this.followOffset.y);
            }
            else
            {
                this.tempVector.y = this.targetMode == TargetMode.TRANSFORM ? this.target.position.y : this.targetVector.y;
            }

            //apply new postion
            this.transform.position = this.tempVector;

            if (this.applyFollowSign)
            {
                //initial scale
                this.tempVector = this.transform.localScale;

                if (this.transform.position.x < (this.targetMode == TargetMode.TRANSFORM ? this.target.position.x : this.targetVector.x) + (Mathf.Sign(this.transform.localScale.x) * this.followOffset.x))
                {
                    this.tempVector.x = Mathf.Abs(this.tempVector.x);
                }
                else
                {
                    this.tempVector.x = -Mathf.Abs(this.tempVector.x);
                }
                this.transform.localScale = this.tempVector;
            }

            if (this.NearByTarget())
            {
                this.reachedTarget = true;
                if (this.onReachTargeEvent != null)
                    this.onReachTargeEvent.Invoke();
            }
        }

        /// <summary>
        /// Whether the object is nearby the target or not.
        /// </summary>
        /// <returns><c>true</c>, if the target is nearby, <c>false</c> otherwise.</returns>
        private bool NearByTarget()
        {
            this.tempPosition = this.transform.position;

            if (this.followX && !this.followY)
            {
                this.tempPosition.y = this.targetMode == TargetMode.TRANSFORM ? this.target.position.y : this.targetVector.y;
            }
            else if (this.followY && !this.followX)
            {
                this.tempPosition.x = this.targetMode == TargetMode.TRANSFORM ? this.target.position.x : this.targetVector.x;
            }

            return Vector2.Distance(this.targetMode == TargetMode.TRANSFORM ? this.target.position : this.targetVector, this.tempPosition) <= this.TotalOffset();
        }

        /// <summary>
        /// The total reach offset.
        /// </summary>
        /// <returns>The offset.</returns>
        private float TotalOffset()
        {
            return this.minReachOffset + this.followOffset.magnitude;
        }

        /// <summary>
        /// Gets the follow value.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="currentValue">Current value.</param>
        /// <param name="targetValue">Next Target value.</param>
        private float GetValue(float currentValue, float targetValue)
        {
            float returnValue = 0;

            if (this.followMethod == FollowMethod.LERP)
            {
                returnValue = Mathf.Lerp(currentValue, targetValue, this.speed * Time.smoothDeltaTime);
            }
            else if (this.followMethod == FollowMethod.MOVE_TOWARDS)
            {
                returnValue = Mathf.MoveTowards(currentValue, targetValue, this.speed * Time.smoothDeltaTime);
            }
            else if (this.followMethod == FollowMethod.SMOOTH_STEP)
            {
                returnValue = Mathf.SmoothStep(currentValue, targetValue, this.speed * Time.smoothDeltaTime);
            }
            return returnValue;
        }


        /// <summary>
        /// Move this instance.
        /// </summary>
        public void Move()
        {
            this.isRunning = true;
        }

        /// <summary>
        /// Reset this instance.
        /// </summary>
        public void Reset()
        {
            this.target = null;
            this.isRunning = false;
            this.reachedTarget = false;
        }

        public enum FollowMethod
        {
            LERP,
            SMOOTH_STEP,
            MOVE_TOWARDS
        };

        public enum TargetMode
        {
            TRANSFORM,
            VECTOR3
        }
    }
}