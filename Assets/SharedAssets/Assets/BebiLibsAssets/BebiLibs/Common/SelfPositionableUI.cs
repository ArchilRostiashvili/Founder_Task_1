
using UnityEngine;

namespace BebiLibs
{
    [ExecuteInEditMode]
    public class SelfPositionableUI : MonoBehaviour
    {
        //[SerializeField] private int _designedHeight;
        //[SerializeField] private int _designedWidth;

        public Vector2 position;
        public Vector2 anchor;

        private Vector2 origin;
        private Vector2 target;

        private void Awake()
        {

            if (Application.isPlaying)
            {
                this.origin = new Vector2
                (
                    (this.anchor.x - 0.5f) * Screen.width * SelfPositionableUI.PPU,
                    (this.anchor.y - 0.5f) * Screen.height * SelfPositionableUI.PPU
                );

                this.transform.position = this.origin + this.position;
            }
        }

        public void ChangePosition(Vector2 p)
        {
            this.position = p;
            this.origin = new Vector2
               (
                   (this.anchor.x - 0.5f) * Screen.width * SelfPositionableUI.PPU,
                   (this.anchor.y - 0.5f) * Screen.height * SelfPositionableUI.PPU
               );

            this.transform.localPosition = this.origin + this.position;
        }


        private static float _oldOrtho;
        private static int _oldHeight;
        private static float _oldPpu;
        public static float PPU
        {
            get
            {
                if (Camera.main != null)
                {
                    if (Camera.main.orthographicSize != _oldOrtho || Screen.height != _oldHeight)
                    {
                        _oldPpu = (Camera.main.orthographicSize * 2f) / (float)Screen.height;
                        _oldOrtho = Camera.main.orthographicSize;
                        _oldHeight = Screen.height;
                    }
                }
                else
                {
                    return 0;
                }

                return _oldPpu;
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            UpdatePosition();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;

            Gizmos.DrawSphere(origin, 0.1f);
            Gizmos.DrawWireSphere(target, 0.1f);
            Gizmos.DrawLine(origin, target);
        }

        private void UpdatePosition()
        {
            if (!Application.isPlaying)
            {
                //_designedHeight = Screen.height;
                //_designedWidth = Screen.width;
                origin = new Vector2((anchor.x - 0.5f) * Screen.width * SelfPositionableUI.PPU, (anchor.y - 0.5f) * Screen.height * SelfPositionableUI.PPU);
                target = origin + position;
                transform.position = target;
            }
        }
#endif
    }
}