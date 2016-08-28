using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using OmiyaGames;

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
                (CrossPlatformInputManager.GetButtonDown("Fire1") == true))
            {
                switch(StageState.Instance.CurrentActionOnFire1)
                {
                    case ITriggers.Action.Examine:
                        SpeechTrigger speechTrigger = StageState.Instance.CurrentTriggerOnFire1 as SpeechTrigger;
                        if (speechTrigger != null)
                        {
                            speechTrigger.StartDialog();
                        }
                        break;
                    case ITriggers.Action.Exit:
                        ExitTrigger exitTrigger = StageState.Instance.CurrentTriggerOnFire1 as ExitTrigger;
                        if (exitTrigger != null)
                        {
                            Singleton.Get<SceneTransitionManager>().LoadNextLevel();
                        }
                        break;
                }
            }
        }
    }
}
