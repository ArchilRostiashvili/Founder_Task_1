using System;
using System.Collections;
using System.Collections.Generic;
using Bebi.FarmLife;
using FarmLife;
using FarmLife.Data.LobbySession;
using UnityEngine;

public class EasterEggController : MonoBehaviour
{
    [SerializeField] private List<EasterEgg> _easterEggsList = new List<EasterEgg>();
    [SerializeField] private List<Sprite> _eggsSpritesList = new List<Sprite>();
    [SerializeField] private List<FarmAnimalAnimator> _animalDataList = new List<FarmAnimalAnimator>();
    [SerializeField] private List<EggPositionData> _eggPositionDataList = new List<EggPositionData>();

    private LobbySessionData _lobbySessionData;

    public void Init()
    {
        _easterEggsList.ForEach((x) => x.EggCrackEvent = OnEggCracked);
    }

    public void ActivateEggs()
    {
        Utils.Shuffle(ref _eggPositionDataList);
        ActivateEasterEggs();
    }

    public void SetSessionData(LobbySessionData lobbySessionData)
        => _lobbySessionData = lobbySessionData;

    public void ActivateSession(LobbySessionData lobbySessionData)
    {
        _lobbySessionData = lobbySessionData;

        for (int i = 0; i < _easterEggsList.Count; i++)
        {
            EggPositionData eggPositionData = lobbySessionData.EasterEggDataList[i].EggPositionData;
            EasterEggData easterEggData = lobbySessionData.EasterEggDataList[i];
            eggPositionData = _eggPositionDataList.Find((x) => x.ID == eggPositionData.ID);

            CreateEggs(i, easterEggData.EggSprite, easterEggData.RandomAnimalGO, eggPositionData);
            _easterEggsList[i].CheckIfCracked(easterEggData.IsCracked);
        }
    }

    private void ActivateEasterEggs()
    {
        _easterEggsList.ForEach((x) => x.gameObject.SetActive(false));

        for (int i = 0; i < _easterEggsList.Count; i++)
        {
            Sprite eggSprite = _eggsSpritesList.GetRandomElement();
            FarmAnimalAnimator randomAnimal = _animalDataList.GetRandomElement();

            _eggPositionDataList[i].SetID(i);

            if (_eggPositionDataList[i].IsInWater)
            {
                randomAnimal = _animalDataList[0];
            }

            CreateEggs(i, eggSprite, randomAnimal, _eggPositionDataList[i]);
            _lobbySessionData.AddEggData(eggSprite, _eggPositionDataList[i], randomAnimal, i);
        }
    }

    private void CreateEggs(int index, Sprite eggSprite, FarmAnimalAnimator randomAnimal, EggPositionData eggPositionData)
    {
        _easterEggsList[index].transform.parent = eggPositionData.EggTRansform;
        _easterEggsList[index].transform.localPosition = Vector2.zero;
        _easterEggsList[index].gameObject.SetActive(true);
        _easterEggsList[index].SetData(randomAnimal, eggSprite, index, !eggPositionData.IsInWater);
        _easterEggsList[index].SetLayerOrder(eggPositionData.EggLayerOrder);
        _easterEggsList[index].EnableShadow(eggPositionData.IsInWater);
        _easterEggsList[index].EnableWaterShadow(eggPositionData.IsInWater);
    }

    private void OnEggCracked(int index)
    {
        if (index > _easterEggsList.Count)
        {
            Debug.LogError("index out of range");
            return;
        }

        _lobbySessionData.SetCrackedEggIndex(index);
    }
}