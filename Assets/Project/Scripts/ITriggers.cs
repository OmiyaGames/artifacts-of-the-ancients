using UnityEngine;

public abstract class ITriggers : MonoBehaviour
{
    public enum Action
    {
        Invalid = -1,
        Exit = 0,
        Flip,
        Examine,
        Grab
    }

    public abstract Action ActionOnFire1
    {
        get;
    }

    // FIXME: add this when indicating the action control to the player
    //public abstract string ActionText
    //{
    //    get;
    //}
}
