using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace FarmLife.MiniGames.TractorFruit
{
    public class FruitUi : MonoBehaviour
    {
        [SerializeField] private Image _fruitImage;
        [SerializeField] private Image _checkMarkImage;


        public void SetData(Sprite fruitSprite)
            => _fruitImage.sprite = fruitSprite;

        public void Activate()
        {
            _fruitImage.SetAlpha(1f);
            _fruitImage.transform.DOPunchScale(1.1f * Vector3.one, 0.2f, 5, 0.3f);
        }

        public void SetInactive()
            => _fruitImage.SetAlpha(.4f);

        public void SetCheckMarkState(bool state)
        {
            _checkMarkImage.gameObject.SetActive(state);
            _checkMarkImage.transform.DOPunchScale(1.2f * Vector3.one, 0.5f);
        }
    }
}
