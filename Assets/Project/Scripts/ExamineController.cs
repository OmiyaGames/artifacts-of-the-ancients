using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class ExamineController : MonoBehaviour
{
    bool flipStarted = false;

    void Update()
    {
        if ((StageState.Instance != null) && (WorldFlipper.Instance != null))
        {
            if ((flipStarted == false) &&
                (StageState.Instance.Platformer.IsGrounded == true) &&
                (StageState.Instance.IsPaused == false) &&
                (StageState.Instance.CurrentActionOnFire1 == ITriggers.Action.Examine) &&
                (CrossPlatformInputManager.GetButtonDown("Fire1") == true))
            {
                SpeechTrigger trigger = StageState.Instance.CurrentTriggerOnFire1 as SpeechTrigger;
                if(trigger != null)
                {
                    trigger.StartDialog();
                }
            }
        }
    }
}
