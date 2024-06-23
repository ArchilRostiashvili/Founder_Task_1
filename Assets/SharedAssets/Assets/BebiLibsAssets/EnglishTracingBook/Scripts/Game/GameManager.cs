using DG.Tweening;
using IndieStudio.EnglishTracingBook.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using BebiLibs;
using ABC_OLD;
/*
 * English Tracing Book Package
 *
 * @License		      Unity Asset Store EULA https://unity3d.com/legal/as_terms
 * @Author		      Indie Studio - Baraa Nasser
 * @Website		      https://indiestd.com
 * @Asset Store       https://assetstore.unity.com/publishers/9268
 * @Unity Connect     https://connect.unity.com/u/5822191d090915001dbaf653/column
 * @Email		      info@indiestd.com
 *
 */

namespace IndieStudio.EnglishTracingBook.Game
{
    [DisallowMultipleComponent]
    public class GameManager : MonoBehaviour
    {
        public Gradient brushColor = new Gradient();

        /// <summary>
        /// Whether the script is running or not.
        /// </summary>
        public bool isRunning = true;

        /// <summary>
        /// Whether to hide shape's image on complete or not
        /// </summary>
        public bool hideShapeImageOnComplete;

        /// <summary>
        /// Whether to move/animate numbers of the paths on Start or not
        /// </summary>
        public bool animateNumbersOnStart = true;

        /// <summary>
        /// Whether to enable Fill Qurater Angle Restriction or not
        /// </summary>
        private readonly bool quarterRestriction = true;

        public Canvas canvasUI;

        /// <summary>
        /// The sentence's prefab (used to create custom user input)
        /// </summary>
        public GameObject sentencePrefab;

        /// <summary>
        /// The line's prefab
        /// </summary>
        public GameObject linePrefab;

        /// <summary>
        /// The PathObjectMove of the Hand
        /// </summary>
        public PathObjectMove handPOM;

        /// <summary>
        /// The reset confirm dialog
        /// </summary>
       //public Dialog resetConfirmDialog;

        /// <summary>
        /// The next/previous button transform
        /// </summary>
        public Transform nextButton;
        public Transform previousButton;

        /// <summary>
        /// The shape order.
        /// </summary>

        public TMP_Text shapeOrder;

        /// <summary>
        /// The write shape name text.
        /// </summary>
        public TMP_Text writeText_lower;
        public TMP_Text writeText_upper;
        public TMP_Text descriptionText;

        /// <summary>
        /// The tracing path that the user selected (currently pressed).
        /// </summary>
        private TracingPath selectedTracingPath;

        /// <summary>
        /// The shape parent.
        /// </summary>
        public Transform shapeParent;

        /// <summary>
        /// The shape reference.
        /// </summary>
        [HideInInspector]
        public Shape shape;

        /// <summary>
        /// The click postion.
        /// </summary>
        private Vector3 clickPostion;

        /// <summary>
        /// The direction between click and shape.
        /// </summary>
        private Vector2 direction;

        /// <summary>
        /// The current angle , angleOffset and fill amount.
        /// </summary>
        private float angle, angleOffset, fillAmount;

        /// <summary>
        /// The clock wise sign.
        /// </summary>
        private float clockWiseSign;

        /// <summary>
        /// The hand reference.
        /// </summary>
        public Transform hand;

        /// <summary>
        /// The default size of the cursor.
        /// </summary>
        private Vector3 cursorDefaultSize;

        /// <summary>
        /// The click size of the cursor.
        /// </summary>
        private Vector3 cursorClickSize;

        /// <summary>
        /// The target quarter of the radial fill.
        /// </summary>
        private float targetQuarter;

        /// <summary>
        /// The complete effect.
        /// </summary>
        public ParticleSystem winEffect;
        public ParticleSystem startParticle;
        public ParticleSystem ConfettiParticle;

        /// <summary>
        /// UI Parent Gameobject that contains the Timer
        /// </summary>
        public Transform timerPanel;

        /// <summary>
        /// The shape picture image reference (used to show the picture image  of the selected shape).
        /// </summary>
        public Image shapePicture;

        /// <summary>
        /// The hit2d reference.
        /// </summary>
        private RaycastHit2D hit2d;
        private bool _isScrolling = false;

        /// <summary>
        /// The compound shape reference.
        /// </summary>
        [HideInInspector]
        public CompoundShape compoundShape;

        /// <summary>
        /// Hand Sparkes effect while moving or tracing
        /// </summary>
        public ParticleSystem handSparkles;

        /// <summary>
        /// Temp Emission Module for the ParticleSystem
        /// </summary>
        private ParticleSystem.EmissionModule tempEmission;

        /// <summary>
        /// Static instance of this class.
        /// </summary>
        public static GameManager instance;

        public GameObject currentShape;
        public GameObject nextShape;

        public AnimationCurve easAnimation;

        private string[] _arrayCorrectPathSounds = { "fx_cartoonsuccess1", "fx_successhigh5" };
        private string[] _arrayCorrectShapeSounds = { "fx_tx_wow", "fx_tx_nice ", "fx_tx_good_job", "fx_tx_great", "fx_tx_smart" };

        void Awake()
        {
            //Initiate GameManager instance 
            if (instance == null)
            {
                instance = this;
            }
        }

        void Start()
        {
            _isScrolling = false;
            this.Init();
        }

        /// <summary>
        /// Init references/values
        /// </summary>
        private void Init()
        {
            //Initiate values and setup the references
            this.cursorDefaultSize = this.hand.transform.localScale;
            this.cursorClickSize = this.cursorDefaultSize / 1.2f;

            if (this.handSparkles != null)
            {
                this.tempEmission = this.handSparkles.emission;
                this.tempEmission.enabled = false;
            }

            if (string.IsNullOrEmpty(ShapesManager.shapesManagerReference) && UserTraceInput.instance == null)
            {
                Debug.LogErrorFormat("You have to start the game from the <b>Main</b> scene");
                return;
            }

            if (this.handPOM == null)
            {
                this.handPOM = GameObject.Find("TracingHand").GetComponent<PathObjectMove>();
            }


            // this.winEffect.gameObject.SetActive(false);
            this.startParticle.gameObject.SetActive(false);
            this.ConfettiParticle.gameObject.SetActive(false);
            this.ResetTargetQuarter();
            this.SetShapeOrderColor();
            this.CreateCarousel(false, false);
        }

