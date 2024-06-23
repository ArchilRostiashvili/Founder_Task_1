using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
using BebiLibs;

namespace EducationalGames.Helpers
{
    public static class SceneObjectsAnimator
    {
        private static List<GameObject> _objectsToAnimateList = new List<GameObject>();
        private static Vector3 _targetScale;
        private static float _scaleAnimationDuration;
        private static string _spawnAudio;
        private static MonoBehaviour _creator;

        public static void AnimateObjects<T>(MonoBehaviour creator, List<T> objectsList, Vector3 targetScale, float duration, string spawnAudio, bool playSound, bool isSequential = false, float sequentialAnimationDelay = 0f) where T : MonoBehaviour
        {
            var selection = objectsList.ToArray();
            _objectsToAnimateList = Array.ConvertAll(selection, item => item.gameObject).ToList();

            _targetScale = targetScale;
            _scaleAnimationDuration = duration;
            _spawnAudio = spawnAudio;
            _creator = creator;

            if (!isSequential)
            {
                if (playSound)
                {
                    ManagerSounds.PlayEffect(_spawnAudio);
                }

                foreach (GameObject item in _objectsToAnimateList)
                {
                    item.transform.DOScale(_targetScale, _scaleAnimationDuration);
                }
            }
            else
            {
                _creator.StartCoroutine(SequentialAnimation(sequentialAnimationDelay));
            }
        }

        public static void AnimateObjects(List<ObjectEffectsHandler> objectEffectHandlersList, string animationID, System.Action onComplete)
        {
            foreach (ObjectEffectsHandler effectsHandler in objectEffectHandlersList)
            {
                effectsHandler.PlayAnimation(animationID, onComplete, false);
            }
        }

        public static IEnumerator AnimateObjectsSequential(List<ObjectEffectsHandler> objectEffectHandlersList, string animationID, System.Action onComplete, float delay)
        {
            foreach (ObjectEffectsHandler effectsHandler in objectEffectHandlersList)
            {
                effectsHandler.PlayAnimation(animationID, onComplete);
                yield return new WaitForSeconds(delay);
            }

            onComplete?.Invoke();
        }

        public static IEnumerator AnimateObjects(List<ObjectEffectsHandler> animationHandlersList, float delay, string animationToPlay, System.Action onComplete = null)
        {
            foreach (ObjectEffectsHandler animation in animationHandlersList)
            {
                animation.PlayAnimation(animationToPlay, onComplete);
                yield return new WaitForSeconds(delay);
            }

            onComplete?.Invoke();
        }

        public static IEnumerator SequentialAnimation(float delay)
        {
            foreach (GameObject item in _objectsToAnimateList)
            {
                ManagerSounds.PlayEffect(_spawnAudio);
                item.transform.DOScale(_targetScale, _scaleAnimationDuration);
                yield return new WaitForSeconds(delay);
            }
        }

        public static void StopAllAnimations()
        {
            _creator.StopAllCoroutines();

            foreach (GameObject item in _objectsToAnimateList)
            {
                item.transform.DOKill();
            }
        }
    }
}