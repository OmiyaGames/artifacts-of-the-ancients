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
    bool isFlipped = false, isPaused = false;
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

    public Rigidbody2D Body
    {
        get
        {
            return body;
        }
    }

    public bool IsFlipped
    {
        get
        {
            return isFlipped;
        }
        private set
        {
            if(isFlipped != value)
            {
                isFlipped = value;
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
            int returnAction = (int)ITriggers.Action.Invalid, compareAction;
            if (Platformer.IsGrounded == true)
            {
                foreach (ITriggers trigger in allTriggers)
                {
                    compareAction = (int)trigger.ActionOnFire1;
                    if (compareAction > returnAction)
                    {
                        returnAction = compareAction;
                    }
                }
            }
            return (ITriggers.Action)returnAction;
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
                    Body.isKinematic = true;
                    Singleton.Get<TimeManager>().TimeScale = 0f;
                    cameraTransform.SetParent(transform);
                }
                else
                {
                    cameraTransform.SetParent(null);
                    Singleton.Get<TimeManager>().RevertToOriginalTime();
                    body.isKinematic = false;
                }
            }
        }
    }

    public void ToggleFlip()
    {
        IsFlipped = !IsFlipped;
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
}
