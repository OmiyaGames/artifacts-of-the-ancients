using UnityEngine;

public abstract class ITriggers : MonoBehaviour
{
    public enum Action
    {
        Invalid = -1,
        Flip = 0,
        Grab
    }

    public abstract Action ActionOnFire1
    {
        get;
    }
}
