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

namespace IndieStudio.EnglishTracingBook.Utility
{
    [DisallowMultipleComponent]
    public class TransformScale : MonoBehaviour
    {
        /// <summary>
        /// Whether to run scale on start or not
        /// </summary>
        public bool runOnStart;

        /// <summary>
        /// Whether the scale process is running or not
        /// </summary>
        private bool isRunning = false;

        /// <summary>
        /// The speed of the scale
        /// </summary>
        [Range(0, 10)]
        public float speed = 4;

        /// <summary>
        /// Whehter to do scale for x , y , z coordinates or not
        /// </summary>
        public bool scaleX, scaleY, scaleZ;

        /// <summary>
        /// The target scale we need to follow
        /// </summary>
        public Vector3 targetScale = Vector3.zero;

        /// <summary>
        /// The initial scale of the object
        /// </summary>
        private Vector3 initalScale;

        /// <summary>
        /// A temp scale vectors
        /// </summary>
        private Vector3 tempScale, tepmTarget;

        /// <summary>
        /// Wehther the scale is loop or not (move to the target scale then to the initial scale then reverse, so on and so forth)
        /// </summary>
        public bool loop;

        void Start()
        {
            this.initalScale = this.transform.localScale;

            this.Stop();

            if (this.runOnStart)
            {
                this.Run();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!this.isRunning)
            {
                return;
            }

            this.Scale();
        }

        public void Run()
        {
            this.isRunning = true;
        }

        public void Stop()
        {
            this.isRunning = false;
            this.tepmTarget = this.targetScale;
            this.transform.localScale = this.initalScale;
        }

        private void Scale()
        {
            this.tempScale = this.transform.localScale;

            if (this.scaleX)
            {
                this.tempScale.x = Mathf.MoveTowards(this.tempScale.x, this.tepmTarget.x, this.speed * Time.deltaTime);
            }
            else
            {
                this.tepmTarget.x = this.tempScale.x;
            }

            if (this.scaleY)
            {
                this.tempScale.y = Mathf.MoveTowards(this.tempScale.y, this.tepmTarget.y, this.speed * Time.deltaTime);
            }
            else
            {
                this.tepmTarget.y = this.tempScale.y;
            }

            if (this.scaleZ)
            {
                this.tempScale.z = Mathf.MoveTowards(this.tempScale.z, this.tepmTarget.z, this.speed * Time.deltaTime);
            }
            else
            {
                this.tepmTarget.z = this.tempScale.z;
            }

            this.transform.localScale = this.tempScale;

            if (Mathf.Approximately(this.transform.localScale.magnitude, this.tepmTarget.magnitude))
            {
                this.transform.localScale = this.tepmTarget;

                if (this.loop)
                {
                    this.tepmTarget = this.transform.localScale.magnitude == this.initalScale.magnitude ? this.targetScale : this.initalScale;
                }
                else
                {
                    this.isRunning = false;
                }
            }
        }
    }
}
