using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using BebiLibs;
using TMPro;
using UnityEngine.Rendering;
using BebiLibs.Popups.Gallery;
using static UnityEngine.UI.AspectRatioFitter;

namespace BebiLibs.Popups.ImageShare
{
    public class ImageSharePopup : PopUpBase
    {
        private static ImageSharePopup _instance;

        private Sprite _sprite;
        private Texture2D _texture;

        [Header("Renderer")]
        public Camera Camera_ShareRender;
        public SpriteRenderer SR_RenderImage;
        public TMP_Text Text_RenderText;
        public Transform TR_RenderText;
        public Vector3 boundOffset;
        public Texture2D shareTexture;

        public Vector2 startSize;
        public AspectRatioFitter aspectRatioFitter;
        public RectTransform contentRect;
        public bool useDefSound = true;

        [Header("UI")]
        public Image Image_Share;
        public Image Image_Share_Pets;
        public TMP_InputField InputText_Name;
        public TMP_Text Text_Name;

        public GameObject GO_ButtonClose;
        public GameObject GO_ButtonBack;

        private CommandBuffer _commandBuffer;
        private bool _fromGallery = false;
        private string _directoryPath;

        private NativeShare _nativeShare;
        private Vector2 _rectSizeDelta;

        [Header("Buttons")]
        public GameObject GO_ButtonSave;
        public static event System.Action<string> CallBack_OnShareOpen;
        public static event System.Action CallBack_OnShareClose;
        public bool isPetsGame = false;

        private void Awake()
        {
            Init();
        }

        public override void Init()
        {
            base.Init();
            if (_instance == null)
            {
                _instance = this;
            }

            startSize = this.contentRect.sizeDelta;
            _rectSizeDelta = this.Image_Share.rectTransform.sizeDelta;
            //Debug.Log(_rectSizeDelta);
        }

        public void OnNameChange(string name)
        {
            this.Text_Name.text = name;
            this.Text_RenderText.text = name;
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        public static void Activate(string filePath, bool fromGallery = false)
        {
            CallBack_OnShareOpen?.Invoke(filePath);
            ManagerSounds.PlayEffect("fx_page16");
            Debug.Log(filePath);
            if (filePath.Length > 0 && File.Exists(filePath))
            {
                _instance.Load(filePath, fromGallery);
            }
            else
            {
                _instance.Hide(false);
            }


            bool canSaveImage = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image) != NativeGallery.Permission.Denied;
            _instance.GO_ButtonSave.SetActive(canSaveImage);
        }

        public override void Show(bool anim)
        {
            base.Show(anim);
            //ManagerSounds.PlayEffect("fx_page15");
        }

        public void Load(string filePath, bool fromGallery)
        {
            if (fromGallery)
            {
                this.GO_ButtonClose.SetActive(false);
                this.GO_ButtonBack.SetActive(true);
            }
            else
            {
                this.GO_ButtonClose.SetActive(true);
                this.GO_ButtonBack.SetActive(false);
            }

            _directoryPath = Path.GetDirectoryName(filePath);
            this.Show(false);
            _fromGallery = fromGallery;
            this.gameObject.SetActive(true);
            this.StartCoroutine(this.LoadTexture(filePath));
        }

        public IEnumerator LoadTexture(string filePath)
        {
            yield return null;
            Texture2D texture = new Texture2D(1, 1);
            _texture = texture;
            ImageConversion.LoadImage(texture, File.ReadAllBytes(filePath));
            _sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 100, 0);

            if (this.isPetsGame)
            {
                this.Image_Share_Pets.sprite = _sprite;
            }
            else
            {
                this.Image_Share.sprite = _sprite;
            }

            //Debug.Log(_rectSizeDelta);

            float aspect = (float)_texture.width / (float)_texture.height;
            if (this.isPetsGame)
            {
                this.contentRect.sizeDelta = this.startSize;
                float contentAspect = this.startSize.x / this.startSize.y;
                Vector2 newSizeDelsta = this.startSize;
                if (aspect > contentAspect)
                {
                    newSizeDelsta.x = ((_texture.width * this.startSize.y) / _texture.height);
                    newSizeDelsta.y = this.startSize.y;
                }
                this.contentRect.sizeDelta = newSizeDelsta;
                this.aspectRatioFitter.aspectMode = AspectMode.HeightControlsWidth;
            }
            else
            {
                this.aspectRatioFitter.aspectMode = aspect < 1.3 ? AspectMode.HeightControlsWidth : AspectMode.WidthControlsHeight;
            }


