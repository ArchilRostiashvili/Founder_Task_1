using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace Pets
{
    public class ManagerDirt : MonoBehaviour
    {
        [SerializeField]
        private int _index;
        [SerializeField]
        private float _alpha;
        public SpriteRenderer[] ID_arrayDirt;
        public System.Action<bool> OnDirtyAction;
        public void Init()
        {
            _index = 0;
            _alpha = 1f;
        }
        public void SetDirt(float time)
        {
            if (_index < this.ID_arrayDirt.Length)
            {
                _alpha = 1f;
                this.ID_arrayDirt[_index].DOKill();
                this.ID_arrayDirt[_index].DOFade(1f, time).SetEase(Ease.Linear).OnComplete(() =>
                {
                    _index++;
                });
            }
        }

        public void UnsetDirt(float alpha, float time)
        {
            _alpha = alpha;
            for (int i = 0; i < _index; i++)
            {
                this.ID_arrayDirt[i].DOFade(alpha, time).OnComplete(() =>
                {
                    if (alpha <= 0.1f)
                    {
                        _index = 0;
                    }
                });
            }
        }

        public void SetDirtAll()
        {
            for (int i = 0; i < this.ID_arrayDirt.Length; i++)
            {
                this.ID_arrayDirt[i].SetAlpha(1f);
                _index = this.ID_arrayDirt.Length;
            }
        }


        public void Check()
        {
            if (_index == this.ID_arrayDirt.Length)
            {
                this.OnDirtyAction?.Invoke(true);
            }
        }
        
        public void SetDirtParent()
        {
            for (int i = 0; i < this.ID_arrayDirt.Length; i++)
            {
                this.ID_arrayDirt[i].transform.SetParent(this.transform);
                this.ID_arrayDirt[i].transform.localPosition = Vector3.zero;
                this.ID_arrayDirt[i].gameObject.SetActive(false);
                this.ID_arrayDirt[i].transform.DOKill();
            }
        }
        public void SetDirtNewScene(int index, float alpha = 1f)
        {
            if ((index >= this.ID_arrayDirt.Length || index == 0) && alpha <= 0.1f)
            {
                index = this.ID_arrayDirt.Length;
                _index = 0;
                this.OnDirtyAction?.Invoke(false);
            }
            else
            {
                _index = index;
            }

            _alpha = alpha;
            for (int i = 0; i < index; i++)
            {
                this.ID_arrayDirt[i].DOFade(alpha, 0.01f);
            }
        }

        public int GetIndex()
        {
            return _index;
        }

        public float GetAlpha()
        {
            return _alpha;
        }
    }
}