        // Update is called once per frame
        void Update()
        {
            //Game Logic is here

            if (!this.isRunning)
            {
                return;
            }

            this.HandleInput();
        }

        /// <summary>
        /// Handle user's input
        /// </summary>
        private void HandleInput()
        {
            this.DrawHand(this.GetCurrentPlatformClickPosition(Camera.main));
            this.DrawBrightEffect(this.GetCurrentPlatformClickPosition(Camera.main));

            if (this.shape == null)
            {
                return;
            }

            if (this.shape.completed)
            {
                return;
            }

            if (CrossPlatformInput.InputHold)
            {
                //if (!shape.completed)
                //brightEffect.gameObject.SetActive(false);

                this.hit2d = Physics2D.Raycast(this.GetCurrentPlatformClickPosition(Camera.main), Vector2.zero);
                if (this.hit2d.collider != null)
                {
                    if (this.handSparkles != null)
                    {
                        this.tempEmission.enabled = true;
                    }

                    if (this.hit2d.transform.tag == "Point")
                    {
                        this.OnPointHitCollider(this.hit2d);

                        if (CrossPlatformInput.InputClick)
                        {
                            this.shape.CancelInvoke();
                            this.DisableHandTracing();
                            this.ShowUserTouchHand();
                        }
                    }
                    else if (this.hit2d.transform.tag == "Collider")
                    {
                        if (CrossPlatformInput.InputClick)
                        {
                            this.DisableHandTracing();
                            this.ShowUserTouchHand();
                        }
                    }
                }
            }
            else if (CrossPlatformInput.InputUp)
            {
                if (this.handSparkles != null)
                    this.tempEmission.enabled = false;

                //brightEffect.gameObject.SetActive(false);
                this.HideUserTouchHand();
                this.StartAutoTracing(this.shape, 6);
                this.ResetPath();
            }

            if (this.selectedTracingPath == null)
            {
                return;
            }

            if (this.selectedTracingPath.completed)
            {
                return;
            }

            if (ShapesManager.GetCurrentShapesManager().enableTracingLimit)
            {
                this.hit2d = Physics2D.Raycast(this.GetCurrentPlatformClickPosition(Camera.main), Vector2.zero);
                if (this.hit2d.collider == null)
                {
                    // AudioSources.instance.PlayWrongSFX();
                    ManagerSounds.PlayEffect("fx_wrong7");
                    this.ResetPath();
                    return;
                }
            }

            this.TracePath();
        }

        /// <summary>
        /// On Point collider hit.
        /// </summary>
        /// <param name="hit2d">Hit2d.</param>
        private void OnPointHitCollider(RaycastHit2D hit2d)
        {
            this.selectedTracingPath = hit2d.transform.GetComponentInParent<TracingPath>();

            if (this.selectedTracingPath == null)
            {
                Debug.Log("No Tracing");
                return;
            }

            this.selectedTracingPath.DisableCurrentPoint();
            this.selectedTracingPath.tracedPoints++;
            this.selectedTracingPath.EnableCurrentPoint();

            if (this.selectedTracingPath.completed || !this.shape.IsCurrentPath(this.selectedTracingPath))
            {
                this.ReleasePath();
            }
            else
            {
                this.selectedTracingPath.StopAllCoroutines();
                this.selectedTracingPath.fillImage.color = this.brushColor.colorKeys[0].color;
            }

            this.selectedTracingPath.curve.Init();

            if (!this.selectedTracingPath.shape.enablePriorityOrder)
            {
                this.shape = this.selectedTracingPath.shape;
            }
        }

        /// <summary>
        /// Get the current platform click position in the world space.
        /// </summary>
        /// <returns>The current platform click position.</returns>
        private Vector3 GetCurrentPlatformClickPosition(Camera camera)
        {
            Vector3 clickPosition = Vector3.zero;

            if (Application.isMobilePlatform)
            {//current platform is mobile
                if (Input.touchCount != 0)
                {
                    Touch touch = Input.GetTouch(0);
                    clickPosition = touch.position;
                }
            }
            else
            {//others
                clickPosition = Input.mousePosition;
            }

            clickPosition = camera.ScreenToWorldPoint(clickPosition);//get click position in the world space
            clickPosition.z = 0;
            return clickPosition;
        }



