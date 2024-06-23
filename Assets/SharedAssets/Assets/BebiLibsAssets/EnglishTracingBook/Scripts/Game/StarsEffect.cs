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
    public class StarsEffect : MonoBehaviour
    {
        /// <summary>
        /// The position of Stars Effect in the World Space.
        /// </summary>
        private Vector3 tempPosition;

        /// <summary>
        /// The stars effect prefab.
        /// </summary>
        public GameObject starsEffectPrefab;

        /// <summary>
        /// The star effect Z position.
        /// </summary>
        [Range(-50, 50)]
        public float starEffectZPosition = -5;

        /// <summary>
        /// The stars effect parent.
        /// </summary>
        public Transform starsEffectParent;

        /// <summary>
        /// Create the stars effect.
        /// </summary>
        public void CreateStarsEffect()
        {
            if (this.starsEffectPrefab == null)
            {
                return;
            }
            this.tempPosition = this.transform.position;
            this.tempPosition.z = this.starEffectZPosition;
            GameObject starsEffect = Instantiate(this.starsEffectPrefab, this.tempPosition, Quaternion.identity) as GameObject;
            if (this.starsEffectParent != null)
                starsEffect.transform.parent = this.starsEffectParent;//setting up Stars Effect Parent
            starsEffect.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}