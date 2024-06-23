using IndieStudio.EnglishTracingBook.Utility;
using System.Collections.Generic;
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
    public class Settings : MonoBehaviour
    {
        /// <summary>
        /// The music slider reference.
        /// </summary>
        public Slider musicSlider;

        /// <summary>
        /// The sfx slider reference.
        /// </summary>
        public Slider sfxSlider;

        /// <summary>
        /// Tracing mode button component
        /// </summary>
        public Button tracingModeButton;

        /// <summary>
        /// Sprites of line/fill tracing modes
        /// </summary>
        public Sprite tracingModeLine, tracingModeFill;

        /// <summary>
        /// Whether the vibration is enabled or not.
        /// </summary>
        public static bool vibrationEnabled;

        /// <summary>
        /// The value of the tracing mode
        /// </summary>
        private ShapesManager.TracingMode tracingMode;

        // Use this for initialization
        void Start()
        {
            this.SetMusicValue(AudioSources.instance.MusicAudioSource().mute);
            this.SetSFXValue(AudioSources.instance.SFXAudioSource().mute);

            this.tracingMode = DataManager.GetTracingModeValue() == 0 ? ShapesManager.TracingMode.FILL : ShapesManager.TracingMode.LINE;
            this.SetTracingModeValue();
        }

        /// <summary>
        /// On music slider change event.
        /// </summary>
        public void OnMusicSliderChange()
        {
            this.SetMusicValue(this.musicSlider.value == 1 ? false : true);
        }

        /// <summary>
        /// On sfx slider change event.
        /// </summary>
        public void OnSFXSliderChange()
        {
            this.SetSFXValue(this.sfxSlider.value == 1 ? false : true);
        }

        /// <summary>
        /// Toggles the tracing mode
        /// </summary>
        public void ToggleTracinggModeChange()
        {
            //toggle the int value
            if (this.tracingMode == ShapesManager.TracingMode.FILL)
            {
                this.tracingMode = ShapesManager.TracingMode.LINE;
            }
            else
            {
                this.tracingMode = ShapesManager.TracingMode.FILL;
            }

            this.SetTracingModeValue();
            DataManager.SaveTracingModeValue(this.tracingMode == ShapesManager.TracingMode.FILL ? 0 : 1);

            foreach (KeyValuePair<string, ShapesManager> shapesManager in ShapesManager.shapesManagers)
            {
                shapesManager.Value.tracingMode = this.tracingMode;
            }
        }

        /// <summary>
        /// Set up the music value.
        /// </summary>
        /// <param name="mute">whether to mute or not.</param>
        private void SetMusicValue(bool mute)
        {
            AudioSources.instance.MusicAudioSource().mute = mute;
            this.musicSlider.value = (mute == true ? 0 : 1);
            DataManager.SaveMusicMuteValue(mute == true ? 1 : 0);
        }

        /// <summary>
        /// Set up the SFX value.
        /// </summary>
        /// <param name="value">Value.</param>
        private void SetSFXValue(bool mute)
        {
            AudioSources.instance.SFXAudioSource().mute = mute;
            this.sfxSlider.value = (mute == true ? 0 : 1);
            DataManager.SaveSFXMuteValue(mute == true ? 1 : 0);
        }

        /// <summary>
        /// Set up the Tracing Mode value.
        /// </summary>
        private void SetTracingModeValue()
        {
            if (this.tracingMode == ShapesManager.TracingMode.FILL)
            {//Fill
                this.tracingModeButton.GetComponent<Image>().sprite = this.tracingModeFill;
            }
            else
            {//Line
                this.tracingModeButton.GetComponent<Image>().sprite = this.tracingModeLine;
            }
        }
    }
}
