using System;
using System.Collections;
using System.Collections.Generic;
using Bebi.FarmLife;
using UnityEngine;

namespace FarmLife.Data.LobbySession
{
    public class LobbySessionData : ScriptableObject
    {
        public Vector3 LastParallaxContentPosition;

        public List<EasterEggData> EasterEggDataList = new List<EasterEggData>();
        public List<RandomContentDataContainer> ContentDataList = new List<RandomContentDataContainer>();
        public List<RandomAnimalIdentifier> RandomAnimalIdentifiers = new List<RandomAnimalIdentifier>();

        public void AddEggData(Sprite eggSprite, EggPositionData eggPositionData, FarmAnimalAnimator randomAnimal, int index)
        {
            EasterEggDataList.Add(new EasterEggData(eggSprite, eggPositionData, randomAnimal, index));
        }

        public void AddRandomContentData(RandomContentPositionData randomContentPositionData, string ID)
        {
            RandomContentDataContainer randomContentDataContainer = ContentDataList.Find((x) => x.RandomContentID == ID);

            if (randomContentDataContainer == null)
            {
                randomContentDataContainer = new RandomContentDataContainer(ID);
                randomContentDataContainer.contentPositionList.Add(randomContentPositionData);
                ContentDataList.Add(randomContentDataContainer);
                return;
            }
            else
                randomContentDataContainer.contentPositionList.Add(randomContentPositionData);
        }

        public void SetCrackedEggIndex(int index)
        {
            EasterEggData eggData = EasterEggDataList.Find((x) => x.Index == index);

            if (eggData == null)
            {
                Debug.LogError($"couldnt find egg with index{index}");
                return;
            }

            eggData.IsCracked = true;
        }

        public void SetAnimalData(string ContentID, string randomAnimalID)
        {
            RandomAnimalIdentifier randomAnimalIdentifier = RandomAnimalIdentifiers.Find((x) => x.ContentIdentifier == ContentID);

            if (randomAnimalIdentifier == null)
            {
                randomAnimalIdentifier = new RandomAnimalIdentifier(ContentID, randomAnimalID);
                RandomAnimalIdentifiers.Add(randomAnimalIdentifier);
                return;
            }
            else
            {
                randomAnimalIdentifier.ContentIdentifier = ContentID;
                randomAnimalIdentifier.AnimalIdentifier = randomAnimalID;
            }
        }
    }

    [Serializable]
    public class RandomContentDataContainer
    {
        public string RandomContentID;
        public List<RandomContentPositionData> contentPositionList = new List<RandomContentPositionData>();

        public RandomContentDataContainer(string iD)
        {
            RandomContentID = iD;
        }
    }

    public class RandomAnimalIdentifier
    {
        public string ContentIdentifier;
        public string AnimalIdentifier;

        public RandomAnimalIdentifier(string contentID, string randomAnimalID)
        {
            ContentIdentifier = contentID;
            AnimalIdentifier = randomAnimalID;
        }
    }
}