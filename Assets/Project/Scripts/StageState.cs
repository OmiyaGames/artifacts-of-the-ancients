using UnityEngine;
using UnityStandardAssets._2D;
using System.Collections.Generic;
using OmiyaGames;
using UnityStandardAssets.ImageEffects;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlatformerCharacter2D))]
public class StageState : MonoBehaviour
{
    public const string PlayerTag = "Player";
    public const string PopUpVisibleBoolField = "Is Pop-Up Visible?";
    public const string PopUpChangeTriggerField = "Change Pop-Up";
    public event System.Action<StageState> onAfterFlipped;

    static StageState instance = null;

    [SerializeField]
    Transform cameraTransform = null;
    [SerializeField]
    UnityEngine.UI.Text popUpLabel = null;

    [Header("Sound")]
    [SerializeField]
    SoundEffect footstep = null;
    [SerializeField]
    SoundEffect land = null;
    [SerializeField]
    SoundEffect jump = null;
    [SerializeField]
    SoundEffect drag = null;
    [SerializeField]
    SoundEffect death = null;

    [Header("Image Effects")]
    [SerializeField]
    SunShafts disableEffectOnWebGl = null;

    PlatformerCharacter2D platformer = null;
    Platformer2DUserControl controller = null;
    bool isRightSideUp = true, isPaused = false,
        wasPopUpVisibleLastFrame = false;
    Portal lastPortal = null;
    Rigidbody2D body;
    Animator characterAnimation = null;
    Vector3 startPosition = Vector3.zero;
    ITriggers latestTrigger = null;
    string lastPopUpText = null;

    readonly HashSet<ITriggers> allTriggers = new HashSet<ITriggers>();

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        characterAnimation = GetComponent<Animator>();
        startPosition = transform.position;
        cameraTransform.SetParent(null);

        if(Singleton.Instance.IsWebplayer == true)
        {
            disableEffectOnWebGl.enabled = false;
        }

        Platformer.onHoldCrate += OnHoldCrate;
        Platformer.onJump += OnJump;
        Platformer.onLand += OnLand;
        Platformer.onWalk += OnWalk;
        