        public void CreateCarousel(bool locked, bool moveRight)
        {
            if (_isScrolling == true) return;
            _isScrolling = true;

            this.nextButton.gameObject.SetActive(false);
            this.previousButton.gameObject.SetActive(false);
            this.isRunning = false;

            if (UserTraceInput.instance == null)
            {
                Timer.instance.Reset();
            }
            // this.winEffect.gameObject.SetActive(false);
            this.startParticle.gameObject.SetActive(false);
            //this.resetConfirmDialog.Hide(true);

            if (this.nextShape != null)
            {
                GameObject.DestroyImmediate(this.nextShape);
            }

            this.nextShape = this.currentShape;

            if (this.nextShape != null)
            {
                float nextMovePosX = !moveRight ? -2000 : 2000;
                this.compoundShape = this.nextShape.GetComponentInChildren<CompoundShape>();
                if (this.compoundShape != null)
                {
                    nextMovePosX = !moveRight ? -2600 : 2600;
                }

                this.nextShape.transform.DOLocalMoveX(nextMovePosX, 0.6f).SetEase(this.easAnimation).OnComplete(() =>
                {
                    _isScrolling = false;
                });
            }
            try
            {
                GameObject shapePrefab = ShapesManager.GetCurrentShapesManager().GetCurrentShape().prefab;
                this.currentShape = Instantiate(shapePrefab, Vector2.zero, Quaternion.identity, this.transform) as GameObject;
                this.currentShape.transform.SetParent(this.shapeParent);
                this.currentShape.transform.localPosition = !moveRight ? new Vector3(2000, -30, 0) : new Vector3(-2000, -30, 0);
                this.currentShape.name = shapePrefab.name;

                this.compoundShape = GameObject.FindObjectOfType<CompoundShape>();
                if (this.compoundShape != null)
                {
                    Vector3 correntPosition = this.currentShape.transform.localPosition;
                    this.compoundShape.transform.localPosition = !moveRight ? new Vector3(2600, -20, 0) : new Vector3(-2600, 20, 0);

                    float aspect = CommonUtil.ShapeGameAspectRatio();
                    // //-0.0208x^3+0.1547x^2+0.3285x+0.4784
                    // //-0.521x^3+1.4069x^2+0.5666x-0.5697
                    //-0.174x^3+0.5751x^2+0.4564x+0.2171

                    float scale = CommonUtil.Cubic(aspect, -0.174f, 0.5752f, 0.4564f, 0.2171f);
                    scale = Mathf.Clamp(scale, 1.4f, 2.3f);

                    //this.currentShape.transform.localScale = Vector3.one * this.compoundShape.scaleFactor * CommonUtil.ShapeGameAspectRatio();
                    this.currentShape.transform.localScale = Vector3.one * scale;
                }
                else
                {
                    Shape shape = this.currentShape.GetComponentInChildren<Shape>();
                    //  //(‑(0.1244*​x^​3))+​0.1574*​x^​2+​0.3677*​x+​0.5252
                    //0.176x^3-0.585x^2+0.31x+1.2989
                    float aspect = CommonUtil.ShapeGameAspectRatio();
                    float scale = CommonUtil.Cubic(aspect, 0.176f, -0.585f, 0.31f, 1.2989f);
                    scale = Mathf.Clamp(scale, 0.75f, 1.1f);

                    shape.content.localScale = Vector3.one * scale;
                }


                Camera camera = Camera.main;
                float safeOffset = (camera.ScreenToWorldPoint(Screen.safeArea.min) - camera.ViewportToWorldPoint(Vector3.zero)).x;
                //Debug.Log(safeOffset);
                this.currentShape.transform.DOLocalMoveX(Screen.safeArea.min.x, 0.6f).SetEase(this.easAnimation).OnComplete(() =>
                  {
                      if (locked == false)
                      {
                          this.StartTrace(this.currentShape);
                      }
                      _isScrolling = false;
                  });
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        //My Method
        private void StartTrace(GameObject shapeGameObject)
        {
            try
            {
                if (UserTraceInput.instance != null)
                {
                    shapeGameObject = Instantiate(this.sentencePrefab, Vector3.zero, Quaternion.identity, this.transform) as GameObject;
                    shapeGameObject.transform.SetParent(this.shapeParent);
                    shapeGameObject.transform.localPosition = this.sentencePrefab.transform.localPosition;

                    var cs = shapeGameObject.GetComponent<CompoundShape>();
                    cs.text = UserTraceInput.instance.text;
                    cs.Generate();
                    this.shapeOrder.gameObject.SetActive(false);
                    this.shapePicture.gameObject.SetActive(false);
                    this.nextButton.gameObject.SetActive(false);
                    this.previousButton.gameObject.SetActive(false);
                }
                else
                {
                    this.shapeOrder.text = (ShapesManager.Shape.selectedShapeID + 1) + "/" + ShapesManager.GetCurrentShapesManager().shapes.Count;
                    ShapesManager.GetCurrentShapesManager().lastSelectedGroup = ShapesManager.Shape.selectedShapeID;
                }

                this.compoundShape = GameObject.FindObjectOfType<CompoundShape>();

                if (this.compoundShape != null)
                {
                    //shapeGameObject.transform.localScale = Vector3.one * this.compoundShape.scaleFactor * CommonUtil.ShapeGameAspectRatio();
                    this.shape = this.compoundShape.shapes[0];
                }
                else
                {
                    this.shape = GameObject.FindObjectOfType<Shape>();
                }

                this.StartAutoTracing(this.shape, 0.5f);
                this.Spell();
            }
            catch (System.Exception ex)
            {
                //Catch the exception or display an alert
                Debug.LogError(ex.Message);
            }

            if (this.shape == null)
            {
                return;
            }
            //Set up write text/label and
            //Setup rest message in the Rest Confirm Dialog
            if (UserTraceInput.instance == null)
            {
                //CommonUtil.FindChildByTag(this.resetConfirmDialog.transform, "Message").GetComponent<Text>().text = "Reset " + ShapesManager.GetCurrentShapesManager().shapeLabel + " " + this.shape.GetTitle() + " ?";
                string text = "<color=#ffee31>\"" + this.shape.GetTitle() + "\" </color>";
                string title = this.shape.GetTitle();
                if (char.IsUpper(title[0]) || int.TryParse(title, out int num))
                {
                    // this.writeText_lower.gameObject.SetActive(false);
                    // this.writeText_upper.gameObject.SetActive(true);
                    // this.writeText_upper.text = text;
                }
                else
                {
                    // this.writeText_lower.gameObject.SetActive(true);
                    // this.writeText_upper.gameObject.SetActive(false);
                    // this.writeText_lower.text = text;
                }
                this.descriptionText.text = "WRITE THE " + ShapesManager.GetCurrentShapesManager().shapeLabel.ToUpper();

                //Set up shape's picture
                this.shapePicture.sprite = ShapesManager.GetCurrentShapesManager().GetCurrentShape().picture;
                if (this.shapePicture.sprite == null)
                {
                    this.shapePicture.enabled = false;
                }
                else
                {
                    this.shapePicture.enabled = true;
                }
            }
            else
            {
                //CommonUtil.FindChildByTag(this.resetConfirmDialog.transform, "Message").GetComponent<Text>().text = "Reset  " + this.shape.GetTitle() + " ?";
                string title = this.shape.GetTitle();
                string text = "<color=#ffee31>\"" + this.shape.GetTitle() + "\" </color>";
                if (char.IsUpper(title[0]) || int.TryParse(title, out int num))
                {
                    // this.writeText_lower.gameObject.SetActive(false);
                    // this.writeText_upper.gameObject.SetActive(true);
                    // this.writeText_upper.text = text;
                }
                else
                {
                    // this.writeText_lower.gameObject.SetActive(true);
                    // this.writeText_upper.gameObject.SetActive(false);
                    // this.writeText_lower.text = text;
                }


                this.descriptionText.text = "WRITE THE TEXT";
            }

            this.EnableGameManager();
        }

        /// <summary>
        /// Create new shape.
        /// </summary>
        private void CreateShape()
        {
            Debug.Log("Create Shape");
            if (UserTraceInput.instance == null)
            {
                Timer.instance.Reset();
            }

            // this.winEffect.gameObject.SetActive(false);
            this.startParticle.gameObject.SetActive(false);
            //this.resetConfirmDialog.Hide(true);

            //nextButton.GetComponent<Animator>().SetBool("Select", false);

            CompoundShape currentCompoundShape = GameObject.FindObjectOfType<CompoundShape>();
            if (currentCompoundShape != null)
            {
                DestroyImmediate(currentCompoundShape.gameObject);
            }
            else
            {
                Shape shapeComponent = GameObject.FindObjectOfType<Shape>();
                Debug.Log(shapeComponent);
                if (shapeComponent != null)
                {
                    DestroyImmediate(shapeComponent.gameObject);
                }
            }

            try
            {
                GameObject shapeGameObject = null;
                if (UserTraceInput.instance != null)
                {
                    shapeGameObject = Instantiate(this.sentencePrefab, Vector3.zero, Quaternion.identity, this.transform) as GameObject;
                    shapeGameObject.transform.SetParent(this.shapeParent);
                    shapeGameObject.transform.localPosition = this.sentencePrefab.transform.localPosition;
                    var cs = shapeGameObject.GetComponent<CompoundShape>();
                    cs.text = UserTraceInput.instance.text;
                    cs.Generate();
                    this.shapeOrder.gameObject.SetActive(false);
                    this.shapePicture.gameObject.SetActive(false);
                    this.nextButton.gameObject.SetActive(false);
                    this.previousButton.gameObject.SetActive(false);
                    Debug.Log("Shape 1000");
                }
                else
                {
                    GameObject shapePrefab = ShapesManager.GetCurrentShapesManager().GetCurrentShape().prefab;
                    shapeGameObject = Instantiate(shapePrefab, Vector3.zero, Quaternion.identity, this.transform) as GameObject;
                    shapeGameObject.transform.SetParent(this.shapeParent);
                    shapeGameObject.transform.localPosition = shapePrefab.transform.localPosition;


                    shapeGameObject.transform.localScale = Vector3.one;
                    shapeGameObject.name = shapePrefab.name;

                    this.shapeOrder.text = (ShapesManager.Shape.selectedShapeID + 1) + "/" + ShapesManager.GetCurrentShapesManager().shapes.Count;
                    ShapesManager.GetCurrentShapesManager().lastSelectedGroup = ShapesManager.Shape.selectedShapeID;

                    Debug.Log("Shape 9999");
                }

                this.compoundShape = GameObject.FindObjectOfType<CompoundShape>();

                if (this.compoundShape != null)
                {//Scentence
                    shapeGameObject.transform.localScale = Vector3.one * this.compoundShape.scaleFactor * CommonUtil.ShapeGameAspectRatio();
                    this.shape = this.compoundShape.shapes[0];
                }
                else
                {//Shape
                    this.shape = GameObject.FindObjectOfType<Shape>();
                    this.shape.content.localScale = Vector3.one;
                    Debug.Log(CommonUtil.ShapeGameAspectRatio());
                    Debug.Log("Shape " + this.shape);
                }

                this.StartAutoTracing(this.shape, 0.5f);
                this.Spell();
            }
            catch (System.Exception ex)
            {
                //Catch the exception or display an alert
                Debug.LogError(ex.Message);
            }

            if (this.shape == null)
            {
                return;
            }

            //Set up write text/label and
            //Setup rest message in the Rest Confirm Dialog
            if (UserTraceInput.instance == null)
            {
                //CommonUtil.FindChildByTag(this.resetConfirmDialog.transform, "Message").GetComponent<Text>().text = "Reset " + ShapesManager.GetCurrentShapesManager().shapeLabel + " " + this.shape.GetTitle() + " ?";
                //this.writeText_lower.text = "Write the " + ShapesManager.GetCurrentShapesManager().shapeLabel.ToLower() + " <color=#ffee31>\"" + this.shape.GetTitle() + "\"</color>";

                //Set up shape's picture
                this.shapePicture.sprite = ShapesManager.GetCurrentShapesManager().GetCurrentShape().picture;
                if (this.shapePicture.sprite == null)
                {
                    this.shapePicture.enabled = false;
                }
                else
                {
                    this.shapePicture.enabled = true;
                }
            }
            else
            {
                //CommonUtil.FindChildByTag(this.resetConfirmDialog.transform, "Message").GetComponent<Text>().text = "Reset " + this.shape.GetTitle() + " ?";
                //this.writeText_lower.text = "Write the text <color=#ffee31>\"" + this.shape.GetTitle() + "\"</color>";
            }

            this.EnableGameManager();
        }

        /// <summary>
        /// Go to the Next shape.
        /// </summary>
        public void Trigger_ButtonClick_NextShape(bool hasSound)
        {
            if (hasSound)
            {
                ManagerSounds.PlayEffect("fx_cartoonjump1");
            }
            this.StopAllCoroutines();
            this.ConfettiParticle.gameObject.SetActive(false);
            if (ShapesManager.Shape.selectedShapeID >= 0 && ShapesManager.Shape.selectedShapeID + 1 < ShapesManager.GetCurrentShapesManager().shapes.Count)
            {
                //if (ShapesManager.Shape.selectedShapeID + 1 < ShapesManager.GetCurrentShapesManager().shapes.Count)
                //{
                //if (DataManager.IsShapeLocked(ShapesManager.Shape.selectedShapeID + 1, ShapesManager.GetCurrentShapesManager()) && !ShapesManager.GetCurrentShapesManager().testMode)
                //{
                //ShapesManager.Shape.selectedShapeID++;
                //this.CreateCarousel(true, false);
                //AudioSources.instance.PlayLockedSFX();
                //return;
                //}
                //}

                ShapesManager.Shape.selectedShapeID++;
                this.CreateCarousel(false, false);
            }
            else
            {
                ShapesManager.Shape.selectedShapeID = 0;
                this.CreateCarousel(false, false);
            }
        }

        /// <summary>
        /// Go to the previous shape.
        /// </summary>
        public void Trigger_ButtonClickPreviousShape(bool sound)
        {
            if (sound)
            {
                ManagerSounds.PlayEffect("fx_cartoonjump1");
            }
            this.StopAllCoroutines();
            if (ShapesManager.Shape.selectedShapeID > 0 && ShapesManager.Shape.selectedShapeID < ShapesManager.GetCurrentShapesManager().shapes.Count)
            {
                ShapesManager.Shape.selectedShapeID--;
                this.CreateCarousel(false, true);
            }
            else
            {
                ShapesManager.Shape.selectedShapeID = ShapesManager.GetCurrentShapesManager().shapes.Count - 1;
                this.CreateCarousel(false, true);
            }
        }

        /// <summary>
        /// Trace the current path
        /// </summary>
        private void TracePath()
        {
            if (ShapesManager.GetCurrentShapesManager().tracingMode == ShapesManager.TracingMode.FILL)
            {
                if (this.selectedTracingPath.fillMethod == TracingPath.FillMethod.Radial)
                {
                    this.RadialFill();
                }
                else if (this.selectedTracingPath.fillMethod == TracingPath.FillMethod.Linear)
                {
                    this.LinearFill();
                }
                else if (this.selectedTracingPath.fillMethod == TracingPath.FillMethod.Point)
                {
                    this.PointFill();
                }
            }
            else if (ShapesManager.GetCurrentShapesManager().tracingMode == ShapesManager.TracingMode.LINE)
            {
                this.DrawLine();
            }

            this.CheckPathComplete();
        }

        /// <summary>
        /// Radial fill tracing method.
        /// </summary>
        private void RadialFill()
        {
            this.clickPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            this.direction = this.clickPostion - this.selectedTracingPath.curve.centroid;

            this.angleOffset = 0;
            this.clockWiseSign = (this.selectedTracingPath.fillImage.fillClockwise ? 1 : -1);

            if (this.selectedTracingPath.fillImage.fillMethod == Image.FillMethod.Radial360)
            {
                if (this.selectedTracingPath.fillImage.fillOrigin == 0)
                {//Bottom
                    this.angleOffset = 0;
                }
                else if (this.selectedTracingPath.fillImage.fillOrigin == 1)
                {//Right
                    this.angleOffset = this.clockWiseSign * 90;
                }
                else if (this.selectedTracingPath.fillImage.fillOrigin == 2)
                {//Top
                    this.angleOffset = -180;
                }
                else if (this.selectedTracingPath.fillImage.fillOrigin == 3)
                {//left
                    this.angleOffset = -this.clockWiseSign * 90;
                }
            }
            else if (this.selectedTracingPath.fillImage.fillMethod == Image.FillMethod.Radial90)
            {
                if (this.selectedTracingPath.fillImage.fillOrigin == 0)
                {//Bottom Left like path in 'a'
                    this.angleOffset = this.selectedTracingPath.fillImage.fillClockwise ? -90 : 0;
                }
                else if (this.selectedTracingPath.fillImage.fillOrigin == 3)
                {//Bottom Right like path in 'a' horinoal flipped
                    this.angleOffset = this.selectedTracingPath.fillImage.fillClockwise ? 0 : -90;
                }
                else if (this.selectedTracingPath.fillImage.fillOrigin == 2)
                {//Top Right like path in 'a' vertial flipped
                    this.angleOffset = this.selectedTracingPath.fillImage.fillClockwise ? 90 : -180;
                }
                else if (this.selectedTracingPath.fillImage.fillOrigin == 1)
                {//Top Left like path in 'a' vertial and horizontal flipped
                    this.angleOffset = this.selectedTracingPath.fillImage.fillClockwise ? -180 : 90;
                }
            }

            this.angle = Mathf.Atan2(-this.clockWiseSign * this.direction.x, -this.direction.y) * Mathf.Rad2Deg + this.angleOffset;

            if (this.angle < 0)
                this.angle += 360;

            this.angle = Mathf.Clamp(this.angle, 0, 360);

            if (this.quarterRestriction)
            {
                if (!(this.angle >= 0 && this.angle <= this.targetQuarter))
                {
                    this.fillAmount = this.selectedTracingPath.fillImage.fillAmount = 0;
                    return;
                }

                if (this.angle >= this.targetQuarter / 2)
                {
                    this.targetQuarter += 90;
                }
                else if (this.angle < 45)
                {
                    this.targetQuarter = 90;
                }

                this.targetQuarter = Mathf.Clamp(this.targetQuarter, 90, 360);
            }

            this.fillAmount = Mathf.Abs(this.angle / 360.0f);
            this.selectedTracingPath.fillImage.fillAmount = this.fillAmount;
        }

        /// <summary>
        /// Linear fill tracing method.
        /// </summary>
        private void LinearFill()
        {
            this.clickPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Rect rect = CommonUtil.RectTransformToWorldSpace(selectedTracingPath.GetComponent<RectTransform>());

            Vector3 pos1 = this.selectedTracingPath.curve.GetFirstPoint().position, pos2 = this.selectedTracingPath.curve.GetLastPoint().position;
            pos1.z = pos2.z = 0;

            this.clickPostion.x = Mathf.Clamp(this.clickPostion.x, Mathf.Min(pos1.x, pos2.x), Mathf.Max(pos1.x, pos2.x));
            this.clickPostion.y = Mathf.Clamp(this.clickPostion.y, Mathf.Min(pos1.y, pos2.y), Mathf.Max(pos1.y, pos2.y));
            this.clickPostion.z = 0;

            this.fillAmount = Vector2.Distance(this.clickPostion, pos1) / Vector2.Distance(pos1, pos2);
            this.selectedTracingPath.fillImage.fillAmount = this.fillAmount;
        }

        /// <summary>
        /// Point fill tracing method.
        /// </summary>
        private void PointFill()
        {
            this.fillAmount = 1;
            this.selectedTracingPath.fillImage.fillAmount = 1;
        }

        /// <summary>
        /// Draw line tracing method
        /// </summary>
        private void DrawLine()
        {
            //Get Line component
            if (this.selectedTracingPath.line != null)
            {
                this.selectedTracingPath.line.SetColor(this.brushColor);
                //Add touch/click point into current line
                this.selectedTracingPath.line.AddPoint(this.selectedTracingPath.line.transform.InverseTransformPoint(this.GetCurrentPlatformClickPosition(Camera.main)));
                this.selectedTracingPath.line.BezierInterpolate(0.3f);
            }
        }

        /// <summary>
        /// Checks wehther path completed or not.
        /// </summary>
        private void CheckPathComplete()
        {
            if (this.selectedTracingPath.tracedPoints == this.selectedTracingPath.curve.points.Count)
            {
                if (ShapesManager.GetCurrentShapesManager().tracingMode == ShapesManager.TracingMode.FILL)
                { //Fill Tracing Mode
                    if (this.fillAmount >= this.selectedTracingPath.completeOffset)
                    {
                        this.selectedTracingPath.completed = true;
                        this.selectedTracingPath.AutoFill();
                    }
                }
                else if (ShapesManager.GetCurrentShapesManager().tracingMode == ShapesManager.TracingMode.LINE)
                {//Line Tracing Mode

                    if (Vector2.Distance(this.selectedTracingPath.curve.GetLastPoint().position, this.GetCurrentPlatformClickPosition(Camera.main)) < 0.5f)
                    {
                        this.selectedTracingPath.completed = true;
                    }
                }
            }

            if (this.selectedTracingPath.completed)
            {
                this.selectedTracingPath.AutoLineComplete();
                this.selectedTracingPath.OnComplete();
                if (this.selectedTracingPath.curve.centroid != null)
                {
                    this.startParticle.transform.position = this.selectedTracingPath.curve.centroid;
                }
                else if (this.selectedTracingPath.line.CenterPoint != null)
                {
                    this.startParticle.transform.position = this.selectedTracingPath.line.CenterPoint;
                }
                ManagerSounds.PlayEffect(_arrayCorrectPathSounds[Random.Range(0, _arrayCorrectPathSounds.Length)]);
                this.startParticle.gameObject.SetActive(true);
                this.startParticle.Play();
                this.ReleasePath();

                if (this.CheckShapeComplete())
                {
                    this.OnShapeComplete();
                }
                else
                {
                    //  AudioSources.instance.PlayCorrectSFX();
                }

                this.shape.ShowPathNumbers(this.shape.GetCurrentPathIndex());
            }
        }

        /// <summary>
        /// Check whether the shape completed or not.
        /// </summary>
        /// <returns><c>true</c>, if shape completed, <c>false</c> otherwise.</returns>
        private bool CheckShapeComplete()
        {
            bool shapeCompleted = true;
            foreach (TracingPath path in this.shape.tracingPaths)
            {
                if (!path.completed)
                {
                    shapeCompleted = false;
                    break;
                }
            }
            return shapeCompleted;
        }

        /// <summary>
        /// On shape completed event.
        /// </summary>
        private void OnShapeComplete()
        {

            this.shape.completed = true;

            bool allDone = true;

            List<Shape> shapes = new List<Shape>();

            if (this.compoundShape != null)
            {
                shapes = this.compoundShape.shapes;
                allDone = this.compoundShape.IsCompleted();

                if (!allDone)
                {
                    this.shape = this.compoundShape.shapes[this.compoundShape.GetCurrentShapeIndex()];
                    this.StartAutoTracing(this.shape, 1);
                }
            }
            else
            {
                shapes.Add(this.shape);
            }

            if (allDone)
            {
                if (this.handSparkles != null)
                    this.tempEmission.enabled = false;

                if (UserTraceInput.instance == null)
                {
                    this.SaveShapesData(shapes);
                }

                this.DisableHandTracing();
                this.HideUserTouchHand();
                //brightEffect.gameObject.SetActive(false);

                foreach (Shape s in shapes)
                {
                    if (this.hideShapeImageOnComplete)
                    {
                        //s.content.GetComponent<Image>().enabled = false;
                    }

                    s.animator.SetTrigger("Completed");
                }

                Timer.instance.Stop();

                // nextButton.GetComponent<Animator>().SetTrigger("Select");
                // this.winEffect.gameObject.SetActive(true);
                //AudioSources.instance.PlayCompletedSFX();
                ManagerSounds.PlayEffect("fx_cartoonsuccess2");
                ManagerTime.Delay(0.5f, () =>
                {
                    ManagerSounds.PlayEffect(_arrayCorrectShapeSounds[Random.Range(0, _arrayCorrectShapeSounds.Length)]);
                });
                this.ConfettiParticle.gameObject.SetActive(true);
                this.ConfettiParticle.Play();
                this.nextButton.gameObject.SetActive(true);
                this.previousButton.gameObject.SetActive(true);
                this.StopAllCoroutines();
                this.StartTimer(6f);
            }
            else
            {
                AudioSources.instance.PlayCorrectSFX();
                Debug.Log("here");
            }
        }

        public void StartTimer(float seconds)
        {
            this.StartCoroutine(timer());
            IEnumerator timer()
            {
                yield return new WaitForSeconds(seconds);
                this.Trigger_ButtonClick_NextShape(false);
            }
        }

        /// <summary>
        /// Reset the shape.
        /// </summary>
        public void ResetShape()
        {
            this.StopAllCoroutines();
            List<Shape> shapes = new List<Shape>();
            if (this.compoundShape != null)
            {
                shapes = this.compoundShape.shapes;
            }
            else
            {
                shapes.Add(this.shape);
            }

            // this.winEffect.gameObject.SetActive(false);
            this.startParticle.gameObject.SetActive(false);
            // nextButton.GetComponent<Animator>().SetBool("Select", false);

            this.DisableHandTracing();

            foreach (Shape s in shapes)
            {
                if (s == null)
                    continue;

                s.completed = false;
                s.content.GetComponent<Image>().enabled = true;
                s.animator.SetBool("Completed", false);
                s.CancelInvoke();
                TracingPath[] paths = s.GetComponentsInChildren<TracingPath>();
                foreach (TracingPath path in paths)
                {
                    path.Reset();
                }

                if (this.compoundShape == null)
                {
                    this.StartAutoTracing(s, 2);
                }
                else if (this.compoundShape.GetShapeIndexByInstanceID(s.GetInstanceID()) == 0)
                {
                    this.shape = this.compoundShape.shapes[0];
                    this.StartAutoTracing(this.shape, 2);
                }
            }

            this.ReleasePath();

            this.Spell();

            if (UserTraceInput.instance == null)
                Timer.instance.Reset();
        }

        /// <summary>
        /// Save the data of the shapes such as (stars,path colors,unlock next shape...) .
        /// </summary>
        private void SaveShapesData(List<Shape> shapes)
        {
            if (shapes == null)
            {
                return;
            }

            if (shapes.Count == 0)
            {
                return;
            }

            //unlock the next shape
            if (ShapesManager.Shape.selectedShapeID + 1 < ShapesManager.GetCurrentShapesManager().shapes.Count)
            {
                DataManager.SaveShapeLockedStatus(ShapesManager.Shape.selectedShapeID + 1, false, ShapesManager.GetCurrentShapesManager());
                ShapesManager.GetCurrentShapesManager().shapes[ShapesManager.Shape.selectedShapeID + 1].isLocked = false;
            }

            // Color tempColor = Colors.whiteColor;
            // // save the colors of the paths
            // int compundID = -1;
            // foreach (Shape s in shapes)
            // {
            //     if (this.compoundShape != null)
            //     {
            //         //ID or Index of the shape in the compound shape
            //         compundID = this.compoundShape.GetShapeIndexByInstanceID(s.GetInstanceID());
            //     }

            //     foreach (TracingPath p in s.tracingPaths)
            //     {
            //         tempColor = ShapesManager.GetCurrentShapesManager().tracingMode == ShapesManager.TracingMode.FILL ? p.fillImage.color : CommonUtil.GradientToColor(p.line.color);
            //         //DataManager.SaveShapePathColor(ShapesManager.Shape.selectedShapeID, compundID, p.from, p.to, tempColor, ShapesManager.GetCurrentShapesManager());
            //     }
            // }
        }

        /// <summary>
        /// Starts the auto tracing for the current path.
        /// </summary>
        /// <param name="s">Shape Reference.</param>
        public void StartAutoTracing(Shape s, float traceDelay)
        {
            if (s == null)
            {
                return;
            }

            //Stop current movement
            this.DisableHandTracing();

            int currentPathIndex = s.GetCurrentPathIndex();

            if (currentPathIndex < 0 || currentPathIndex > s.tracingPaths.Count - 1)
                return;

            //Hide Numbers for other shapes , if we have compound shape
            if (this.compoundShape != null)
            {
                foreach (Shape ts in this.compoundShape.shapes)
                {
                    if (s.GetInstanceID() != ts.GetInstanceID())
                        ts.ShowPathNumbers(-1);
                }
            }

            if (s.tracingPaths.Count != 0)
            {
                //Set up the curve , and set position of Hand to the first point
                this.handPOM.curve = s.tracingPaths[currentPathIndex].curve;
                this.handPOM.transform.position = s.tracingPaths[currentPathIndex].curve.points[0].transform.position;
            }

            s.ShowPathNumbers(currentPathIndex);

            //Move the hand
            this.Invoke("EnableHandTracing", traceDelay);
        }

        /// <summary>
        /// Spell the shape.
        /// </summary>
        public void Spell()
        {
            if (ShapesManager.GetCurrentShapesManager().GetCurrentShape().clip == null)
            {
                return;
            }

            AudioSources.instance.PlaySFXClip(ShapesManager.GetCurrentShapesManager().GetCurrentShape().clip, false);
        }

        public static void UnLoadScene(string sceneToUnload, System.Action CallBack_OnSceneUnload)
        {
            instance.StartCoroutine(loadYourAsyncScene());
            IEnumerator loadYourAsyncScene()
            {
                AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneToUnload);
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }
                CallBack_OnSceneUnload?.Invoke();
            }
        }

