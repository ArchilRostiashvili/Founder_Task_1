using IndieStudio.EnglishTracingBook.Utility;
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
    public class Booster : MonoBehaviour
    {
        /// <summary>
        /// The number text of the booster.
        /// </summary>
        public Text number;

        /// <summary>
        /// The type of the booster.
        /// </summary>
        public Type type;

        /// <summary>
        /// The remaining value/count of the booster.
        /// </summary>
        private int value;

        /// <summary>
        /// References array.
        /// You can attach references/objects to be accessed
        /// </summary>
        public Transform[] references;

        // Use this for initialization
        void Awake()
        {
            //Load booster value
            this.value = this.LoadValue();
            if (this.value == -1)
            {
                //If there is no data , use default value
                this.ResetValue();
            }

            //Refresh/Set the number text value
            this.RefreshNumberText();
        }

        /// <summary>
        /// Get the value/count of the booster.
        /// </summary>
        /// <returns>The value/count of the booster.</returns>
        public int GetValue()
        {
            return this.value;
        }

        /// <summary>
        /// Decrease booster's value by one.
        /// </summary>
        public void DecreaseValue()
        {
            if (this.value == 0)
            {
                return;
            }

            this.value--;
            this.RefreshNumberText();
            this.SaveValue();
        }

        /// <summary>
        /// Load booster's value/count.
        /// </summary>
        /// <returns>The value/count of the booster.</returns>
        public int LoadValue()
        {
            if (this.type == Type.HELP_USER)
            {
                //return DataManager.GetHelpCountValue();
            }
            return 0;
        }

        /// <summary>
        /// Save the value/count of the booster.
        /// </summary>
        public void SaveValue()
        {
            if (this.type == Type.HELP_USER)
            {
                //DataManager.SaveHelpCountValue(this.value);
            }
        }

        /// <summary>
        /// Reset the value/count of the booster.
        /// </summary>
        public void ResetValue()
        {
            this.value = 3;

            if (this.type == Type.HELP_USER)
            {
                //DataManager.SaveHelpCountValue(this.value);
            }
            this.RefreshNumberText();
        }

        /// <summary>
        /// Refresh number text value.
        /// </summary>
        private void RefreshNumberText()
        {
            if (this.number != null)
                this.number.text = this.value.ToString();
        }

        public enum Type
        {
            HELP_USER,
        };

    }
}