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
    public class Destroy : MonoBehaviour
    {
        /// <summary>
        /// Destroy time.
        /// </summary>
        public float time = 3;

        // Use this for initialization
        void Start()
        {
            //Destry the current gameobject
            Destroy(this.gameObject, this.time);
        }
    }
}
