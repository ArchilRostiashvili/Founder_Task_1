using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D.Animation;
using MiniGames.RocketAdventure;

namespace MiniGames
{
    [System.Serializable]
    public class ItemObject : MonoBehaviour
    {
        public List<string> arraySounds;
        public string elementName;

        public ElementData elementData;
        public Vector3 scale = new Vector3(1, 1, 1);
        public Vector2 positiion = new Vector2(0, -5.6f);

        public SpriteRenderer SR_Shadow;

        public Maps objectMap;
        //[HideInInspector] public int indexInPuzzleArray = 0;
        public int indexInPuzzleArray = 0;


        public string GetSoundName()
        {
            if (this.arraySounds.Count > 0)
            {
                return this.arraySounds[Random.Range(0, arraySounds.Count)];
            }
            else
            {
                //Common.DebugLog("there is no audio");
                return "";
            }
        }

        public void DisableShadow()
        {
            if (this.SR_Shadow)
            {
                this.SR_Shadow.gameObject.SetActive(false);
            }
        }

        public void EnableShadow()
        {
            if (this.SR_Shadow)
            {
                this.SR_Shadow.gameObject.SetActive(true);
            }
        }
    }
}


[System.Serializable]
public enum Maps
{
    Rockets
}
