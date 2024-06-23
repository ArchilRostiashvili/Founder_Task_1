using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Android;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.Networking;
using DG.Tweening;
using BebiLibs;

namespace BebiLibs.Popups.Gallery
{
    public class GalleryElement : MonoBehaviour
    {
        public System.Action<GalleryElement> OnElementClickEvent;

        [SerializeField] public string ImageFilePath;
        [SerializeField] public string ThumbnailFilePath;
        [SerializeField] public Texture2D Texture;
        [SerializeField] public Sprite Sprite;
        [SerializeField] public bool IsInitialized;
        [SerializeField] public bool IsSelected;


        [Header("Item Data")]
        [SerializeField] private Image _galleryImage;
        [SerializeField] private Image _selectorKnobImage;

        private ButtonScale _galleryImageButton;

        private void Awake()
        {
            _galleryImageButton = this.GetComponent<ButtonScale>();
            _galleryImageButton.onClick.RemoveAllListeners();
            _galleryImageButton.onClick.AddListener(OnItemClick);
        }

        public void OnItemClick()
        {
            OnElementClickEvent?.Invoke(this);
        }

        public void Generate(string filePath, string thumbnailPath)
        {
            gameObject.SetActive(true);
            enabled = true;
            ImageFilePath = filePath;
            ThumbnailFilePath = thumbnailPath;
            _galleryImage.enabled = false;
            _selectorKnobImage.enabled = false;

            StartCoroutine(LoadTexture(ThumbnailFilePath));
        }

        public void Select()
        {
            _selectorKnobImage.enabled = true;
            IsSelected = true;
        }

        public void CancelSelection()
        {
            _selectorKnobImage.enabled = false;
            IsSelected = false;
        }

        public void Clear()
        {
            transform.DOKill();
            gameObject.SetActive(false);

            _galleryImage.enabled = false;
            ImageFilePath = string.Empty;

            if (Texture != null)
            {
                GameObject.Destroy(Texture);
                Texture = null;
            }

            IsInitialized = false;
            IsSelected = false;
        }

        public IEnumerator LoadTexture(string filePath)
        {
            yield return null;
            Texture2D texture = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture, File.ReadAllBytes(filePath));
            Sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100, 0);
            _galleryImage.sprite = Sprite;
            IsInitialized = true;
            _galleryImage.enabled = true;
        }
    }
}