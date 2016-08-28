using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ExitTrigger : ITriggers
{
    public override Action ActionOnFire1
    {
        get
        {
            return Action.Exit;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag(StageState.PlayerTag) == true) &&
            (StageState.Instance != null))
        {
            StageState.Instance.AddTrigger(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((other.CompareTag(StageState.PlayerTag) == true) &&
            (StageState.Instance != null))
        {
            StageState.Instance.RemoveTrigger(this);
        }
    }
}
