using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace BebiLibs
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextLinkClickDetector : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
        [SerializeField] private List<LinkIDData> _linkIDDataList = new List<LinkIDData>();

        [SerializeField] private bool _callImmediately = false;

        public UnityEvent<LinkIDData> onClick = new UnityEvent<LinkIDData>();

        private void Awake()
        {
            if (_textMeshProUGUI == null)
            {
                _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("OnPointerClick");
            int linkID = TMP_TextUtilities.FindIntersectingLink(_textMeshProUGUI, Input.mousePosition, null);
            if (linkID == -1)
            {
                return;
            }

            var linkData = _linkIDDataList.Find(x => x.LinkIndex == linkID);

            if (linkData == null)
            {
                Debug.Log("Unable to find link data with ID: " + linkID);
                return;
            }

            if (_callImmediately)
            {
                Application.OpenURL(linkData.LinkURL);
                onClick?.Invoke(linkData);
            }
            else
            {
                onClick?.Invoke(linkData);
            }
        }
    }

    [System.Serializable]
    public class LinkIDData
    {
        public int LinkIndex;
        public string LinkID;
        public string LinkURL;
    }
}
