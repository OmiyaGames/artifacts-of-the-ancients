using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PortalTrigger : MonoBehaviour
{
    [SerializeField]
    Portal parent;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag(StageState.PlayerTag) == true)
        {
            StageState.Instance.AddTrigger(parent);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(StageState.PlayerTag) == true)
        {
            StageState.Instance.RemoveTrigger(parent);
        }
    }

    void OnEnabled(bool newFlag)
    {
        if(newFlag == false)
        {
            StageState.Instance.RemoveTrigger(parent);
        }
    }
}
