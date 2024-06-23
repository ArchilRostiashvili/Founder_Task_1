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
    public class ScrollSlider : MonoBehaviour
    {
        /// <summary>
        /// The groups grid layout reference
        /// </summary>
        public GridLayoutGroup groupsGridLayout;

        /// <summary>
        /// The sscroll rect component
        /// </summary>
        private ScrollRect scrollRect;

        /// <summary>
        /// The scroll content
        /// </summary>
        public RectTransform scrollContent;

        /// <summary>
        /// The pointers grid layout.
        /// </summary>
        public GridLayoutGroup pointersGridLayout;

        /// <summary>
        /// The rect transform of this instance
        /// </summary>
        private RectTransform rectTransform;

        /// <summary>
        /// The next button.
        /// </summary>
        public Transform nextButton;

        /// <summary>
        /// The previous button.
        /// </summary>
        public Transform previousButton;

        /// <summary>
        /// The current group text.
        /// </summary>
        public Text currentGroupText;

        /// <summary>
        /// The pointer enabled sprite.
        /// </summary>
        public Sprite pointerEnabled;

        /// <summary>
        /// The pointer disabled sprite.
        /// </summary>
        public Sprite pointerDisabled;

        /// <summary>
        /// The lerp speed.
        /// </summary>
        [Range(5, 100)]
        public float lerpSpeed = 8;

        /// <summary>
        /// The group width ratio.
        /// </summary>
        [Range(0.1f, 5.0f)]
        public float groupWidthRatio = 1;

        /// <summary>
        /// The group height ratio.
        /// </summary>
        [Range(0.1f, 5.0f)]
        public float groupHeightRatio = 1;

        /// <summary>
        /// The group spacing ratio.
        /// </summary>
        [Range(0, 5.0f)]
        public float groupSpacingRatio = 0.3f;

        /// <summary>
        /// The pointers width ratio.
        /// </summary>
        [Range(0.01f, 5.0f)]
        public float pointersWidthRatio = 0.036f;

        /// <summary>
        /// The pointers height ratio.
        /// </summary>
        [Range(0.01f, 5.0f)]
        public float pointersHeightRatio = 1;

        /// <summary>
        /// The pointers spacing ratio.
        /// </summary>
        [Range(0, 5.0f)]
        public float pointersSpacingRatio = 0.002f;

        /// <summary>
        /// A loop scroll.
        /// </summary>
        public bool loop = true;

        /// <summary>
        /// The groups list.
        /// </summary>
        [HideInInspector]
        public GameObject[] groups;

        /// <summary>
        /// The pointers list.
        /// </summary>
        [HideInInspector]
        public GameObject[] pointers;

        /// <summary>
        /// The holder of the callback on change group .
        /// </summary>
        public Transform callBackHolder;

        /// <summary>
        /// The callback on change group .
        /// </summary>
        public string changeGroupCallBack = "OnChangeGroup";

        /// <summary>
        /// The index of the current group.
        /// </summary>
        [HideInInspector]
        public int currentGroupIndex = 0;

        /// <summary>
        /// A temp color.
        /// </summary>
        private Color tempColor;

        /// <summary>
        /// A temp anchroed postion.
        /// </summary>
        private Vector3 tempAnchoredPostion;

        /// <summary>
        /// The group anchored position.
        /// </summary>
        private Vector3 groupAnchoredPosition;

        /// <summary>
        /// Whether lerping to the group or not
        /// </summary>
        private bool isLerping;

        /// <summary>
        /// The screen's initial aspect ratio.
        /// </summary>
        private float initialAspectRatio;

        /// <summary>
        ///A temp lerp speed.
        /// </summary>
        private float tempLerpSpeed;

        /// <summary>
        /// Static instance of this class.
        /// </summary>
        public static ScrollSlider instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public void Init()
        {

            //Setting up references and initial values
            this.pointers = CommonUtil.FindGameObjectsOfTag("Pointer");
            this.groups = CommonUtil.FindGameObjectsOfTag("Group");
            this.scrollRect = this.GetComponent<ScrollRect>();
            this.rectTransform = this.GetComponent<RectTransform>();
            this.initialAspectRatio = Camera.main.aspect;
            this.CalculateGridLayoutsValues();
            this.isLerping = true;

            if (this.groups == null)
            {
                Debug.LogWarning("No groups found");
            }
            else if (this.groups.Length == 0)
            {
                Debug.LogWarning("No groups found");
            }

            this.SnapToGroup();
            this.GoToCurrentGroup();
            this.tempLerpSpeed = 256;
        }

        void Update()
        {
            if (Camera.main.aspect != this.initialAspectRatio)
            {
                this.CalculateGridLayoutsValues();
                this.initialAspectRatio = Camera.main.aspect;
            }

            if (this.isLerping)
                this.SnapToGroup();

            this.HandleInput();
        }

        /// <summary>
        /// Calculate the grid layouts values such as Spacing,Cell Size,Constraint Count.
        /// </summary>
        private void CalculateGridLayoutsValues()
        {
            if (this.rectTransform == null)
            {
                return;
            }

            if (this.groupsGridLayout != null)
            {
                this.groupsGridLayout.spacing = new Vector2(this.rectTransform.rect.width * this.groupSpacingRatio, 0);
                this.groupsGridLayout.cellSize = new Vector2(this.rectTransform.rect.width * this.groupWidthRatio, this.rectTransform.rect.height * this.groupHeightRatio);
            }
            if (this.pointersGridLayout != null)
            {
                this.pointersGridLayout.spacing = new Vector2(this.pointersGridLayout.GetComponent<RectTransform>().rect.width * this.pointersSpacingRatio, 0);
                this.pointersGridLayout.cellSize = new Vector2(this.pointersGridLayout.GetComponent<RectTransform>().rect.width * this.pointersWidthRatio, this.pointersGridLayout.GetComponent<RectTransform>().rect.height * this.pointersHeightRatio);
            }
        }

        /// <summary>
        /// Handles the user input.
        /// </summary>
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.NextGroup();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                this.PreviousGroup();
            }
        }

        /// <summary>
        /// Calcualte the group anchored position.
        /// </summary>
        /// <param name="group">Group.</param>
        public void CalculateGroupAnchoredPosition(RectTransform group)
        {
            this.groupAnchoredPosition = (Vector2)this.scrollRect.transform.InverseTransformPoint(this.scrollContent.position) - (Vector2)this.scrollRect.transform.InverseTransformPoint(group.position) + new Vector2(this.rectTransform.rect.width / 2.0f, 0);
        }

        /// <summary>
        /// Snaps to the group.
        /// </summary>
        public void SnapToGroup()
        {
            if (this.groups == null)
            {
                return;
            }

            if (this.groups.Length == 0)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();

            this.tempAnchoredPostion.x = Mathf.Lerp(this.tempAnchoredPostion.x, this.groupAnchoredPosition.x, this.tempLerpSpeed * Time.smoothDeltaTime);
            this.tempAnchoredPostion.y = 0;
            this.scrollContent.anchoredPosition = this.tempAnchoredPostion;

            if (Vector2.Distance(this.tempAnchoredPostion, this.groupAnchoredPosition) <= 5)
            {
                this.DisableLerping();
            }
        }

        /// <summary>
        /// Go to the current group.
        /// </summary>
        public void GoToCurrentGroup()
        {
            if (this.groups == null)
            {
                return;
            }

            if (this.groups.Length == 0)
            {
                return;
            }

            this.tempLerpSpeed = this.lerpSpeed;

            this.DisableFarGroups(this.currentGroupIndex);

            if (this.groups.Length <= 1)
            {
                this.DisableNextButton();
                this.DisablePreviousButton();
            }
            else if (this.currentGroupIndex == 0)
            {
                this.DisablePreviousButton();
                this.EnableNextButton();
            }
            else if (this.currentGroupIndex == this.groups.Length - 1)
            {
                this.DisableNextButton();
                this.EnablePreviousButton();
            }
            else
            {
                this.EnableNextButton();
                this.EnablePreviousButton();
            }

            if (this.currentGroupText != null)
                this.currentGroupText.text = (this.currentGroupIndex + 1) + "/" + this.groups.Length;

            this.scrollRect.StopMovement();
            this.CalculateGroupAnchoredPosition(this.groups[this.currentGroupIndex].GetComponent<RectTransform>());
            this.tempAnchoredPostion = this.scrollContent.anchoredPosition;
            this.EnableCurrentPointer();
            this.EnableLerping();
            if (this.callBackHolder != null && !string.IsNullOrEmpty(this.changeGroupCallBack))
            {
                this.callBackHolder.SendMessage(this.changeGroupCallBack, this.currentGroupIndex, SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// Go to the next group.
        /// </summary>
        public void NextGroup()
        {
            if (this.groups == null)
            {
                return;
            }

            if (this.groups.Length == 0)
            {
                return;
            }

            if (this.currentGroupIndex + 1 >= this.groups.Length)
            {
                if (this.loop)
                {
                    this.DisableCurrentPointer();
                    this.currentGroupIndex = 0;
                }
            }
            else
            {
                this.DisableCurrentPointer();
                this.currentGroupIndex += 1;
            }

            this.GoToCurrentGroup();
        }

        /// <summary>
        /// Go to the previous group.
        /// </summary>
        public void PreviousGroup()
        {
            if (this.groups == null)
            {
                return;
            }

            if (this.groups.Length == 0)
            {
                return;
            }

            if (this.currentGroupIndex - 1 < 0)
            {
                if (this.loop)
                {
                    this.DisableCurrentPointer();
                    this.currentGroupIndex = this.groups.Length - 1;
                }
            }
            else
            {
                this.DisableCurrentPointer();
                this.currentGroupIndex -= 1;
            }
            this.GoToCurrentGroup();
        }


        /// <summary>
        /// Raises the drag begin event.
        /// </summary>
        public void OnDragBegin()
        {
            this.DisableLerping();
        }

        /// <summary>
        /// Raises the drag end event.
        /// </summary>
        public void OnDragEnd()
        {
            if (this.groups == null)
            {
                return;
            }

            if (this.groups.Length == 0)
            {
                return;
            }

            if (this.scrollRect.velocity.x < -350)
            {
                //Scroll to the next
                this.NextGroup();
            }
            else if (this.scrollRect.velocity.x > 350)
            {
                //Scroll to the previous
                this.PreviousGroup();
            }
            else
            {
                //Scroll to the closest
                RectTransform closestGroup = this.groups[0].GetComponent<RectTransform>();
                float dist1, dist2 = Mathf.Infinity;
                foreach (GameObject group in this.groups)
                {
                    //The Horizontal Distance between the current group and levels panel
                    dist1 = Mathf.Abs(group.transform.position.x - this.rectTransform.transform.position.x);

                    if (dist1 < dist2)
                    {
                        closestGroup = group.GetComponent<RectTransform>();
                        //The Horizontal Distance between the closest group and levels panel
                        dist2 = Mathf.Abs(closestGroup.transform.position.x - this.rectTransform.transform.position.x);
                    }
                }

                this.DisableCurrentPointer();
                this.currentGroupIndex = closestGroup.transform.GetComponent<Group>().Index;
                this.GoToCurrentGroup();
            }
        }

        /// <summary>
        /// Enable the current pointer.
        /// </summary>
        public void EnableCurrentPointer()
        {
            if (this.pointers == null)
            {
                return;
            }

            if (this.pointers.Length == 0)
            {
                return;
            }

            if (this.currentGroupIndex >= 0 && this.currentGroupIndex < this.pointers.Length)
            {
                Color tempColor = this.pointers[this.currentGroupIndex].GetComponent<Image>().color;
                tempColor.a = 1;
                this.pointers[this.currentGroupIndex].GetComponent<Image>().color = tempColor;
                this.pointers[this.currentGroupIndex].GetComponent<Image>().sprite = this.pointerEnabled;
            }
        }

        /// <summary>
        /// Disable the current pointer.
        /// </summary>
        public void DisableCurrentPointer()
        {
            if (this.pointers == null)
            {
                return;
            }

            if (this.pointers.Length == 0)
            {
                return;
            }

            if (this.pointers == null)
            {
                return;
            }

            if (this.currentGroupIndex >= 0 && this.currentGroupIndex < this.pointers.Length)
            {
                Color tempColor = this.pointers[this.currentGroupIndex].GetComponent<Image>().color;
                tempColor.a = 0.3f;
                this.pointers[this.currentGroupIndex].GetComponent<Image>().color = tempColor;
                this.pointers[this.currentGroupIndex].GetComponent<Image>().sprite = this.pointerDisabled;
            }
        }

        /// <summary>
        /// Enable the next button.
        /// </summary>
        public void EnableNextButton()
        {
            if (this.nextButton != null)
            {
                this.nextButton.GetComponent<Button>().interactable = true;
            }
        }

        /// <summary>
        /// Disables the next button.
        /// </summary>
        public void DisableNextButton()
        {
            if (this.nextButton != null)
            {
                this.nextButton.GetComponent<Button>().interactable = false;
            }
        }

        /// <summary>
        /// Enable the next button.
        /// </summary>
        public void EnablePreviousButton()
        {
            if (this.previousButton != null)
            {
                this.previousButton.GetComponent<Button>().interactable = true;
            }
        }

        /// <summary>
        /// Disables the next button.
        /// </summary>
        public void DisablePreviousButton()
        {
            if (this.previousButton != null)
            {
                this.previousButton.GetComponent<Button>().interactable = false;
            }
        }

        /// <summary>
        /// Disable lerping to the lerping.
        /// </summary>
        public void DisableLerping()
        {
            this.isLerping = false;
        }

        /// <summary>
        /// Enable the lerping to the groupd.
        /// </summary>
        public void EnableLerping()
        {
            this.isLerping = true;
        }

        /// <summary>
        /// Disable the far groups.
        /// </summary>
        private void DisableFarGroups(int groupIndex)
        {
            if (this.groups == null)
            {
                return;
            }

            if (!(groupIndex >= 0 && groupIndex < this.groups.Length))
            {
                return;
            }

            for (int i = 0; i < this.groups.Length; i++)
            {
                if (i == groupIndex - 1 || i == groupIndex || i == groupIndex + 1)
                {
                    CommonUtil.EnableChildern(this.groups[i].transform);
                }
                else
                {
                    CommonUtil.DisableChildern(this.groups[i].transform);
                }
            }
        }
    }
}