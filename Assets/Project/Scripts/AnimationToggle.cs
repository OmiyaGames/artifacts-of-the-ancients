using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationToggle : IToggle
{
    [SerializeField]
    bool isOnAtStart = true;
    [SerializeField]
    bool oneWayToggle = false;

    [Header("Programmer Info")]
    [SerializeField]
    string isOnBoolField = "Is This On?";

    bool isOn = false, isToggledAtLeastOnce = false;

    public override bool IsOn
    {
        get
        {
            return isOn;
        }
        set
        {
            if((isOn != value) && ((oneWayToggle == false) || (isToggledAtLeastOnce == false)))
            {
                isOn = value;
                CachedAnimator.SetBool(isOnBoolField, isOn);
                isToggledAtLeastOnce = true;
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
        isOn = isOnAtStart;
        CachedAnimator.SetBool(isOnBoolField, isOn);

        isToggledAtLeastOnce = false;
    }
}
