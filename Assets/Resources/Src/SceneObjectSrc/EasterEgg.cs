using System;
using System.Collections;
using System.Collections.Generic;
using Bebi.FarmLife;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Rendering;

namespace FarmLife
{
    public class EasterEgg : MonoBehaviour
    {
        public Action<int> EggCrackEvent;

        [field: SerializeField] public List<SpriteRenderer> EggRenderersList { get; private set; }

        [SerializeField] private GameObject _normalObj;
        [SerializeField] private GameObject _shadowGO;
        [SerializeField] private GameObject _crackedObj;
        [SerializeField] private GameObject _waterShadow;
        [SerializeField] private Transform _animalSpawnTR;
        [SerializeField] private Transform _spawnTR;
        [SerializeField] private SortingGroup _eggSortingGroup;

        private int _eggIndex;
        private FarmAnimalAnimator _animalGO;

        public void CrackEgg()
        {
            _normalObj.SetActive(false);
            _crackedObj.SetActive(true);
            EggCrackEvent?.Invoke(_eggIndex);
            StartCoroutine(ShowAnimal());
        }

        public void SetData(FarmAnimalAnimator randomAnimal, Sprite sprite, int index, bool enableShadow)
        {
            _animalGO = Instantiate(randomAnimal, _animalSpawnTR);
            _animalGO.transform.localPosition = Vector2.zero;
            _animalGO.transform.localScale = Vector2.one;
            _animalGO.FarmAnimator.transform.localScale = Vector2.one;
            _animalGO.EnableShadow(enableShadow);
            _eggIndex = index;
            SetSprite(sprite);

            _spawnTR.transform.localScale = Vector2.zero;
        }

        public void SetLayerOrder(int eggLayerOrder)
        {
            _eggSortingGroup.sortingOrder = eggLayerOrder;
        }

        private void SetSprite(Sprite sprite)
        {
            foreach (SpriteRenderer sr in EggRenderersList)
            {
                sr.sprite = sprite;
            }
        }

        public void CheckIfCracked(bool isCracked)
        {
            if (!isCracked)
                return;

            _normalObj.SetActive(false);
            _spawnTR.transform.localScale = Vector2.one;
        }

        IEnumerator ShowAnimal()
        {
            yield return new WaitForSeconds(2f);

            _animalGO.gameObject.SetActive(true);
        }

        internal void EnableShadow(bool isInWater)
        {
            if (_shadowGO != null)
                _shadowGO.SetActive(!isInWater);
        }

        internal void EnableWaterShadow(bool isInWater)
        {
            if (_waterShadow != null)
                _waterShadow.SetActive(isInWater);
        }
    }
}