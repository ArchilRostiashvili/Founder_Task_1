using DG.Tweening;
using UnityEngine;

namespace Libs_Mirian
{
    public class InteractableBase : MonoBehaviour
    {

        [HideInInspector] public Collider2D col;
        [HideInInspector] public bool isInteracted;
        [HideInInspector] public string ID;
        protected Vector2 touchPosition;
        protected Vector2 touchDelta;
        protected Vector2 touchOffset;
        protected Vector2 deltaSum;

        public bool selfInit = true;
        public int interactableLayer;

        private bool _interactCondition = true;
        private void Start()
        {

            if (this.selfInit) this.Init();
        }

        public virtual void Init()
        {
            col = this.GetComponent<Collider2D>();
            ManagerInteractions.Register(this);
        }
        public virtual void Run()
        {
        }

        public bool GetInteractCondition()
        {
            return _interactCondition;
        }

        protected void SetInteractableCondition(bool condition)
        {
            _interactCondition = condition;
        }
        public virtual void OnInteractBegin(Vector2 touchPosition)
        {
            if (!_interactCondition) return;
            this.isInteracted = true;
            this.touchPosition = touchPosition;

            this.touchOffset = touchPosition - (Vector2)this.transform.position;
        }

        public virtual void OnInteractMove(Vector2 touchPosition)
        {
            this.touchDelta = touchPosition - this.touchPosition;
            this.touchPosition = touchPosition;

            this.deltaSum += this.touchDelta;
        }

        public virtual void OnInteractStatic(Vector2 touchPosition)
        {
            this.touchDelta = Vector2.zero;
        }

        public virtual void OnInteractEnd()
        {
            this.isInteracted = false;

            this.deltaSum = Vector2.zero;
        }

        public virtual void Clear()
        {
            this.transform.DOKill();
            this.StopAllCoroutines();
        }
    }
}