        /// <summary>
        /// Help the user.
        /// </summary>
        public void HelpUser()
        {
            int currentPathIndex = this.shape.GetCurrentPathIndex();

            if (currentPathIndex < 0 || currentPathIndex > this.shape.tracingPaths.Count - 1)
            {
                return;
            }

            this.selectedTracingPath = this.shape.tracingPaths[currentPathIndex];

            if (ShapesManager.GetCurrentShapesManager().tracingMode == ShapesManager.TracingMode.FILL)
            {
                this.selectedTracingPath.fillImage.color = this.brushColor.colorKeys[0].color;
                this.selectedTracingPath.AutoFill();
            }
            else if (ShapesManager.GetCurrentShapesManager().tracingMode == ShapesManager.TracingMode.LINE)
            {
                this.selectedTracingPath.line.SetColor(CommonUtil.ColorToGradient(this.brushColor.colorKeys[0].color));
            }


            this.selectedTracingPath.completed = true;
            var shapeCompleted = this.CheckShapeComplete();

            if (shapeCompleted)
            {
                UnityEvent unityEvent = new UnityEvent();
                unityEvent.AddListener(() => this.OnShapeComplete());
                this.selectedTracingPath.AutoLineComplete(true, unityEvent);
            }
            else
            {
                this.selectedTracingPath.AutoLineComplete(true);
                this.selectedTracingPath.OnComplete();

                this.StartAutoTracing(this.shape, 1);
                AudioSources.instance.PlayCorrectSFX();
            }

            this.selectedTracingPath = null;
        }

