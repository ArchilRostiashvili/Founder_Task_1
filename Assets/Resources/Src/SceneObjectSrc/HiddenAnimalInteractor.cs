using System.Collections.Generic;
using UnityEngine;

namespace FarmLife
{
    public class HiddenAnimalInteractor : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _animalsList = new List<GameObject>();

        public void ShowRandomAnimal()
        {
            foreach (GameObject obj in _animalsList)
            {
                obj.SetActive(true);
            }

            GameObject randAnimal = _animalsList.GetRandomElement();
            randAnimal.GetComponentInChildren<FeelAnimator>().Play(AnimationNamesData.ANIM_SHOW);

            foreach (GameObject obj in _animalsList)
            {
                if (obj != randAnimal)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}