            this.aspectRatioFitter.aspectRatio = aspect;
            this.Image_Share.rectTransform.sizeDelta = _rectSizeDelta;
            this.SR_RenderImage.sprite = _sprite;

            Bounds bounds = this.SR_RenderImage.bounds;
            Vector3 buttomLeft = new Vector3(bounds.max.x, bounds.min.y, 0);
            this.TR_RenderText.transform.position = buttomLeft + this.boundOffset;
        }

        public override void Hide(bool anim)
        {
            if (_fromGallery)
            {
                GalleryPopup.Activate(false, false);
                base.Hide(false);
            }
            else
            {
                base.Hide(true);
            }
            this.InputText_Name.text = string.Empty;
            this.Text_Name.text = string.Empty;
            this.Text_RenderText.text = string.Empty;
            Destroy(_texture);
            ManagerSounds.PlayEffect("fx_page17");
            CallBack_OnShareClose?.Invoke();
        }

        public void Trigger_ButtonClick_SaveGallery()
        {
            System.Guid gUid = System.Guid.NewGuid();
            string imageFilePath = Path.Combine(this.InputText_Name.text + "_" + gUid + ".png");
            this.shareTexture = this.Render(_texture.width, _texture.height);
            ManagerSounds.PlayEffect("fx_page11");

            NativeGallery.Permission writePermission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);
            if (writePermission == NativeGallery.Permission.ShouldAsk)
            {
                writePermission = NativeGallery.RequestPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);
            }

            if (writePermission == NativeGallery.Permission.Granted)
            {
                NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(this.shareTexture, "Child Coloing", imageFilePath, (success, path) =>
                {
                    Debug.Log("Media save result: " + success + " " + path);
                });
            }
            else
            {
                Debug.Log("Permission result: " + writePermission);
            }

            Destroy(this.shareTexture);
        }


        public void Trigger_ButtonClick_Share()
        {
            System.Guid gUid = System.Guid.NewGuid();
            string imageFilePath = Path.Combine(this.InputText_Name.text + "_" + gUid + ".png");
            this.shareTexture = this.Render(_texture.width, _texture.height);

            new NativeShare()
           .AddFile(this.shareTexture, imageFilePath)
           .SetSubject("Share Image")
           .SetCallback((result, shareTarget) =>
           {
               Debug.Log("Share result: " + result + ", selected app: " + shareTarget);
               GameObject.Destroy(this.shareTexture);
           })
           .Share();
            Destroy(this.shareTexture);
        }


        private Texture2D Render(int width, int height)
        {
            //this.InitializeCommandBuffer();
            RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.Default);
            Graphics.Blit(clearTexture, renderTexture);
            Bounds bounds = this.SR_RenderImage.bounds;
            Matrix4x4 ortho = Matrix4x4.Ortho(bounds.min.x, bounds.max.x, bounds.min.y, bounds.max.y, 0, 100);
            this.Camera_ShareRender.targetTexture = renderTexture;
            this.Camera_ShareRender.projectionMatrix = ortho;
            this.Camera_ShareRender.enabled = true;
            this.Camera_ShareRender.Render();
            RenderTexture.active = renderTexture;
            Texture2D tex = new Texture2D(width, height);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
            tex.Apply();
            RenderTexture.active = null;
            renderTexture.Release();
            GameObject.Destroy(renderTexture);
            this.Camera_ShareRender.enabled = false;
            return tex;
        }

        public void InitializeCommandBuffer()
        {
            if (_commandBuffer == null)
            {
                _commandBuffer = new CommandBuffer();
                this.Camera_ShareRender.AddCommandBuffer(CameraEvent.AfterForwardAlpha, _commandBuffer);
            }
            else
            {
                _commandBuffer.Clear();
            }
        }


        private static Texture2D _clearTexture;
        public static Texture2D clearTexture
        {
            get
            {
                if (_clearTexture == null)
                {
                    _clearTexture = new Texture2D(1, 1);
                    _clearTexture.SetPixel(0, 0, Color.clear);
                    _clearTexture.Apply();
                    return _clearTexture;
                }
                return _clearTexture;
            }
        }

    }
}