        /// <summary>
        /// Reset the path.
        /// </summary>
        private void ResetPath()
        {
            if (this.selectedTracingPath != null)
                this.selectedTracingPath.Reset();
            this.ReleasePath();
        }

        /// <summary>
        /// Init The Curve of current tracing path
        /// (used using when the position of the shape is changed in the world space)
        /// </summary>
        public void ReInitTracingPathCurve()
        {
            if (this.handPOM != null && this.handPOM.curve.IsInitialized())
            {
                this.handPOM.curve.ReInit();

                this.handPOM.Stop();
                this.handPOM.SetTarget(0);
                this.handPOM.Move();
            }
        }

        /// <summary>
        /// Release the path.
        /// </summary>
        private void ReleasePath()
        {
            this.selectedTracingPath = null;
            this.fillAmount = this.angleOffset = this.angle = 0;
            this.ResetTargetQuarter();
        }

        /// <summary>
        /// Reset the target quarter.
        /// </summary>
        private void ResetTargetQuarter()
        {
            this.targetQuarter = 90;
        }

        /// <summary>
        /// Enable the auto tracing of the hand.
        /// </summary>
        public void EnableHandTracing()
        {
            if (this.selectedTracingPath != null)
            {
                this.handPOM.curve = this.selectedTracingPath.curve;
            }

            if (this.handPOM.curve != null)
                this.handPOM.curve.Init();
            this.handPOM.MoveToStart();
        }

