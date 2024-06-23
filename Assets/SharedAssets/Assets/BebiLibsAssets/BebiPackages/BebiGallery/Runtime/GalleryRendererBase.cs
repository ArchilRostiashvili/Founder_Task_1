using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace BebiLibs.Popups.Gallery
{
    public abstract class GalleryRendererBase : MonoBehaviour
    {
        [Header("Render Camera")]
        [SerializeField] protected Camera _galleryImageRendererCamera;

        [Header("Dimentions")]
        [SerializeField] protected Sprite _backgroundSprite;

        [SerializeField] protected int _width;
        [SerializeField] protected int _height;

        [Range(0, 6)]
        [SerializeField] protected int _thumbnailSizeFactory;

        [Header("Outline")]
        [SerializeField] protected Material _firstOutlineMaterial;
        [SerializeField] protected Material _secondOutlineMaterial;

        [Range(1, 10)]
        [SerializeField] protected float _firstOutlineSize;
        [Range(1, 30)]
        [SerializeField] protected float _secondOutlineSize;

        [Header("File Path")]
        [SerializeField] protected string _galleryPath = "artboard_gallery";
        [SerializeField] protected string _imagesPath = "Image";
        [SerializeField] protected string _thumbnailsPath = "Thumbnails";

        protected RenderTexture _renderTexture;
        protected RenderTexture _thumbnailTexture;
        protected CommandBuffer _commandBuffer;

        private System.Action<Texture2D> _imageRenderingEndEvent;
        private int _imageryRenderingEndListenerCount = 0;
        protected bool _hasImageRenderingEndListeners => _imageryRenderingEndListenerCount > 0;

        public void AddImageRenderingEndEventListener(System.Action<Texture2D> action)
        {
            _imageryRenderingEndListenerCount++;
            _imageRenderingEndEvent += action;
        }

        public void RemoveImageRenderingEnvEventListener(System.Action<Texture2D> action)
        {
            _imageryRenderingEndListenerCount = Mathf.Clamp(_imageryRenderingEndListenerCount--, 0, 100);
            _imageRenderingEndEvent -= action;
        }

        protected void InvokeImageRenderingEndEvent(Texture2D texture2D)
        {
            _imageRenderingEndEvent?.Invoke(texture2D);
        }


#if UNITY_EDITOR
        protected bool _isTest = false;
        private void OnValidate()
        {
            _firstOutlineMaterial.SetFloat("_OutlineDist", _firstOutlineSize);
            _secondOutlineMaterial.SetFloat("_OutlineDist", _secondOutlineSize);
        }
#endif


        public void InitializeCommandBuffer()
        {
            if (_commandBuffer == null)
            {
                _commandBuffer = new CommandBuffer();
                _galleryImageRendererCamera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, _commandBuffer);
            }
            else
            {
                _commandBuffer.Clear();
            }
        }


        protected void RenderDelayFast(string name, System.Action<string> onRenderEnd, bool saveAsNew = false)
        {
            string ImagePath = Path.Combine(Application.persistentDataPath, _galleryPath, _imagesPath);
            string ThumbnailPath = Path.Combine(Application.persistentDataPath, _galleryPath, _thumbnailsPath);
            CheckDirectoryPath(ImagePath);
            CheckDirectoryPath(ThumbnailPath);

            string imageFilePath = Path.Combine(ImagePath, name + ".png");
            string thumbnailImagePath = Path.Combine(ThumbnailPath, name + ".png");

            if (saveAsNew)
            {
                try
                {
                    FileInfo[] arrayImageFiles = new DirectoryInfo(ImagePath).GetFiles(name + "*").OrderByDescending(f => f.LastWriteTime).ToArray();
                    for (int i = 0; i < arrayImageFiles.Length; i++)
                    {
                        if (i > 10)
                        {
                            if (File.Exists(arrayImageFiles[i].FullName))
                            {
                                File.Delete(arrayImageFiles[i].FullName);
                            }
                            string thumb = Path.Combine(ThumbnailPath, arrayImageFiles[i].Name);
                            if (File.Exists(thumb))
                            {
                                File.Delete(thumb);
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(e.Message);
                }

                System.Guid myGUID = System.Guid.NewGuid();
                imageFilePath = Path.Combine(ImagePath, name + "_" + myGUID.ToString() + ".png");
                thumbnailImagePath = Path.Combine(ThumbnailPath, name + "_" + myGUID.ToString() + ".png");
            }

#if UNITY_EDITOR
            if (_isTest)
            {
                ImagePath = Path.Combine(Application.dataPath);
                imageFilePath = Path.Combine(ImagePath, "TempFile" + ".png");
                thumbnailImagePath = Path.Combine(ImagePath, "TempFile_Temp" + ".png");
                onRenderEnd = null;
            }

            Debug.Log(imageFilePath);
#endif
            SaveTextures(imageFilePath, thumbnailImagePath, onRenderEnd);
        }

        protected abstract void SaveTextures(string imageFilePath, string thumbnailFilePath, System.Action<string> action);

        public virtual void Clear()
        {
            _galleryImageRendererCamera.targetTexture = null;
            RenderTexture.active = null;
            if (_renderTexture != null)
            {
                _renderTexture.Release();
            }

            if (_thumbnailTexture != null)
            {
                _thumbnailTexture.Release();
            }
        }

        private void CheckDirectoryPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public bool SaveRTToFile(RenderTexture rt, string filePath, out Texture2D texture)
        {
            RenderTexture.active = rt;
            texture = new Texture2D(rt.width, rt.height);
            texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);
            texture.Apply();
            RenderTexture.active = null;
            byte[] bytes = ImageConversion.EncodeToPNG(texture);
            try
            {
                File.WriteAllBytes(filePath, bytes);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error: " + e.Message);
                GameObject.Destroy(texture);
                return false;
            }
        }
    }

}
