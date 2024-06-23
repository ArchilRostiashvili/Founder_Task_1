using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BebiLibs;

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
    public class SceneLoader : MonoBehaviour
    {

        /// <summary>
        /// The canvas group.
        /// </summary>
        public CanvasGroup canvasGroup;

        /// <summary>
        /// Loading image reference
        /// </summary>
        public Image loadingImage;

        /// <summary>
        /// This Gameobject defined as a Singleton.
        /// </summary>
        public static SceneLoader instance;

        void Awake()
        {
            // Debug.Log("Register For Event");
            if (instance == null)
            {
                instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }


            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDestroy()
        {
            if (instance != null)
            {
                if (this.GetInstanceID() == instance.GetInstanceID())
                {
                    //Debug.Log("On Tracing Destroy Event");
                    SceneManager.sceneLoaded -= OnSceneLoaded;
                    instance = null;
                }
            }
        }

        /// <summary>
        /// On Load the scene.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (instance != null)
            {
                gameObject.SetActive(true);
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1;
                    StartCoroutine(CanvasFade(FadeType.FADE_OUT));
                }
            }
        }

        /// <summary>
        /// Fade in/out the canvas.
        /// </summary>
        public IEnumerator CanvasFade(FadeType fadeType)
        {
            canvasGroup.blocksRaycasts = true;

            //float delay = 0.03f;
            float alphaOffset = 0.1f;

            alphaOffset = (fadeType == FadeType.FADE_IN ? alphaOffset : -alphaOffset);

            // while (fadeType == FadeType.FADE_IN ? canvasGroup.alpha < 1 : canvasGroup.alpha > 0)
            // {
            //     yield return new WaitForSeconds(delay);
            //     canvasGroup.alpha += alphaOffset;
            // }

            canvasGroup.alpha = 0;
            yield return null;
            canvasGroup.blocksRaycasts = false;

            if (fadeType == FadeType.FADE_OUT)
            {
                gameObject.SetActive(false);
                if (loadingImage != null)
                    loadingImage.gameObject.SetActive(false);
            }
        }

        public enum FadeType
        {
            FADE_IN,
            FADE_OUT
        }
    }
}