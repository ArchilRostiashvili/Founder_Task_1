using IndieStudio.EnglishTracingBook.Utility;
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
    public class Dialog : MonoBehaviour
    {
        /// <summary>
        /// The animator of the dialog.
        /// </summary>
        public Animator animator;

        /// <summary>
        /// The visible flag.
        /// </summary>
        [HideInInspector]
        public bool visible;

        void Start()
        {
            if (this.animator == null)
            {
                this.animator = this.GetComponent<Animator>();
            }
        }

        /// <summary>
        /// Show the dialog.
        /// </summary>
        public void Show(bool playClickSFX)
        {
            this.SetActiveTrue();

            BlackArea.instance.Show();
            this.animator.SetBool("Off", false);
            this.animator.SetTrigger("On");
            this.visible = true;
        }

        /// <summary>
        /// Hide the dialog.
        /// </summary>
        public void Hide(bool playClickSFX)
        {
            if (playClickSFX)
                AudioSources.instance.PlayButtonClickSFX();

            BlackArea.instance.Hide();
            this.animator.SetBool("On", false);
            this.animator.SetTrigger("Off");
            this.visible = false;
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