using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BebiLibs;
using System.IO;
using System.Linq;
using TMPro;
using BebiLibs.Popups.ImageShare;
using System;

namespace BebiLibs.Popups.Gallery
{
    public class GalleryPopup : PopUpBase
    {
        private static GalleryPopup _Instance;

        public ScrollRect scrollRect;
        [Header("Gallery View")]
        public GameObject GO_Gallery;
        public GameObject GO_GalleryEmpty;

        [Header("Gallery Settings")]
        public GalleryElement galleryElementPrefab;
        public Transform parentTransform;
        public string _galleryPath = "artboard_gallery";
        public string _imagesPath = "Image";
        public string _thumbnailsPath = "Thumbnails";

        [Header("Sounds")]
        //public string clickSound = "fx_page14";
        //public string cancelClickSound = "fx_page17";

        [Header("Buttons")]
        public ButtonScale Button_Select;
        public ButtonScale Button_Delete;
        public ButtonScale Button_Cancel;
        public GameObject GO_Select_Text;
        public TMP_Text Text_Warning;

        private static int _notificationCount;

        private readonly Queue<GalleryElement> _itemGalleries = new Queue<GalleryElement>();
        private readonly List<GalleryElement> _arraySelectedItems = new List<GalleryElement>();
        private readonly List<GalleryElement> _activeGallery = new List<GalleryElement>();

        private static string _imagePath;
        private static string _thumbnailPath;
        private bool _isSelectionActive;

        public static event System.Action CallBack_OnGalleryClose;
        public static event System.Action CallBack_OnGalleryOpen;

        public int maxImageCount = 30;

        private void Awake()
        {
            Init();
        }

        public override void Init()
        {
            base.Init();
            _Instance = this;

            _imagePath = Path.Combine(Application.persistentDataPath, _galleryPath, _imagesPath);
            _thumbnailPath = Path.Combine(Application.persistentDataPath, _galleryPath, _thumbnailsPath);

            CheckDirectoryPath(_imagePath);
            CheckDirectoryPath(_thumbnailPath);
        }

        public static void AddNotification()
        {
            if (_Instance != null)
            {
                _notificationCount++;
                SetNotificationCount(_notificationCount);
            }
        }

        private static void ClearNotification()
        {
            SetNotificationCount(0);
            _notificationCount = 0;
        }

        public static void SetNotificationCount(int count)
        {
            PlayerPrefs.SetInt("NotificationCount", count);
        }

        public static int GetNotificationCount()
        {
            return PlayerPrefs.GetInt("NotificationCount");
        }

        public static void Activate(bool anim = true, bool useSound = true)
        {
            ClearNotification();
            _Instance.Show(anim);
            if (useSound)
                ManagerSounds.PlayEffect("fx_page16");

            _Instance.Recalculate();
            CallBack_OnGalleryOpen?.Invoke();
        }

        public override void Show(bool anim)
        {
            base.Show(anim);
        }



        public override void Hide(bool anim)
        {
            Close();
            base.Hide(anim);
            CallBack_OnGalleryClose?.Invoke();
        }

        public void Trigger_ButtonClick_ClickItem(GalleryElement galleryElement)
        {
            if (_isSelectionActive)
            {
                ManagerSounds.PlayEffect("fx_page14");
                if (galleryElement.IsSelected)
                {
                    galleryElement.CancelSelection();
                    _arraySelectedItems.Remove(galleryElement);
                }
                else
                {
                    galleryElement.Select();
                    _arraySelectedItems.Add(galleryElement);
                }
                if (_arraySelectedItems.Count > 0)
                {
                    Button_Delete.gameObject.SetActive(true);
                    Button_Cancel.gameObject.SetActive(true);
                    GO_Select_Text.gameObject.SetActive(false);
                }
                else
                {
                    Button_Delete.gameObject.SetActive(false);
                    Button_Cancel.gameObject.SetActive(true);
                    GO_Select_Text.gameObject.SetActive(true);
                }
            }
            else
            {
                ImageSharePopup.Activate(galleryElement.ImageFilePath, true);
                Hide(false);
            }
        }

        public void Trigger_ButtonClick_DeleteSelected()
        {
            ManagerSounds.PlayEffect("fx_page17");
            for (int i = 0; i < _arraySelectedItems.Count; i++)
            {
                string imageFilePath = _arraySelectedItems[i].ImageFilePath;
                string thumbnailFilePath = _arraySelectedItems[i].ThumbnailFilePath;
                if (File.Exists(imageFilePath))
                {
                    File.Delete(imageFilePath);
                }

                if (File.Exists(thumbnailFilePath))
                {
                    File.Delete(thumbnailFilePath);
                }
                _arraySelectedItems[i].Clear();
                _activeGallery.Remove(_arraySelectedItems[i]);
                _itemGalleries.Enqueue(_arraySelectedItems[i]);
            }
            _arraySelectedItems.Clear();
            Button_Delete.gameObject.SetActive(false);
            Button_Cancel.gameObject.SetActive(false);
            Button_Select.gameObject.SetActive(true);
            GO_Select_Text.gameObject.SetActive(false);
            _isSelectionActive = false;
        }

