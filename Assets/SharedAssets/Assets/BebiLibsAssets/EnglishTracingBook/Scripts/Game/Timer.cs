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
    public class Timer : MonoBehaviour
    {
        /// <summary>
        /// Text Component
        /// </summary>
        public Text uiText;

        /// <summary>
        /// The time in seconds.
        /// </summary>
        [HideInInspector]
        public int timeInSeconds;

        /// <summary>
        /// Whether the timer is paused or not.
        /// </summary>
        private bool isPaused;


        /// <summary>
        /// Whether the Timer is running
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// Whether to run timer on start or not
        /// </summary>
        public bool runOnStart;

        /// <summary>
        /// The time counter.
        /// </summary>
        private float timeCounter;

        /// <summary>
        /// The sleep time.
        /// </summary>
        private float sleepTime;

        /// <summary>
        /// Static instance of this class.
        /// </summary>
        public static Timer instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        void Start()
        {

            if (this.uiText == null)
            {
                this.uiText = this.GetComponent<Text>();
            }

            if (this.runOnStart)
            {
                ///Run the Timer
                this.Run();
            }
        }

        /// <summary>
        /// Run the Timer.
        /// </summary>
        public void Run()
        {
            if (!this.isRunning)
            {
                this.isPaused = false;
                this.timeCounter = 0;
                this.sleepTime = 0.01f;
                this.isRunning = true;
                this.timeInSeconds = 0;
                this.InvokeRepeating("Wait", 0, this.sleepTime);
            }
        }

        /// <summary>
        /// Pause the Timer.
        /// </summary>
        public void Pause()
        {
            this.isPaused = false;
        }

        /// <summary>
        /// Resume the Timer.
        /// </summary>
        public void Resume()
        {
            this.isPaused = true;
        }

        /// <summary>
        /// Stop the Timer.
        /// </summary>
        public void Stop()
        {
            if (this.isRunning)
            {
                this.isRunning = false;
                this.CancelInvoke();
            }
        }

        /// <summary>
        /// Reset the timer.
        /// </summary>
        public void Reset()
        {
            this.Stop();
            this.Run();
        }

        /// <summary>
        /// Wait.
        /// </summary>
        private void Wait()
        {
            if (this.isPaused)
            {
                return;
            }
            this.timeCounter += this.sleepTime;
            this.timeInSeconds = (int)this.timeCounter;
            this.ApplyTime();
        }

        /// <summary>
        /// Applies the time into TextMesh Component.
        /// </summary>
        private void ApplyTime()
        {
            if (this.uiText == null)
            {
                return;
            }
            //	int mins = timeInSeconds / 60;
            //	int seconds = timeInSeconds % 60;

            //	uiText.text = "Time : " + GetNumberWithZeroFormat (mins) + ":" + GetNumberWithZeroFormat (seconds);
            this.uiText.text = this.timeInSeconds.ToString();
        }

        /// <summary>
        /// Get the number with zero format.
        /// </summary>
        /// <returns>The number with zero format.</returns>
        /// <param name="number">Ineger Number.</param>
        public static string GetNumberWithZeroFormat(int number)
        {
            string strNumber = "";
            if (number < 10)
            {
                strNumber += "0";
            }
            strNumber += number;

            return strNumber;
        }
    }
}