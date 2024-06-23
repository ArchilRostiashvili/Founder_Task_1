using UnityEngine;
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
    public class UserInputManager : MonoBehaviour
    {
        public InputField input;

        // Use this for initialization
        void Start()
        {
            ShapesManager.shapesManagerReference = "SShapesManager";

            if (UserTraceInput.instance != null)
            {
                this.input.text = UserTraceInput.instance.text;
            }
        }

        public void LoadGameScene()
        {
            if (UserTraceInput.instance != null)
            {
                if (!string.IsNullOrEmpty(this.input.text))
                {
                    UserTraceInput.instance.text = this.input.text;
                    UIEvents.instance.LoadGameScene();
                }
                else
                {
                    AudioSources.instance.PlayLockedSFX();
                }
            }
        }

    }
}