        /// <summary>
        /// Disable the auto tracing of the hand.
        /// </summary>
        public void DisableHandTracing()
        {
            this.CancelInvoke("EnableHandTracing");
            this.handPOM.Stop();
        }

        /// <summary>
        /// Show User's click/touch hand.
        /// </summary>
        public void ShowUserTouchHand()
        {
            this.hand.GetComponent<SpriteRenderer>().enabled = true;
        }

        /// <summary>
        /// Hide User's click/touch hand.
        /// </summary>
        public void HideUserTouchHand()
        {
            this.hand.GetComponent<SpriteRenderer>().enabled = false;
        }

        /// <summary>
        /// Draw the hand.
        /// </summary>
        /// <param name="clickPosition">Click position.</param>
        private void DrawHand(Vector3 clickPosition)
        {
            if (this.hand == null)
            {
                return;
            }

            this.hand.transform.position = clickPosition;
        }

        /// <summary>
        /// Set the size of the hand to default size.
        /// </summary>
        private void SetHandDefaultSize()
        {
            this.hand.transform.localScale = this.cursorDefaultSize;
        }

        /// <summary>
        /// Set the size of the hand to click size.
        /// </summary>
        private void SetHandClickSize()
        {
            this.hand.transform.localScale = this.cursorClickSize;
        }

