using UnityEngine;
using OmiyaGames;

[RequireComponent(typeof(Animator))]
public class AnimationDisabled : MonoBehaviour
{
    [Header("Programmer Info")]
    [SerializeField]
    string isDisabledBoolField = "Is Disabled?";

    Animator animator;
    bool isRightSideUpOnStart = true;

    // Use this for initialization
    void Start()
    {
        isRightSideUpOnStart = (transform.localScale.y > 0);
        animator = GetComponent<Animator>();
        StageState.Instance.onAfterFlipped += UpdateOnFlip;
        UpdateOnFlip(StageState.Instance);
    }

    private void UpdateOnFlip(StageState obj)
    {
        animator.SetBool(isDisabledBoolField, (isRightSideUpOnStart != obj.IsRightSideUp));
    }
}
