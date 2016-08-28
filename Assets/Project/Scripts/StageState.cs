using UnityEngine;
using UnityStandardAssets._2D;
using System.Collections.Generic;
using OmiyaGames;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlatformerCharacter2D))]
public class StageState : MonoBehaviour
{
    public const string PlayerTag = "Player";
    public event System.Action<StageState> onAfterFlipped;

    static StageState instance = null;

    [SerializeField]
    Transform cameraTransform = null;

    PlatformerCharacter2D platformer = null;
    Platformer2DUserControl controller = null;
    bool isRightSideUp = true, isPaused = false;
    Portal lastPortal = null;
    Rigidbody2D body;
    Vector3 startPosition = Vector3.zero;

    readonly HashSet<ITriggers> allTriggers = new HashSet<ITriggers>();

    void Awake()
    {
        instance = this;
        body = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        cameraTransform.SetParent(null);
    }

    void OnDestroy()
    {
        instance = null;
    }

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
            ITriggers.Action returnAction = ITriggers.Action.Invalid;
            ITriggers returnTrigger = null;
            SearchCurrentTrigger(out returnTrigger, out returnAction);
            return returnAction;
        }
    }

    public ITriggers CurrentTriggerOnFire1
    {
        get
        {
            ITriggers.Action returnAction = ITriggers.Action.Invalid;
            ITriggers returnTrigger = null;
            SearchCurrentTrigger(out returnTrigger, out returnAction);
            return returnTrigger;
        }
    }

    public Portal LastPortal
    {
        get
        {
            return lastPortal;
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

    public bool AddTrigger(ITriggers trigger)
    {
        if (trigger is Portal)
        {
            lastPortal = (Portal)trigger;
        }
        return allTriggers.Add(trigger);
    }

    public bool RemoveTrigger(ITriggers trigger)
    {
        return allTriggers.Remove(trigger);
    }

    void SearchCurrentTrigger(out ITriggers returnTrigger, out ITriggers.Action returnAction)
    {
        // Setup default return values
        returnAction = ITriggers.Action.Invalid;
        returnTrigger = null;

        // Search for the current trigger
        int returnActionInt = (int)returnAction, compareActionInt;
        if (Platformer.IsGrounded == true)
        {
            foreach (ITriggers trigger in allTriggers)
            {
                compareActionInt = (int)trigger.ActionOnFire1;
                if (compareActionInt > returnActionInt)
                {
                    returnTrigger = trigger;
                    returnActionInt = compareActionInt;
                }
            }
        }
        returnAction = (ITriggers.Action)returnActionInt;
    }
}
