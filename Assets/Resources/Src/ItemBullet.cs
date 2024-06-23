using UnityEngine;
using UnityEngine.UI;

namespace FarmLife
{
    public class ItemBullet : MonoBehaviour
    {
        [SerializeField] Image _image;
        [SerializeField] Sprite _activeSprite;
        [SerializeField] Sprite _inactiveSprite;

        public void SetIsActive(bool val)
        {
            _image.sprite = val ? _activeSprite : _inactiveSprite;
        }
    }
}