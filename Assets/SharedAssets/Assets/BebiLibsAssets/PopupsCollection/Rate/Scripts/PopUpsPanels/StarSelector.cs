using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace BebiLibs
{
    public class StarSelector : MonoBehaviour
    {
        public static System.Action<int> CallbackOnStarSelect;
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Sprite _deselectedSprite;

        [System.Serializable]
        public class DataStarButton
        {
            public Image star;
            public AudioClip audioClip;
            public ButtonHoverActive buttonHoverActive;
        }
        public List<DataStarButton> arrayStars = new List<DataStarButton>();

        private void OnEnable()
        {
            for (int i = 0; i < this.arrayStars.Count; i++)
            {
                this.arrayStars[i].star.sprite = this._deselectedSprite;
            }
        }

        public void Trigger_ButtonClick_OnStarSelect(Image star)
        {
            int index = this.arrayStars.FindIndex(x => x.star == star);

            ManagerSounds.PlayEffect(this.arrayStars[index].audioClip);
            for (int i = 0; i < this.arrayStars.Count; i++)
            {
                this.arrayStars[i].star.sprite = i <= index ? this._selectedSprite : this._deselectedSprite;
            }
            CallbackOnStarSelect?.Invoke(index + 1);
        }
    }
}
