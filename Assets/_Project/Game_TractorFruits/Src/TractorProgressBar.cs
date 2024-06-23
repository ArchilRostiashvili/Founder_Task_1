using System.Collections;
using System.Collections.Generic;
using BebiLibs;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FarmLife
{
    public class TractorProgressBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        [SerializeField] private List<ItemSpriteSizer> _fruitSRsList = new List<ItemSpriteSizer>();

        private int _activatedFruitCounter;

        public void SetData(List<Sprite> spritesList)
        {
            for (int i = 0; i < spritesList.Count; i++)
            {
                _fruitSRsList[i].SetSprite(spritesList[i]);
            }
        }
        
        public void ProgressUp()
        {
            _slider.value += 1 / 30f;
        }

        public void ActivateFruit()
        {
            if (_activatedFruitCounter >= _fruitSRsList.Count)
                return;

            _fruitSRsList[_activatedFruitCounter].gameObject.SetActive(true);
            _activatedFruitCounter++;
        }
    }
}