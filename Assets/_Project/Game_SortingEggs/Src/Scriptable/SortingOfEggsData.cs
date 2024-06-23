using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmLife.MiniGames.SortingOfEggs
{
    [CreateAssetMenu(fileName = "SortingOfEggsData", menuName = "FarmLife/Data/Minigames/SortingOfEgg")]
    public class SortingOfEggsData : MiniGameBaseData
    {
        [SerializeField] private List<EggSizeData> _eggSizesList = new List<EggSizeData>();
        [SerializeField] private List<EggColorData> _colorDataList = new List<EggColorData>();
        [SerializeField] private bool _isColoredGamePlay;

        public void CreateContent(List<DraggableEggHolder> draggableEggList, List<ItemNest> nestList)
        {
            if (_isColoredGamePlay)
                CreateColoredEggs(draggableEggList, nestList);
            else
                CreateSizedEggs(draggableEggList, nestList);
        }

        private void CreateSizedEggs(List<DraggableEggHolder> draggableEggList, List<ItemNest> nestList)
        {
            int divider = 5;
            List<EggSizeData> eggSizeDataList = new List<EggSizeData>();
            for (int i = 0; i < draggableEggList.Count; i++)
            {
                if (i < divider)
                    eggSizeDataList.Add(_eggSizesList[0]);
                else
                    eggSizeDataList.Add(_eggSizesList[1]);
            }

            Utils.Shuffle(ref eggSizeDataList);

            for (int i = 0; i < draggableEggList.Count; i++)
            {
                draggableEggList[i].DraggableEgg.SetSizeData(eggSizeDataList[i].Size, eggSizeDataList[i].SizeID);
            }

            for (int i = 0; i < nestList.Count; i++)
            {
                nestList[i].SetProgressBarCounter(divider);
                nestList[i].SetID(_eggSizesList[i].SizeID);
            }
        }

        private void CreateColoredEggs(List<DraggableEggHolder> draggableEggList, List<ItemNest> nestList)
        {
            int divider = draggableEggList.Count / nestList.Count;
            int index = 0;
            int helpingIndex = 0;

            List<EggColorData> eggsColorDataList = new List<EggColorData>();
            List<EggColorData> eggsColorDataTempList = new List<EggColorData>();

            Utils.Shuffle(ref _colorDataList);

            for (int i = 0; i < nestList.Count; i++)
            {
                eggsColorDataTempList.Add(_colorDataList[i]);
            }

            for (int i = 0; i < draggableEggList.Count; i++)
            {
                if (helpingIndex >= divider)
                {
                    helpingIndex = 0;
                    index++;
                }

                eggsColorDataList.Add(eggsColorDataTempList[index]);
                helpingIndex++;
            }

            Utils.Shuffle(ref eggsColorDataList);

            for (int i = 0; i < draggableEggList.Count; i++)
            {
                draggableEggList[i].DraggableEgg.SetColorData(eggsColorDataList[i].EggColor, eggsColorDataList[i].ColorID, eggsColorDataList[i].audioTrack);
            }

            Utils.Shuffle(ref eggsColorDataTempList);
            for (int i = 0; i < nestList.Count; i++)
            {
                nestList[i].SetColoredProgressBarData(eggsColorDataTempList[i].EggColor, divider);
                nestList[i].SetColorData(eggsColorDataTempList[i].EggColor);
                nestList[i].SetID(eggsColorDataTempList[i].ColorID);
            }
        }
    }
}