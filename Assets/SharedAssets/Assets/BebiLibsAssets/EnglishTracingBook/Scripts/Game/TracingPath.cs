using IndieStudio.EnglishTracingBook.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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
    [RequireComponent(typeof(LineAnimation))]
    public class TracingPath : MonoBehaviour
    {
        /// <summary>
        /// Line Animation component used to animate the line of tracing paths e.g on user hint or help
        /// </summary>
        private LineAnimation lineAnimation;

        /// <summary>
        /// Whether the path is completed or not.
        /// </summary>
        [HideInInspector]
        public bool completed;

        /// <summary>
        /// The fill method (Radial or Linear or Point).
        /// </summary>
        public FillMethod fillMethod;

        /// <summary>
        /// The complete offset (The fill amount offset).
        /// </summary>
        public float completeOffset = 0.85f;

        /// <summary>
        /// The first number reference.
        /// </summary>
        public Transform firstNumber;

        /// <summary>
        /// The second number reference.
        /// </summary>
        public Transform secondNumber;

        /// <summary>
        /// The shape reference.
        /// </summary>
        [HideInInspector]
        public Shape shape;

        /// <summary>
        /// The curve of the tracing path
        /// </summary>
        [HideInInspector]
        public Curve curve;

        /// <summary>
        /// The fill image
        /// </summary>
        [HideInInspector]
        public Image fillImage;

        /// <summary>
        /// The count of the traced points
        /// </summary>
        [HideInInspector]
        public int tracedPoints;

        /// <summary>
        /// The line of the tracing path
        /// </summary>
        [HideInInspector]
        public Line line;

        /// <summary>
        /// From : First Number Value, To : Second Number Value
        /// </summary>
        [HideInInspector]
        public int from, to;

        private ITracingExtension SC_GameTracingNumbers;

        void Awake()
        {

            this.SC_GameTracingNumbers = FindObjectsOfType<MonoBehaviour>().OfType<ITracingExtension>().ToArray().FirstOrDefault();

            this.lineAnimation = this.GetComponent<LineAnimation>();

            //Requires LineAnimation component
            if (this.lineAnimation == null)
            {
                this.lineAnimation = this.gameObject.AddComponent<LineAnimation>();
            }

            string[] slices = this.gameObject.name.Split('-');
            this.from = int.Parse(slices[1]);
            this.to = int.Parse(slices[2]);

            this.shape = this.GetComponentInParent<Shape>();
            this.curve = this.GetComponentInChildren<Curve>();
            this.fillImage = CommonUtil.FindChildByTag(this.transform, "Fill").GetComponent<Image>();
            this.fillImage.fillAmount = 0;
            this.tracedPoints = 0;

            //create new line
            this.CreateNewLine();
        }

        void Start()
        {
            if (SceneManager.GetActiveScene().name == "Game")
            {
                if (GameManager.instance.animateNumbersOnStart && GameManager.instance.compoundShape == null)
                {
                    //Animate the numbers of the Path
                    this.firstNumber.transform.position = this.secondNumber.transform.position = Vector3.zero;
                    TransformFollow2D firstNTF = this.firstNumber.gameObject.AddComponent<TransformFollow2D>();
                    TransformFollow2D secondtNTF = this.secondNumber.gameObject.AddComponent<TransformFollow2D>();

                    firstNTF.target = this.curve.GetFirstPoint();
                    secondtNTF.target = this.curve.GetLastPoint();

                    firstNTF.targetMode = secondtNTF.targetMode = TransformFollow2D.TargetMode.TRANSFORM;
                    firstNTF.speed = secondtNTF.speed = 6;
                    firstNTF.transform.position = secondtNTF.transform.position = Vector3.zero;
                    firstNTF.isRunning = secondtNTF.isRunning = true;
                }
            }
            else if (this.SC_GameTracingNumbers != null)
            {
                if (this.SC_GameTracingNumbers.AnimateNumbersOnStart && this.SC_GameTracingNumbers.CompoundShape == null)
                {
                    //Animate the numbers of the Path
                    this.firstNumber.transform.position = this.secondNumber.transform.position = Vector3.zero;
                    TransformFollow2D firstNTF = this.firstNumber.gameObject.AddComponent<TransformFollow2D>();
                    TransformFollow2D secondtNTF = this.secondNumber.gameObject.AddComponent<TransformFollow2D>();

                    firstNTF.target = this.curve.GetFirstPoint();
                    secondtNTF.target = this.curve.GetLastPoint();

                    firstNTF.targetMode = secondtNTF.targetMode = TransformFollow2D.TargetMode.TRANSFORM;
                    firstNTF.speed = secondtNTF.speed = 6;
                    firstNTF.transform.position = secondtNTF.transform.position = Vector3.zero;
                    firstNTF.isRunning = secondtNTF.isRunning = true;
                }
            }

            //Change numbers parent and sibling to be visible on the top
            RectTransform firstNumberRectTransfrom = this.firstNumber.GetComponent<RectTransform>();
            RectTransform secondNumberRectTransfrom = this.secondNumber.GetComponent<RectTransform>();

            firstNumberRectTransfrom.GetComponent<RectTransform>().SetParent(this.transform.parent);
            secondNumberRectTransfrom.GetComponent<RectTransform>().SetParent(this.transform.parent);

            firstNumberRectTransfrom.SetAsLastSibling();
            secondNumberRectTransfrom.SetAsLastSibling();
        }


        /// <summary>
        /// Create new line
        /// </summary>
        private void CreateNewLine()
        {
            if (ShapesManager.GetCurrentShapesManager().tracingMode != ShapesManager.TracingMode.LINE)
            {
                //skip if tracing mode not LINE
                return;
            }

            //disable fill image
            this.fillImage.enabled = false;

            GameObject linePrefab = null;
            if (SceneManager.GetActiveScene().name == "Game")
            {
                linePrefab = GameManager.instance.linePrefab;
            }
            else if (SceneManager.GetActiveScene().name == "Album")
            {
                linePrefab = ShapesTable.instance.linePrefab;
            }
            else if (this.SC_GameTracingNumbers != null)
            {
                linePrefab = this.SC_GameTracingNumbers.LinePrefab;
            }

            if (linePrefab != null)
            {
                GameObject lineGO = Instantiate(linePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                lineGO.transform.SetParent(this.transform);
                Vector3 temp = lineGO.transform.localPosition;
                temp.z = -500;
                lineGO.transform.localPosition = temp;
                lineGO.transform.localScale = Vector3.one;
                this.line = lineGO.GetComponent<Line>();

                this.SetUpLineWidth();
            }
        }

        public void SetUpLineWidth()
        {
            if (this.line == null)
                return;

            CompoundShape cs = this.GetComponentInParent<CompoundShape>();

            //Change line width ratio as you want from below in Game,Album scenes
            if (SceneManager.GetActiveScene().name == "Game")
            {
                if (cs == null && UserTraceInput.instance == null)
                {
                    //shape line width
                    this.line.SetWidth(this.shape.content.localScale.magnitude * ShapesManager.GetCurrentShapesManager().GameShapeLineWidth);
                }
                else
                {
                    //compound shape line width

                    if (cs == null)
                    {
                        this.line.SetWidth(this.transform.GetComponentInParent<Shape>().transform.localScale.magnitude * ShapesManager.GetCurrentShapesManager().GameCompoundShapeLineWidth);
                    }
                    else
                    {
                        if (cs.shapes.Count != 0)
                            this.line.SetWidth(cs.shapes[0].transform.localScale.magnitude * ShapesManager.GetCurrentShapesManager().GameCompoundShapeLineWidth);
                    }

                }
            }
            else if (SceneManager.GetActiveScene().name == "Album")
            {
                if (cs == null)
                {
                    //shape line width
                    this.line.SetWidth(this.shape.content.localScale.magnitude * ShapesManager.GetCurrentShapesManager().AlbumShapeLineWidth);
                }
                else
                {
                    //compound shape line width
                    if (cs.shapes.Count != 0)
                        this.line.SetWidth(cs.shapes[0].transform.localScale.magnitude * ShapesManager.GetCurrentShapesManager().AlbumCompoundShapeLineWidth);
                }
            }

        }

        /// <summary>
        /// Auto fill.
        /// </summary>
        public void AutoFill()
        {
            if (Mathf.Approximately(this.fillImage.fillAmount, 1))
            {
                return;
            }

            this.StartCoroutine("AutoFillCoroutine");
        }


        /// <summary>
        /// Auto fill coroutine.
        /// </summary>
        /// <returns>The fill coroutine.</returns>
        private IEnumerator AutoFillCoroutine()
        {
            while (this.fillImage.fillAmount < 1)
            {
                this.fillImage.fillAmount += 0.02f;
                yield return new WaitForSeconds(0.001f);
            }

        }

        /// <summary>
        /// Set the status of the numbers.
        /// </summary>
        /// <param name="status">the status value.</param>
        public void SetNumbersStatus(bool status, bool both = true)
        {
            this.StartCoroutine(this.SetNumbersStatusCoroutine(status, both));
        }

        private IEnumerator SetNumbersStatusCoroutine(bool status, bool bothNumbers = true)
        {
            yield return 0;

            Transform[] numbers = new Transform[] { this.firstNumber, this.secondNumber };
            if (!bothNumbers)
                numbers[1] = null;
            Color tempColor = Color.white;
            Animator animator = null;
            foreach (Transform number in numbers)
            {
                if (number == null)
                    continue;

                animator = number.GetComponent<Animator>();
                yield return new WaitUntil(() => animator.isInitialized);

                if (status == true)
                {
                    animator.SetBool("Select", true);
                    tempColor.a = 1;
                }
                else
                {
                    if (this.shape.enablePriorityOrder)
                    {
                        animator.SetBool("Select", false);
                        tempColor.a = 0.3f;
                    }
                }
                number.GetComponent<Image>().color = tempColor;
            }
        }

        /// <summary>
        /// Set the visibility of the  numbers.
        /// </summary>
        /// <param name="visible">visibility value.</param>
        public void SetNumbersVisibility(bool visible)
        {
            if (this.firstNumber != null)
            {
                this.firstNumber.gameObject.SetActive(visible);
            }
            if (this.secondNumber != null)
            {
                if (this.fillMethod == FillMethod.Point)
                {
                    this.secondNumber.gameObject.SetActive(false);
                }
                else
                {
                    this.secondNumber.gameObject.SetActive(visible);
                }
            }
        }

        public void SetFirstNumberVisible(bool visible)
        {
            if (this.firstNumber != null)
            {
                this.firstNumber.gameObject.SetActive(visible);
            }
        }
        /// <summary>
        /// Disable the current point
        /// </summary>
        public void DisableCurrentPoint()
        {
            this.curve.SetPointActiveValue(false, this.tracedPoints);
        }

        /// <summary>
        /// Enable the current point
        /// </summary>
        public void EnableCurrentPoint()
        {
            this.curve.SetPointActiveValue(true, this.tracedPoints);
        }

        /// <summary>
        /// Reset the path.
        /// </summary>
        public void Reset(bool setNumbersVisibility = true)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (this.line != null)
            {
                this.line.Reset();
            }

            this.tracedPoints = 0;
            this.curve.DisablePoints();
            this.EnableCurrentPoint();
            if (setNumbersVisibility)
                this.SetNumbersVisibility(true);
            this.completed = false;

            if (!this.shape.enablePriorityOrder)
            {
                this.SetNumbersStatus(true, setNumbersVisibility);
            }
            this.StartCoroutine("ReleaseFillCoroutine");
        }


        /// <summary>
        /// Release Fill coroutine.
        /// </summary>
        /// <returns>The coroutine.</returns>
        private IEnumerator ReleaseFillCoroutine()
        {
            while (this.fillImage.fillAmount > 0)
            {
                this.fillImage.fillAmount -= 0.02f;
                yield return new WaitForSeconds(0.005f);
            }
        }

        /// <summary>
        /// On complete the tracing path
        /// </summary>
        public void OnComplete()
        {
            this.SetNumbersVisibility(false);
        }

        /// <summary>
        /// Auto line drawing or complete for the path
        /// </summary>
        public void AutoLineComplete(bool animated = false, UnityEvent animationDoneEvent = null)
        {
            if (this.line == null)
            {
                return;
            }

            this.line.Reset();
            Vector3 inversePoint;
            List<Vector3> curvePoints = new List<Vector3>();

            foreach (Transform point in this.curve.points)
            {
                inversePoint = this.line.transform.InverseTransformPoint(point.position);
                curvePoints.Add(inversePoint);

                if (this.fillMethod == FillMethod.Point && this.curve.points.Count == 1)
                {
                    curvePoints.Add(inversePoint);
                }
            }
            if (!animated)
            {
                //without animation / instantly
                foreach (Vector3 point in curvePoints)
                {
                    this.line.AddPoint(point);
                }

                this.line.BezierInterpolate(this.curve.smoothness);
            }
            else
            {
                //with animation / delay
                Line.bezier.Interpolate(curvePoints, this.curve.smoothness);
                this.lineAnimation.Run(Line.bezier.GetDrawingPoints2(), this.line, animationDoneEvent);
            }
        }

        public enum FillMethod
        {
            Radial,
            Linear,
            Point
        }
    }
}