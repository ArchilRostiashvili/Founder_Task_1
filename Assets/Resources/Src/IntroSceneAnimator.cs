using BebiAnimations.Libs.Core;
using UnityEngine;

namespace FarmLife
{


    public class IntroSceneAnimator : MonoBehaviour
    {
        [SerializeField] private BebiAnimator _bebiAnimator;

        [SerializeField] private string _introAnimationID;

        private void Awake()
        {
            if (_bebiAnimator == null)
                _bebiAnimator = GetComponent<BebiAnimator>();

            if (_introAnimationID == "")
                _introAnimationID = AnimationNamesData.ANIM_SHOW;

            _bebiAnimator.Play(_introAnimationID);
        }
    }
}