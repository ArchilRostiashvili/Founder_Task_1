using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class OthersButton : MonoBehaviour
    {
        public ButtonPhysics button;
        public Transform TR_Content;
        public Transform TR_ContainerPrefab;
        public SpriteRenderer SR_Banner;
        private Animator _animPrefab;

        public Collider2D collider1;
        public Collider2D collider2;

        private float _bannerBoundsHeight;
        private bool _isBannerSet;

        public void Activate(bool anim)
        {
            if (!_isBannerSet)
            {
                return;
            }

            this.gameObject.SetActive(true);

            this.TR_Content.DOKill();
            /*
            if (ManagerApp.dataServerParams.othersState == 2)
            {
                if (_animPrefab != null)
                {
                    this.StartCoroutine(this.PlayAnim());
                }
            }
            */
            if (anim)
            {
                this.TR_Content.localPosition = new Vector3(0.0f, _bannerBoundsHeight + 1.0f, 0.0f);
                this.TR_Content.DOLocalMoveY(0.0f, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                {

                });
            }
            else
            {
                this.TR_Content.localPosition = Vector3.zero;
            }
        }

        public void AddPrefab(GameObject prefab)
        {
            _bannerBoundsHeight = 3.66f;
            this.SR_Banner.gameObject.SetActive(false);
            this.TR_ContainerPrefab.gameObject.SetActive(true);
            for (int i = 0; i < this.TR_ContainerPrefab.childCount; i++)
            {
                GameObject.Destroy(this.TR_ContainerPrefab.GetChild(i));
            }

            if(prefab != null)
            {
                _isBannerSet = true;
                GameObject go = GameObject.Instantiate(prefab, this.TR_ContainerPrefab);
                go.transform.localPosition = new Vector3(0.0f, -2.52f, 0.0f);
                _animPrefab = go.GetComponent<Animator>();
                this.button.SetCollider(this.collider2);
            }
            else
            {
                _isBannerSet = false;
            }
        }

        public void SetSprite(Sprite sprite)
        {
            if (sprite != null)
            {
                this.SR_Banner.gameObject.SetActive(true);
                this.TR_ContainerPrefab.gameObject.SetActive(false);
                this.SR_Banner.sprite = sprite;
                _bannerBoundsHeight = this.SR_Banner.bounds.size.y;
                this.SR_Banner.transform.localPosition = new Vector3(this.SR_Banner.transform.localPosition.x, -_bannerBoundsHeight * 0.5f, 0.0f);
                this.button.SetCollider(this.collider1);
                _isBannerSet = true;

                //iphone - 3.33f 
            }
            else
            {
                _isBannerSet = false;
            }
        }

        private void OnDisable()
        {
            this.Hide(false);
        }

        public void Hide(bool anim)
        {
            if (!_isBannerSet)
            {
                this.TR_Content.DOKill();
                this.gameObject.SetActive(false);
                return;
            }

            this.TR_Content.DOKill();
            this.StopAllCoroutines();
            if (anim)
            {
                this.TR_Content.DOLocalMoveY(_bannerBoundsHeight + 1.0f, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    this.gameObject.SetActive(false);
                });
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }

        private IEnumerator PlayAnim()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3.0f, 4.5f));
            if (_animPrefab != null)
            {
                _animPrefab.SetTrigger("Show");
            }
            this.StartCoroutine(this.PlayAnim());
        }
    }
}
