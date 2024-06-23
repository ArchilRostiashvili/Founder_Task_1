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
    public class BlackArea : MonoBehaviour
    {
        /// <summary>
        /// Black area animator.
        /// </summary>
        private Animator blackAreaAnimator;

        /// <summary>
        /// A static instance of this class.
        /// </summary>
        public static BlackArea instance;

        // Use this for initialization
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            if (this.blackAreaAnimator == null)
            {
                this.blackAreaAnimator = this.GetComponent<Animator>();
            }

            this.SetActiveFalse();
        }

        /// <summary>
        /// Show the Black Area
        /// </summary>
        public void Show()
        {
            if (this.blackAreaAnimator == null)
            {
                return;
            }

            this.SetActiveTrue();

            this.blackAreaAnimator.SetTrigger("Running");
        }

        /// <summary>
        /// Hide the Black Area
        /// </summary>
        public void Hide()
        {
            if (this.blackAreaAnimator != null)
                this.blackAreaAnimator.SetBool("Running", false);

            this.SetActiveFalse();
        }

        /// <summary>
        /// Set gameobject active false.
        /// </summary>
        public void SetActiveFalse()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Set gameobject active true.
        /// </summary>
        public void SetActiveTrue()
        {
            this.gameObject.SetActive(true);
        }

    }
}