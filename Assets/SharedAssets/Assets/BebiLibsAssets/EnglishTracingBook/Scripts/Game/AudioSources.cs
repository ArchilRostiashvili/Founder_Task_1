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
    public class AudioSources : MonoBehaviour
    {
        /// <summary>
        /// The audio sources references.
        /// First Audio Souce used for the music
        /// Second Audio Souce used for the sound effects
        /// </summary>
        private AudioSource[] audioSources;

        //AudioClips references

        /// <summary>
        /// Background music
        /// </summary>
        public AudioClip backgroundMusic;

        /// <summary>
        /// Button Click SFX
        /// </summary>
        public AudioClip buttonClickSFX;

        /// <summary>
        /// The bubble sound effect.
        /// </summary>
        public AudioClip bubbleSFX;

        /// <summary>
        /// The completed sound effect.
        /// </summary>
        public AudioClip completedSFX;

        /// <summary>
        /// The correct sound effect.
        /// </summary>
        public AudioClip correctSFX;

        /// <summary>
        /// The wrong sound effect.
        /// </summary>
        public AudioClip wrongSFX;

        /// <summary>
        /// The locked sound effect.
        /// </summary>
        public AudioClip lockedSFX;

        /// <summary>
        /// Star sound effect.
        /// </summary>
        public AudioClip starSFX;

        /// <summary>
        /// Drop star sound effect
        /// </summary>
        public AudioClip dropStarSFX;

        /// <summary>
        /// This Gameobject defined as a Singleton.
        /// </summary>
        public static AudioSources instance;

        // Use this for initialization
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                this.audioSources = this.GetComponents<AudioSource>();
                this.SetUpMuteValues();
                //DontDestroyOnLoad(this.gameObject);
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        void Start()
        {
            //Play the background music clip on start
            this.PlayBackgroundMusic();
        }

        private void OnDestroy()
        {
            instance = null;
        }

        /// <summary>
        /// Set up the mute values for sfx, music audio sources.
        /// </summary>
        public void SetUpMuteValues()
        {
            bool muteAudio = PlayerPrefs.GetInt("SoundOnEffects", 1) == 1 ? true : false;
            bool muteBackground = PlayerPrefs.GetInt("SoundOnBackgrounds", 1) == 1 ? true : false;

            this.MusicAudioSource().mute = !muteBackground;
            this.SFXAudioSource().mute = !muteAudio;
        }

        /// <summary>
        /// Returns the Audio Source of the Music.
        /// </summary>
        /// <returns>The Audio Source of the Music.</returns>
        public AudioSource MusicAudioSource()
        {
            return this.audioSources[0];
        }


        /// <summary>
        /// Returns the Audio Source of the Sound Effects.
        /// </summary>
        /// <returns>The Audio Source of the Sound Effects.</returns>
        public AudioSource SFXAudioSource()
        {
            return this.audioSources[1];
        }

        /// <summary>
        /// Play the given SFX clip.
        /// </summary>
        /// <param name="clip">The Clip reference.</param>
        /// <param name="Stop Current Clip">If set to <c>true</c> stop current clip.</param>
        public void PlaySFXClip(AudioClip clip, bool stopCurrentClip)
        {
            if (clip == null)
            {
                return;
            }
            if (stopCurrentClip)
            {
                this.SFXAudioSource().Stop();
            }
            this.SFXAudioSource().PlayOneShot(clip);
        }


        /* List of play methods */

        public void PlayButtonClickSFX()
        {
            this.PlaySFXClip(this.buttonClickSFX, false);
        }

        public void PlayBackgroundMusic()
        {
            this.MusicAudioSource().clip = this.backgroundMusic;
            this.MusicAudioSource().Play();
        }

        public void PlayBubbleSFX()
        {
            this.PlaySFXClip(this.bubbleSFX, false);
        }

        public void PlayCompletedSFX()
        {
            this.PlaySFXClip(this.completedSFX, false);
        }

        public void PlayCorrectSFX()
        {
            this.PlaySFXClip(this.correctSFX, false);
        }

        public void PlayWrongSFX()
        {
            this.PlaySFXClip(this.wrongSFX, true);
        }

        public void PlayLockedSFX()
        {
            this.PlaySFXClip(this.lockedSFX, false);
        }

        public void PlayStarSFX()
        {
            this.PlaySFXClip(this.starSFX, false);
        }

        public void PlayDropStarSFX()
        {
            this.PlaySFXClip(this.dropStarSFX, false);
        }
    }
}