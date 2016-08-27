using UnityEngine;
using UnityStandardAssets._2D;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlatformerCharacter2D))]
public class StageState : MonoBehaviour
{
    public const string PlayerTag = "Player";
    public event System.Action<StageState> onAfterFlipped;

    static StageState instance = null;
    PlatformerCharacter2D platformer = null;
    bool isFlipped = false;
    Portal lastPortal = null;
    readonly HashSet<ITriggers> allTriggers = new HashSet<ITriggers>();

    void Awake()
    {
        instance = this;
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

    public void ToggleFlip()
    {
        IsFlipped = !IsFlipped;
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
