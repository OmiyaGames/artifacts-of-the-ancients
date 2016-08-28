using UnityEngine;

public abstract class IToggle : MonoBehaviour
{
    Animator animatorCache = null;
    ITriggers lastTrigger = null;

    public abstract bool IsOn
    {
        get;
        set;
    }

    public Animator CachedAnimator
    {
        get
        {
            if(animatorCache == null)
            {
                animatorCache = GetComponent<Animator>();
            }
            return animatorCache;
        }
    }

    public void Toggle(ITriggers trigger)
    {
        lastTrigger = trigger;
        IsOn = !IsOn;
    }
}
