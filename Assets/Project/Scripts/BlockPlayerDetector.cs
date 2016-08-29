using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BlockPlayerDetector : MonoBehaviour
{
    [SerializeField]
    Block parent;

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag(StageState.PlayerTag) == true) &&
            (StageState.Instance != null) &&
            (parent.Body.isKinematic == false))
        {
            StageState.Instance.AddTrigger(parent);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((other.CompareTag(StageState.PlayerTag) == true) &&
            (StageState.Instance != null))
        {
            StageState.Instance.RemoveTrigger(parent);
        }
    }
}
