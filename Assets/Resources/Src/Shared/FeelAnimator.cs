using System;
using System.Collections;
using System.Collections.Generic;
using BebiLibs.AudioSystem;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FeelAnimator : MonoBehaviour
{
    [SerializeField] private List<MMF_Player> _MMFPlayersList;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private bool _waitForCallbackDuration;

    // To reference Play with parameter into UnityEvent

    public void InitializeFeedbackPlayers()
    {
        for (int i = 0; i < _MMFPlayersList.Count; i++)
        {
            if (_MMFPlayersList[i].InitializationMode == MMFeedbacks.InitializationModes.Script)
                _MMFPlayersList[i].Initialization();
        }
    }

    public void Play(string animationID)
    {
        Play(animationID, null);
    }

    public void Play(string animationID, Action callback, bool logWarning = true)
    {
        MMF_Player currentPlayer = CurrentAnimation(animationID, logWarning);

        if (currentPlayer != null)
        {
            currentPlayer.PlayFeedbacks();
        }
        else if (logWarning)
        {
            LogActionNotFountWarning(animationID, "animationID doesn't exist in list");
        }

        if (callback != null)
        {
            if (currentPlayer != null)
            {
                MMF_Feedback eventFeedback = currentPlayer.FeedbacksList.Find(x => x.Label == "Unity Events");

                float additionalDuration = 0;

                if (eventFeedback != null && _waitForCallbackDuration)
                {
                    additionalDuration += 2.5f;
                }

                StartCoroutine(SendCallback(callback, currentPlayer.TotalDuration + additionalDuration));
            }
        }
    }

    public void Stop(string animationID, bool stop = true)
    {
        MMF_Player currentPlayer = CurrentAnimation(animationID);
        if (currentPlayer != null)
        {
            currentPlayer.StopFeedbacks(stop);
        }
        else
        {
            LogActionNotFountWarning(animationID, "animationID can't be stopped");
        }
    }

    public void Disable(string animationID)
    {
        MMF_Player currentPlayer = CurrentAnimation(animationID);
        if (currentPlayer != null)
        {
            currentPlayer.enabled = false;
            currentPlayer.gameObject.SetActive(false);
        }
        else
        {
            LogActionNotFountWarning(animationID, "animationID can't be stopped");
        }
    }

    public void DeleteAnimation(string animationID)
    {
        MMF_Player currentPlayer = CurrentAnimation(animationID);

        if (currentPlayer != null && _MMFPlayersList.Contains(currentPlayer))
        {
            _MMFPlayersList.Remove(currentPlayer);
        }
    }

    private IEnumerator SendCallback(Action callback, float delay)
    {
        if (!gameObject.activeInHierarchy)
            yield break;

        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

    private void LogActionNotFountWarning(string actionID, string error)
    {
        Debug.LogWarning($"{actionID} {error}");
    }

    public void SetAudio(AudioTrackBaseSO audioTrack, string animationID)
    {
        MMF_Player currentPlayer = CurrentAnimation(animationID);

        if (currentPlayer != null)
        {
            currentPlayer.FeedbacksList.ForEach(x =>
            {
                if (x is MMF_AudioTrack audioTrackFeedback)
                {
                    audioTrackFeedback.SetAudio(audioTrack);
                    Debug.Log("AudioTrack setted");
                }
            });
        }
    }

    public void SetEvent(UnityEvent currentEvent, string animationID)
    {
        MMF_Player currentPlayer = CurrentAnimation(animationID);

        if (currentPlayer != null)
        {
            currentPlayer.FeedbacksList.ForEach(x =>
            {
                if (x is MMF_Events currentEventFeedback)
                {
                    currentEventFeedback.PlayEvents = currentEvent;
                    Debug.Log("Event setted");
                }
            });
        }
    }

    public void SetTransform(Transform tr, string animationID)
    {
        MMF_Player currentPlayer = CurrentAnimation(animationID);

        if (currentPlayer != null)
        {
            currentPlayer.FeedbacksList.ForEach(x =>
            {
                if (x is MMF_Scale currentFeedback)
                {
                    currentFeedback.AnimateScaleTarget = tr;
                    Debug.Log("Event setted");
                }
            });
        }
    }

    public bool HasAnimation(string animationID)
        => _MMFPlayersList.Find(x => x.name == animationID);

    private MMF_Player CurrentAnimation(string animationID, bool logWarning = true)
    {
        MMF_Player player = _MMFPlayersList.Find(x => x.name == animationID);
        if (player == null)
        {
            if (logWarning)
                Debug.LogWarning($"No MMF player by the name of {animationID} available on {name}");
            return null;
        }

        return player;
    }

    public bool IsAnimationPlaying(string animationID)
    {
        MMF_Player player = _MMFPlayersList.Find(x => x.name == animationID);
        if (player == null)
        {
            Debug.LogWarning($"No MMF player available on {name}");
            return false;
        }

        return player.IsPlaying;
    }

    public void ChangeLayerOrder(int order)
    {
        if (_spriteRenderer != null) _spriteRenderer.sortingOrder = order;
    }
}