using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FarmLife
{
    public class BulletProgressBar : MonoBehaviour
    {
        [SerializeField] GameObject GO_Content;
        [SerializeField] GameObject itemBulletProgressBar;

        private List<ItemBullet> _itemBulletProgressBarList = new List<ItemBullet>();

        public void SetEnabled(bool val) => this.GO_Content.SetActive(val);

        public void SetProgress(int currentCount, int maxCount)
        {
            _itemBulletProgressBarList.Clear();

            int roundIndex = currentCount - 2;

            if (!this.GO_Content.activeSelf) this.SetEnabled(true);

            foreach (Transform item in this.GO_Content.transform)
            {
                Destroy(item.gameObject);
            }

            for (int i = 1; i <= maxCount; i++)
            {
                ItemBullet item = Instantiate(this.itemBulletProgressBar, this.GO_Content.transform)
                    .GetComponent<ItemBullet>();
                _itemBulletProgressBarList.Add(item);
                item.SetIsActive(i < currentCount);
            }

            if (roundIndex > 0)
            {
                _itemBulletProgressBarList[roundIndex].transform.DOScale(Vector3.one * 2f, 0.15f).OnComplete(() =>
                {
                    _itemBulletProgressBarList[roundIndex].transform.DOScale(Vector3.one, 0.15f);
                });
            }
            else
            {
                _itemBulletProgressBarList[0].transform.DOScale(Vector3.one * 2f, 0.15f).OnComplete(() =>
                {
                    _itemBulletProgressBarList[0].transform.DOScale(Vector3.one, 0.15f);
                });
            }
        }
    }
}