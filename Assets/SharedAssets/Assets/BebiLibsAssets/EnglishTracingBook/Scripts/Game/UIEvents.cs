
using IndieStudio.EnglishTracingBook.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BebiLibs;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;


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
    public class UIEvents : MonoBehaviour
    {
        /// <summary>
        /// Static instance of this class.
        /// </summary>
        /// 
        public static UIEvents instance;

        public SharedSceneResources sharedSceneResources;

        //Set of dialogs
        private Dialog renewHelpBoosterDialog;

        /// <summary>
        /// A Unity event.
        /// </summary>
        private readonly UnityEvent unityEvent = new UnityEvent();

        private ITracingExtension IResumable;

        void Awake()
        {
            this.IResumable = FindObjectsOfType<MonoBehaviour>().OfType<ITracingExtension>().ToArray().FirstOrDefault();

            if (instance == null)
            {
                instance = this;
            }

            GameObject temp;

            temp = GameObject.Find("RenewHelpBoosterDialog");
            if (temp != null)
            {
                this.renewHelpBoosterDialog = temp.GetComponent<Dialog>();
            }
        }

        public void ActivateLettersGame()
        {
            if (isGameLoaded == true) return;
            isGameLoaded = true;

            AudioSources.instance.SetUpMuteValues();
            //SharedSceneEvents.Invoke_FromTracingLobbyToTracingGame();
            if (string.IsNullOrEmpty("ComboShapesManager"))
            {
                Debug.LogError("Empty Shapes Manager Reference in the Button Component");
                return;
            }
            ShapesManager shapesManager = ShapesManager.shapesManagers["ComboShapesManager"];
            ShapesManager.shapesManagerReference = "ComboShapesManager";
            if (shapesManager == null)
            {
                Debug.LogWarning("ComboShapesManager is null");
                return;
            }
            if (shapesManager.shapes[0].isLocked && !shapesManager.testMode)
            {
                return;
            }

            ManagerSounds.PlayEffect("fx_successhigh1");
        }

        public void ActivateNumberGame()
        {
            if (isGameLoaded == true) return;
            isGameLoaded = true;

            AudioSources.instance.SetUpMuteValues();
            //SharedSceneEvents.Invoke_FromTracingLobbyToTracingGame();
            if (string.IsNullOrEmpty("NShapesManager"))
            {
                Debug.LogError("Empty Shapes Manager Reference in the Button Component");
                return;
            }
            ShapesManager shapesManager = ShapesManager.shapesManagers["NShapesManager"];
            ShapesManager.shapesManagerReference = "NShapesManager";
            if (shapesManager == null)
            {
                Debug.LogWarning("NShapesManager is null");
                return;
            }
            if (shapesManager.shapes[0].isLocked && !shapesManager.testMode)
            {
                return;
            }

            ManagerSounds.PlayEffect("fx_successhigh1");
        }
        public void ActivateShapesGame()
        {
            if (isGameLoaded == true) return;
            isGameLoaded = true;

            AudioSources.instance.SetUpMuteValues();
            //SharedSceneEvents.Invoke_FromTracingLobbyToTracingGame();
            if (string.IsNullOrEmpty("GShapeManager"))
            {
                Debug.LogError("Empty Shapes Manager Reference in the Button Component");
                return;
            }
            ShapesManager shapesManager = ShapesManager.shapesManagers["GShapeManager"];
            ShapesManager.shapesManagerReference = "GShapeManager";
            if (shapesManager == null)
            {
                Debug.LogWarning("GShapeManager is null");
                return;
            }
            if (shapesManager.shapes[0].isLocked && !shapesManager.testMode)
            {
                return;
            }
        }

        private static bool isGameLoaded = false;
        public void Trigger_ButtonClick_LoadGame(string shapesManagerReference)
        {
            if (isGameLoaded == true)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogError("Game loading already stated");
                }

                return;
            }
            isGameLoaded = true;

            AudioSources.instance.SetUpMuteValues();
            //SharedSceneEvents.Invoke_FromTracingLobbyToTracingGame();
            if (string.IsNullOrEmpty(shapesManagerReference))
            {
                Debug.LogError("Empty Shapes Manager Reference in the Button Component");
                return;
            }
            ShapesManager shapesManager = ShapesManager.shapesManagers[shapesManagerReference];
            ShapesManager.shapesManagerReference = shapesManagerReference;
            if (shapesManager == null)
            {
                Debug.LogWarning("shapesManagerReference is null");
                return;
            }
            if (shapesManager.shapes[0].isLocked && !shapesManager.testMode)
            {
                Debug.Log("Shape is locked");
                return;
            }

            ManagerSounds.PlayEffect("fx_successhigh1");
            // ManagerAnalyticsCustom.Analytics_GameStarted("tracing_" + shapesManagerReference.Substring(0, 1));
            // ManagerAnalytics.Analytics_SetScene("s_tg");

            ShapesManager.Shape.selectedShapeID = 0;
            this.LoadGameScene();
        }

        public void AlbumShapeEvent(TableShape tableShape)
        {
            if (tableShape == null)
            {
                return;
            }

            ShapesManager shapesManager = ShapesManager.GetCurrentShapesManager();

            if (shapesManager == null)
            {
                Debug.LogWarning("active shape manager is null");
                return;
            }

            if (shapesManager.shapes[tableShape.ID].isLocked && !shapesManager.testMode)
            {
                return;
            }

            ShapesManager.Shape.selectedShapeID = tableShape.ID;
            this.LoadGameScene();
        }

        public void PointerButtonEvent(Pointer pointer)
        {
            if (pointer == null)
            {
                return;
            }
            if (pointer.group != null)
            {
                ScrollSlider scrollSlider = GameObject.FindObjectOfType(typeof(ScrollSlider)) as ScrollSlider;
                if (scrollSlider != null)
                {
                    scrollSlider.DisableCurrentPointer();
                    FindObjectOfType<ScrollSlider>().currentGroupIndex = pointer.group.Index;
                    scrollSlider.GoToCurrentGroup();
                }
            }
        }

        public void UnloadGames()
        {
            isGameLoaded = false;
        }

        public void LoadMainScene()
        {
            isGameLoaded = false;
            //SharedSceneEvents.Invoke_FromTracingGameToTracingLobby();
            this.DestroyUserTraceInput();
            ManagerSounds.PlayEffect("fx_page17");
            GameManager.UnLoadScene("Game", () =>
            {
                Debug.Log("Unload Scene");
            });
            //this.StartCoroutine(SceneLoader.instance.LoadScene("Main"));
        }

        public void LoadGameScene()
        {
            Loader.Show();
            BebiLibs.SceneLoader.LoadScene("Game", LoadSceneMode.Additive, () =>
            {
                Loader.Hide();
            });
        }

        public void LoadSettingsScene()
        {
            Loader.Show();
            BebiLibs.SceneLoader.LoadScene("Settings", LoadSceneMode.Additive, () =>
            {
                Loader.Hide();
            });
        }

        /// <summary>
        /// Loads scene coroutine.
        /// </summary>
        public IEnumerator OnSceneLoad(string sceneName)
        {
            this.gameObject.SetActive(true);

            yield return 0;
            Loader.Hide();
        }

        public void LoadAlbumScene()
        {
            if (UserTraceInput.instance != null)
            {
                this.LoadUserInputScene();
            }
            //Load Album scene based on current shapesManagerReference
            else if (ShapesManager.GetCurrentShapesManager() != null)
            {
                Loader.Show();
                string sceneName = ShapesManager.GetCurrentShapesManager().sceneName;
                BebiLibs.SceneLoader.LoadScene(sceneName, LoadSceneMode.Additive, () =>
                {
                    Loader.Hide();
                });
            }
            else
            {
                Debug.LogWarning("CurrentShapesManager  is null");
            }
        }

        public void LoadAlbumScene(string shapesManagerReference)
        {
            //Load Album scene based on given shapesManagerReference
            if (string.IsNullOrEmpty(shapesManagerReference))
            {
                Debug.LogError("Empty Shapes Manager Reference in the Button Component");
                return;
            }
            ShapesManager shapesManager = ShapesManager.shapesManagers[shapesManagerReference];
            ShapesManager.shapesManagerReference = shapesManagerReference;

            Loader.Show();
            string sceneName = shapesManager.sceneName;
            BebiLibs.SceneLoader.LoadScene(sceneName, LoadSceneMode.Additive, () =>
            {
                Loader.Hide();
            });
        }

        public void LoadUserInputScene()
        {
            Loader.Show();
            BebiLibs.SceneLoader.LoadScene("UserInput", LoadSceneMode.Additive, () =>
            {
                Loader.Hide();
            });
        }

        private void DestroyUserTraceInput()
        {
            if (UserTraceInput.instance != null)
            {
                UserTraceInput.instance.DestroyReference();
            }
        }

        public void NextClickEvent()
        {
            if (IResumable != null) IResumable.Trigger_ButtonClick_NextShape(true);
            else GameManager.instance.Trigger_ButtonClick_NextShape(true);
        }

        public void PreviousClickEvent()
        {
            if (IResumable != null) return;
            else GameManager.instance.Trigger_ButtonClickPreviousShape(true);
        }

        public void SpeechClickEvent()
        {
            if (IResumable != null) IResumable.Spell();
            else GameManager.instance.Spell();
        }

        public void ResetShape()
        {
            // if (!GameManager.instance.shape.completed)
            // {
            //     GameManager.instance.DisableGameManager();
            //     GameObject.Find("ResetShapeConfirmDialog").GetComponent<Dialog>().Show(false);
            // }
            //else
            //{
            //  GameManager.instance.ResetShape();
            //}

            if (IResumable != null) IResumable.ResetShape();
            else GameManager.instance.ResetShape();
        }



        public void PencilClickEvent(Pencil pencil)
        {
            if (pencil == null || (GameManager.instance == null && IResumable == null)) return;

            if (IResumable != null)
            {
                pencil.EnableSelection();
            }
            else if (GameManager.instance != null)
            {
                GameManager.instance.SetShapeOrderColor();
                pencil.EnableSelection();
            }
        }

        public void HelpUserBoosterClick(Booster booster)
        {
            if (booster == null
                || (GameManager.instance != null && !GameManager.instance.isRunning)
                || (this.IResumable != null && this.IResumable.IsRunning)) return;

            if (booster.GetValue() == 0)
            {
                AudioSources.instance.PlayLockedSFX();
                this.ShowRenewHelpBoosterDialog();
                return;
            }

            booster.DecreaseValue();

            if (this.IResumable != null) this.IResumable.HelpUser();
            else GameManager.instance.HelpUser();
        }

        public void RenewHelpBooster(Booster booster)
        {
            if (booster == null) return;

            if (this.IResumable != null) this.IResumable.Resume();
            else GameManager.instance.Resume();

            this.renewHelpBoosterDialog.Hide(true);

            this.renewHelpBoosterDialog.transform.Find("YesButton").GetComponent<Button>().interactable = false;

            this.unityEvent.RemoveAllListeners();
            this.unityEvent.AddListener(() => booster.ResetValue());
            this.unityEvent.AddListener(() => this.renewHelpBoosterDialog.transform.Find("YesButton").GetComponent<Button>().interactable = true);

        }

        public void ResetShapeClickEvent()
        {
            if (this.IResumable != null) this.IResumable.ResetShape();
            else GameManager.instance.ResetShape();
        }


        public void ShowRenewHelpBoosterDialog()
        {
            if (this.IResumable != null) this.IResumable.Pause();
            else GameManager.instance.Pause();

            this.renewHelpBoosterDialog.Show(true);
        }

        public void ResetGame()
        {
            DataManager.ResetGame();
        }

        public void LeaveApp()
        {
            Application.Quit();
        }
    }
}