        // Set instance last
        instance = this;
    }

    private void OnWalk(PlatformerCharacter2D obj)
    {
        if(footstep != null)
        {
            footstep.Play();
        }
    }

    private void OnLand(PlatformerCharacter2D obj)
    {
        if (land != null)
        {
            land.Play();
        }
    }

    private void OnJump(PlatformerCharacter2D obj)
    {
        if (jump != null)
        {
            jump.Play();
        }
    }

    private void OnHoldCrate(PlatformerCharacter2D obj, bool isGrabbing)
    {
        if (drag != null)
        {
            bool playSound = (isGrabbing == true) && (Mathf.Abs(obj.IntendedVelocity.x) > 0.01f);
            if ((playSound == true) && (drag.CurrentState == IAudio.State.Stopped))
            {
                drag.Play();
            }
            else if ((playSound == false) && (drag.CurrentState == IAudio.State.Playing))
            {
                drag.CurrentState = IAudio.State.Stopped;
            }
        }
    }

    void OnDestroy()
    {
        instance = null;
    }

    void Update()
    {
        if(IsPopUpVisible == true)
        {
            if(wasPopUpVisibleLastFrame == false)
            {
                // Show the pop up
                UpdatePopUpText();
                characterAnimation.SetBool(PopUpVisibleBoolField, true);
                characterAnimation.ResetTrigger(PopUpChangeTriggerField);

                // Update this frame's information
                lastPopUpText = CurrentTriggerOnFire1.ActionText;
                wasPopUpVisibleLastFrame = true;
            }
            else if(string.Equals(lastPopUpText, CurrentTriggerOnFire1.ActionText) == false)
            {
                // Play the pop-up not visible anmation
                characterAnimation.SetTrigger(PopUpChangeTriggerField);

                // Update this frame's information
                lastPopUpText = CurrentTriggerOnFire1.ActionText;
            }
        }
        else if(wasPopUpVisibleLastFrame == true)
        {
            // Hide the pop-up
            characterAnimation.SetBool(PopUpVisibleBoolField, false);

            // Update this frame's information
            lastPopUpText = null;
            wasPopUpVisibleLastFrame = false;
        }
    }

    #region Properties
    public static StageState Instance
    {
        get
        {
            return instance;
        }
    }

    public PlatformerCharacter2D Platformer
    {
        get
        {
            if(platformer == null)
            {
                platformer = GetComponent<PlatformerCharacter2D>();
            }
            return platformer;
        }
    }

    public Platformer2DUserControl Controller
    {
        get
        {
            if(controller == null)
            {
                controller = GetComponent<Platformer2DUserControl>();
            }
            return controller;
        }
    }

    public Rigidbody2D Body
    {
        get
        {
            return body;
        }
    }

    public bool IsRightSideUp
    {
        get
        {
            return isRightSideUp;
        }
        private set
        {
            if(isRightSideUp != value)
            {
                isRightSideUp = value;
                if(onAfterFlipped != null)
                {
                    onAfterFlipped(this);
                }
            }
        }
    }

    public ITriggers.Action CurrentActionOnFire1
    {
        get
        {
            if(CurrentTriggerOnFire1 != null)
            {
                return CurrentTriggerOnFire1.ActionOnFire1;
            }
            else
            {
                return ITriggers.Action.Invalid;
            }
        }
    }

    public ITriggers CurrentTriggerOnFire1
    {
        get
        {
            return latestTrigger;
        }
    }

    public Portal LastPortal
    {
        get
        {
            return lastPortal;
        }
    }

    public bool IsPopUpVisible
    {
        get
        {
            bool returnFlag = false;
            if((CurrentTriggerOnFire1 != null) && (string.IsNullOrEmpty(CurrentTriggerOnFire1.ActionText) == false) && (Platformer.IsGrounded == true) && (IsPaused == false) && (Controller.IsCrouching == false))
            {
                returnFlag = true;
            }
            return returnFlag;
        }
    }

    public bool IsPaused
    {
        get
        {
            return isPaused;
        }
        set
        {
            if(isPaused != value)
            {
                isPaused = value;
                if (isPaused == true)
                {
                    Controller.enabled = false;
                    Body.isKinematic = true;
                    Singleton.Get<TimeManager>().TimeScale = 0f;
                    cameraTransform.SetParent(transform);
                }
                else
                {
                    Controller.enabled = true;
                    cameraTransform.SetParent(null);
                    Singleton.Get<TimeManager>().RevertToOriginalTime();
                    body.isKinematic = false;
                }
            }
        }
    }
    #endregion

    public void ToggleFlip()
    {
        IsRightSideUp = !IsRightSideUp;
    }

    public void Respawn()
    {
        if((LastPortal != null) && (LastPortal.SpawnPoint != null))
        {
            Debug.Log("Spawn at: " + LastPortal.SpawnPoint.name);
            transform.position = LastPortal.SpawnPoint.position;
        }
        else
        {
            transform.position = startPosition;
        }
    }

    public void PlayDeathSoundEffect()
    {
        if (death != null)
        {
            death.Play();
        }
    }

    public bool AddTrigger(ITriggers trigger)
    {
        bool returnFlag = false;
        if (trigger != null)
        {
            returnFlag = allTriggers.Add(trigger);
            if (trigger is Portal)
            {
                lastPortal = (Portal)trigger;
            }
            if (returnFlag == true)
            {
                SearchCurrentTrigger(out latestTrigger);
            }
        }
        return returnFlag;
    }

    public bool RemoveTrigger(ITriggers trigger)
    {
        bool returnFlag = false;
        if (trigger != null)
        {
            returnFlag = allTriggers.Remove(trigger);
            if (returnFlag == true)
            {
                SearchCurrentTrigger(out latestTrigger);
            }
        }
        return returnFlag;
    }

    public void UpdatePopUpText()
    {
        if(CurrentTriggerOnFire1 != null)
        {
            popUpLabel.text = CurrentTriggerOnFire1.ActionText;
        }
    }

    void SearchCurrentTrigger(out ITriggers returnTrigger)
    {
        // Setup default return values
        returnTrigger = null;
        if (allTriggers.Count > 0)
        {
            // Search for the current trigger
            int compareActionInt, highestActionInt = (int)ITriggers.Action.Invalid;
            foreach (ITriggers trigger in allTriggers)
            {
                // Check to see if this trigger has a higher action id
                compareActionInt = (int)trigger.ActionOnFire1;
                if (compareActionInt > highestActionInt)
                {
                    // If it does, set the return value
                    returnTrigger = trigger;
                    highestActionInt = compareActionInt;
                }
            }
        }
    }
}
