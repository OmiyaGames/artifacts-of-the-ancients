using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class FlipperController : MonoBehaviour
{
    [SerializeField]
    float lerpSpeed = 20f;

    bool flipStarted = false;

    void Start()
    {
        WorldFlipper.Instance.onStartFlipAnimation += StartAnimation;
        WorldFlipper.Instance.onEndFlipAnimation += EndAnimation;
    }

    void Update()
    {
        if ((StageState.Instance != null) && (WorldFlipper.Instance != null))
        {
            if ((flipStarted == false) &&
                (StageState.Instance.Platformer.IsGrounded == true) &&
                (StageState.Instance.IsPaused == false) &&
                (StageState.Instance.CurrentActionOnFire1 == ITriggers.Action.Flip) &&
                (CrossPlatformInputManager.GetButtonDown("Fire1") == true))
            {
                StageState.Instance.ToggleFlip();
                WorldFlipper.Instance.ExecuteFlip(StageState.Instance.IsRightSideUp);
                flipStarted = true;
            }
            else if (flipStarted == true)
            {
                transform.position = Vector3.Lerp(transform.position, StageState.Instance.LastPortal.FinalSpawnPointPosition, (Time.unscaledDeltaTime * lerpSpeed));
            }
        }
    }

    void StartAnimation(WorldFlipper flipper)
    {
        StageState.Instance.IsPaused = true;
    }

    void EndAnimation(WorldFlipper flipper)
    {
        if ((StageState.Instance != null) && (StageState.Instance.LastPortal != null))
        {
            StageState.Instance.Respawn();
        }
        flipStarted = false;
        StageState.Instance.IsPaused = false;
    }
}