        public void Trigger_ButtonClick_Select()
        {
            ManagerSounds.PlayEffect("fx_page14");
            _isSelectionActive = true;
            Button_Select.gameObject.SetActive(false);
            Button_Delete.gameObject.SetActive(false);
            Button_Cancel.gameObject.SetActive(true);
            GO_Select_Text.gameObject.SetActive(true);
        }

        public override void Trigger_ButtonClick_Close()
        {
            Close();
            ManagerSounds.PlayEffect("fx_page17");
            base.Trigger_ButtonClick_Close();
        }

        private void Close()
        {
            StopAllCoroutines();
            CancelActon();
            for (int i = 0; i < _activeGallery.Count; i++)
            {
                _activeGallery[i].Clear();
                _itemGalleries.Enqueue(_activeGallery[i]);
            }

            _activeGallery.Clear();
        }

        private void CancelActon()
        {
            StopAllCoroutines();
            for (int i = 0; i < _arraySelectedItems.Count; i++)
            {
                _arraySelectedItems[i].CancelSelection();
            }
            _arraySelectedItems.Clear();
            _isSelectionActive = false;

            Button_Select.gameObject.SetActive(true);
            Button_Delete.gameObject.SetActive(false);
            Button_Cancel.gameObject.SetActive(false);
            GO_Select_Text.gameObject.SetActive(false);

        }

        public void Trigger_ButtonClick_Cancel()
        {
            ManagerSounds.PlayEffect("fx_page17");
            CancelActon();
        }

        private void CheckDirectoryPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void CleanGallery()
        {
            FileInfo[] arrayThumbnailFiles = new DirectoryInfo(_thumbnailPath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToArray();

            if (arrayThumbnailFiles.Length > _Instance.maxImageCount)
            {
                FileInfo[] arrayImageFiles = new DirectoryInfo(_imagePath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToArray();
                for (int i = _Instance.maxImageCount; i < arrayThumbnailFiles.Length; i++)
                {
                    if (_Instance.FindFile(arrayImageFiles, arrayThumbnailFiles[i].Name, out string imageName))
                    {
                        if (File.Exists(arrayThumbnailFiles[i].FullName))
                        {
                            File.Delete(arrayThumbnailFiles[i].FullName);
                        }

                        if (File.Exists(imageName))
                        {
                            File.Delete(imageName);
                        }
                    }
                }
            }
        }


        private void Recalculate()
        {
            _activeGallery.Clear();
            FileInfo[] arrayImageFiles = new DirectoryInfo(_imagePath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToArray();
            FileInfo[] arrayThumbnailFiles = new DirectoryInfo(_thumbnailPath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToArray();


            if (arrayThumbnailFiles.Length == 0)
            {
                GO_Gallery.SetActive(false);
                GO_GalleryEmpty.SetActive(true);
            }
            else
            {
                GO_Gallery.SetActive(true);
                GO_GalleryEmpty.SetActive(false);
            }

            if (arrayThumbnailFiles.Length > 9)
            {
                Text_Warning.gameObject.SetActive(true);
                Text_Warning.text = $"Clear old images, total size: {(int)(arrayImageFiles.Length * 0.15f)} MB";
            }
            else
            {
                Text_Warning.gameObject.SetActive(false);
            }

            for (int i = 0; i < arrayThumbnailFiles.Length; i++)
            {
                if (FindFile(arrayImageFiles, arrayThumbnailFiles[i].Name, out string imageName))
                {
                    GalleryElement galleryElementInstance = CreateGalleryImage(imageName, arrayThumbnailFiles[i].FullName);
                    _activeGallery.Add(galleryElementInstance);
                }
            }

            StartCoroutine(MoveScrollUp());
        }

        IEnumerator MoveScrollUp()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            scrollRect.verticalNormalizedPosition = 1;
        }

        public bool FindFile(FileInfo[] arrayInfo, string fileName, out string fullPath)
        {
            for (int i = 0; i < arrayInfo.Length; i++)
            {
                if (arrayInfo[i].Name == fileName)
                {
                    fullPath = arrayInfo[i].FullName;
                    return true;
                }
            }
            fullPath = string.Empty;
            return false;
        }

        private GalleryElement CreateGalleryImage(string imageFilePath, string thumbnailPath)
        {
            GalleryElement galleryElementInstance;
            if (_itemGalleries.Count > 0)
            {
                galleryElementInstance = _itemGalleries.Dequeue();
            }
            else
            {
                galleryElementInstance = GameObject.Instantiate(galleryElementPrefab, parentTransform);


            }
            galleryElementInstance.Generate(imageFilePath, thumbnailPath);
            galleryElementInstance.OnElementClickEvent = Trigger_ButtonClick_ClickItem;
            return galleryElementInstance;
        }

        private void OnDestroy()
        {
            _Instance = null;
        }
    }
}