using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationToggle : IToggle
{
    [SerializeField]
    bool isOnAtStart = true;
    [SerializeField]
    string isOnBoolField = "Is This On?";

    bool isOn = false;

    public override bool IsOn
    {
        get
        {
            return isOn;
        }
        set
        {
            if(isOn != value)
            {
                isOn = value;
                CachedAnimator.SetBool(isOnBoolField, isOn);
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
        isOn = isOnAtStart;
        CachedAnimator.SetBool(isOnBoolField, isOn);
    }
}
