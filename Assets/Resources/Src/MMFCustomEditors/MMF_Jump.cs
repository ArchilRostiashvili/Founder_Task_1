using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback will allow you to jump object")]
    [FeedbackPath("Transform/Jump")]
    public class MMF_Jump : MMF_Feedbacks
    {
        [MMFInspectorGroup("Jump", true, 28, true)]
        [SerializeField] private GameObject _object;

        [SerializeField] private GameObject _targetObject;

        [SerializeField] private float _jumpDuration;
        [SerializeField] private float _jumpPower;

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (_object != null)
                _object.transform.DOJump(_targetObject.transform.position, _jumpPower, 1, _jumpDuration);
        }
    }
}