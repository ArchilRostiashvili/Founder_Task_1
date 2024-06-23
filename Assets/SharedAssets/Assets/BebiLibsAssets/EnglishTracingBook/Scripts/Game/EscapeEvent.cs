using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
    /// <summary>
    /// Escape or Back event
    /// </summary>

    public interface ITracingExtension
    {
        public CompoundShape CompoundShape { get; }
        bool IsRunning { get; }
        bool AnimateNumbersOnStart { get; }
        GameObject LinePrefab { get; }

        public void Resume();
        void Trigger_ButtonClick_NextShape(bool v);
        void Spell();
        void ResetShape();
        void HelpUser();
        void Pause();
    }


    [DisallowMultipleComponent]
    public class EscapeEvent : MonoBehaviour
    {
        /// <summary>
        /// On escape/back event
        /// </summary>
        public UnityEvent escapeEvent;
        private ITracingExtension _resumable;

        private void Awake()
        {
            _resumable = FindObjectsOfType<MonoBehaviour>().OfType<ITracingExtension>().ToArray().FirstOrDefault();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.OnEscapeClick();
            }
        }

        /// <summary>
        /// On Escape click event.
        /// </summary>
        public void OnEscapeClick()
        {
            bool visibleDialogFound = this.HideVisibleDialogs();

            if (visibleDialogFound)
            {
                return;
            }
            this.escapeEvent.Invoke();
        }

        /// <summary>
        /// Hide the visible dialogs.
        /// </summary>
        /// <returns><c>true</c>, if visible a dialog was visible, <c>false</c> otherwise.</returns>
        private bool HideVisibleDialogs()
        {
            bool visibleDialogFound = false;

            Dialog[] dialogs = GameObject.FindObjectsOfType<Dialog>();
            if (dialogs != null)
            {
                foreach (Dialog d in dialogs)
                {
                    if (d.visible)
                    {
                        if (d.name == "ResetShapeConfirmDialog" || d.name == "RenewHelpBoosterDialog")
                        {
                            //Do not forget to resume game manager, on escape event for this dialog
                            if (_resumable != null) _resumable.Resume();
                            else GameManager.instance.Resume();
                        }

                        d.Hide(true);
                        visibleDialogFound = true;
                    }
                }
            }
            return visibleDialogFound;
        }
    }
}