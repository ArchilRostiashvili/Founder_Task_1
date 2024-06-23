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
    [DisallowMultipleComponent]
    public class StarsRate : MonoBehaviour
    {

        /// <summary>
        /// The references of the stars images.
        /// </summary>
        public Image[] stars;

        /// <summary>
        /// The star on,off sprites.
        /// </summary>
        public Sprite starOn, starOff;

        /// <summary>
        /// The shapes manager reference as name.
        /// </summary>
        public string shapesManagerReference;

        void Start()
        {

            //Setting up the stars rate
            ShapesManager shapesManager = ShapesManager.shapesManagers[this.shapesManagerReference];
            int starsRate = shapesManager.GetStarsRate();

            if (starsRate == 0)
            {//Zero Stars
                this.stars[0].sprite = this.starOff;
                this.stars[1].sprite = this.starOff;
                this.stars[2].sprite = this.starOff;
            }
            else if (starsRate == 1)
            {//One Star
                this.stars[0].sprite = this.starOn;
                this.stars[1].sprite = this.starOff;
                this.stars[2].sprite = this.starOff;
            }
            else if (starsRate == 2)
            {//Two Stars
                this.stars[0].sprite = this.starOn;
                this.stars[1].sprite = this.starOn;
                this.stars[2].sprite = this.starOff;
            }
            else
            {//Three Stars
                this.stars[0].sprite = this.starOn;
                this.stars[1].sprite = this.starOn;
                this.stars[2].sprite = this.starOn;
            }
        }
    }
}