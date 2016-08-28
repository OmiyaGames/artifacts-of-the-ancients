﻿using UnityEngine;
using UnityEngine.UI;
using OmiyaGames;

[RequireComponent(typeof(Animator))]
public class SpokenDialog : IMenu
{
    [SerializeField]
    Text nameLabel;
    [SerializeField]
    Text speechLabel;
    [SerializeField]
    RectTransform panel;
    [SerializeField]
    GameObject skipButton;
    [SerializeField]
    GameObject nextButton;

    RectTransform cacheTransform = null;
    Vector2? targetAnchorPosition = null;

    int speechIndex = 0;
    SpokenSpeech currentSpeech = null;

    #region Properties
    public RectTransform CachedTransform
    {
        get
        {
            if(cacheTransform == null)
            {
                cacheTransform = transform as RectTransform;
            }
            return cacheTransform;
        }
    }

    public override Type MenuType
    {
        get
        {
            return Type.ManagedMenu;
        }
    }

    public override GameObject DefaultUi
    {
        get
        {
            return nextButton;
        }
    }

    public Vector2 TargetAnchorPosition
    {
        get
        {
            if(targetAnchorPosition.HasValue == true)
            {
                return targetAnchorPosition.Value;
            }
            else
            {
                return CachedTransform.anchoredPosition;
            }
        }
        set
        {
            targetAnchorPosition = value;
        }
    }

    public bool Highlight
    {
        get
        {
            return (CurrentState == State.Visible);
        }
        set
        {
            if(CurrentState != State.Hidden)
            {
                if(value == true)
                {
                    CurrentState = State.Visible;
                }
                else
                {
                    CurrentState = State.StandBy;
                }
            }
        }
    }
    #endregion

    #region Animation Events
    public void UpdateText()
    {
        nameLabel.text = currentSpeech.AllSpeeches[speechIndex].Name;
        speechLabel.text = currentSpeech.AllSpeeches[speechIndex].Dialog;
    }
    public void UpdateState()
    {
        CurrentState = State.Visible;
    }
    #endregion

    public void ShowSpeech(SpokenSpeech newSpeech, bool showSkip)
    {
        if (currentSpeech != null)
        {
            skipButton.SetActive(showSkip);
            currentSpeech = newSpeech;
            speechIndex = 0;
            nameLabel.text = currentSpeech.AllSpeeches[speechIndex].Name;
            speechLabel.text = currentSpeech.AllSpeeches[speechIndex].Dialog;
            CurrentState = State.Visible;
            StageState.Instance.IsPaused = true;
        }
    }

    public override void Hide()
    {
        base.Hide();
        StageState.Instance.IsPaused = false;
    }

    public void OnNextClicked()
    {
        if (CurrentState == State.Visible)
        {
            ++speechIndex;
            if (speechIndex < currentSpeech.AllSpeeches.Length)
            {
                CurrentState = State.StandBy;
            }
            else
            {
                CurrentState = State.Hidden;
            }
        }
    }
}