        /// <summary>
        /// Draw the bright effect.
        /// </summary>
        /// <param name="clickPosition">Click position.</param>
        private void DrawBrightEffect(Vector3 clickPosition)
        {
            /*
            if (brightEffect == null) {
                return;
            }

            clickPosition.z = 0;
            brightEffect.transform.position = clickPosition;
            */
        }

        /// <summary>
        /// Hide/Disable Corner Timer Panel
        /// </summary>
        public void HideTimerPanel()
        {
            if (UserTraceInput.instance == null) return;

            Timer.instance.Stop();

            if (this.timerPanel != null)
                this.timerPanel.gameObject.SetActive(false);
        }

        /// <summary>
        /// Set the color of the shape order.
        /// </summary>
        public void SetShapeOrderColor()
        {
            //this.shapeOrder.color = this.brushColor.colorKeys[0].color;
        }

        /// <summary>
        /// Disable the game manager.
        /// </summary>
        public void DisableGameManager()
        {
            this.isRunning = false;
        }

        /// <summary>
        /// Enable the game manager.
        /// </summary>
        public void EnableGameManager()
        {
            this.isRunning = true;
        }

        /// <summary>
        /// Pause the game.
        /// </summary>
        public void Pause()
        {
            if (!this.isRunning)
            {
                return;
            }

            if (Timer.instance != null)
                Timer.instance.Pause();
            this.DisableGameManager();
        }

        /// <summary>
        /// Resume the game.
        /// </summary>
        public void Resume()
        {
            if (Timer.instance != null && UserTraceInput.instance == null)
                Timer.instance.Resume();
            this.EnableGameManager();
        }
    }
}