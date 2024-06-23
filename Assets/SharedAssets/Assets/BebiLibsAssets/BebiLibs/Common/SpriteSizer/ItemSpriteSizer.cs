using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BebiLibs;
using UnityEngine.Rendering;

namespace BebiLibs
{
    [ExecuteInEditMode]
    public class ItemSpriteSizer : MonoBehaviour
    {
        public bool DontRecalculateOnStart = false;
        public bool IsActive = true;

        public SpriteRenderer SR_Rect;
        public SpriteRenderer SR_Sprite;
        public SortingGroup SG_Main;

        public Vector2 rectSize;

        [Range(0.0f, 1.0f)]
        public float pivotX;
        [Range(0.0f, 1.0f)]
        public float pivotY;

        public PivotPointX pivotInnerX = PivotPointX.CENTER;
        public PivotPointY pivotInnerY = PivotPointY.CENTER;

        public enum PivotPointX
        {
            LEFT,
            CENTER,
            RIGHT,
        }

        public enum PivotPointY
        {
            BOTTOM,
            CENTER,
            TOP,
        }

        void Start()
        {
            if (!DontRecalculateOnStart)
            {
                this.rectSize = this.SR_Rect.size;
                if (Application.isPlaying)
                {
                    this.SR_Rect.sprite = null;
                    this.SetSprite(null);
                }
            }
        }

        public void SetXFlipped(bool val) => SR_Sprite.flipX = val;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(this.transform.position.x + pivotX, this.transform.position.y + pivotY), 0.1f);
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {

                this.SetSprite(null);
            }
        }

        public void SetColor(Color color)
        {
            this.SR_Sprite.color = color;
        }

        public void SetSprite(Sprite sp)
        {
            if (sp != null)
            {
                if (IsActive) this.gameObject.SetActive(true);
                this.SR_Sprite.sprite = sp;
            }

            this.SR_Rect.size = this.rectSize;

            Vector2 positionLocalRect = this.SR_Rect.size * (new Vector2(-this.pivotX + 0.5f, -this.pivotY + 0.5f));
            this.SR_Rect.transform.localPosition = positionLocalRect;

            if (this.SR_Sprite.sprite == null)
            {
                return;
            }
            /*
            float rectWidth = this.SR_Rect.size.x;
            float rectHeight = this.SR_Rect.size.y;

            float spriteWidth = this.SR_Sprite.sprite.bounds.size.x;
            float spriteHeight = this.SR_Sprite.sprite.bounds.size.y;

            float scaleWidth = rectWidth / spriteWidth;
            float scaleHeight = rectHeight / spriteHeight;


            if (scaleWidth < scaleHeight)
            {
                scale = scaleWidth;
            }
            else
            {
                scale = scaleHeight;
            }
            */
            float scale = this.GetScale(this.SR_Rect, this.SR_Sprite);

            this.SR_Sprite.transform.localScale = Vector2.one * scale;
            this.SR_Sprite.transform.localPosition = positionLocalRect;

            if (this.pivotInnerY == PivotPointY.BOTTOM)
            {
                positionLocalRect.y -= this.SR_Rect.size.y * 0.5f;
                positionLocalRect.y += this.SR_Sprite.sprite.bounds.size.y * 0.5f * scale;
                this.SR_Sprite.transform.localPosition = positionLocalRect;
            }
            else
            if (this.pivotInnerY == PivotPointY.TOP)
            {
                positionLocalRect.y += this.SR_Rect.size.y * 0.5f;
                positionLocalRect.y -= this.SR_Sprite.sprite.bounds.size.y * 0.5f * scale;
                this.SR_Sprite.transform.localPosition = positionLocalRect;
            }

            if (this.pivotInnerX == PivotPointX.LEFT)
            {
                positionLocalRect.x -= this.SR_Rect.size.x * 0.5f;
                positionLocalRect.x += this.SR_Sprite.sprite.bounds.size.x * 0.5f * scale;
                this.SR_Sprite.transform.localPosition = positionLocalRect;
            }
            else
            if (this.pivotInnerX == PivotPointX.RIGHT)
            {
                positionLocalRect.x += this.SR_Rect.size.x * 0.5f;
                positionLocalRect.x -= this.SR_Sprite.sprite.bounds.size.x * 0.5f * scale;
                this.SR_Sprite.transform.localPosition = positionLocalRect;
            }
        }

        public float GetScale(SpriteRenderer SR_rect, SpriteRenderer SR_sprite)
        {
            float rectWidth = SR_rect.size.x;
            float rectHeight = SR_rect.size.y;

            float spriteWidth = SR_sprite.sprite.bounds.size.x;
            float spriteHeight = SR_sprite.sprite.bounds.size.y;

            float scaleWidth = rectWidth / spriteWidth;
            float scaleHeight = rectHeight / spriteHeight;
            if (scaleWidth < scaleHeight)
            {
                return scaleWidth;
            }
            else
            {
                return scaleHeight;
            }
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}
