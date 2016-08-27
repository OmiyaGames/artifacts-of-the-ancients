using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class FlipperController : MonoBehaviour
{
    [SerializeField]
    float lerpSpeed = 20f;
    [SerializeField]
    Transform cameraTransform = null;

    Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        WorldFlipper.Instance.onStartFlipAnimation += StartAnimation;
        WorldFlipper.Instance.onEndFlipAnimation += EndAnimation;
        cameraTransform.SetParent(null);
    }

    void Update()
    {
        if ((body.isKinematic == false) && (StageState.Instance != null) && (WorldFlipper.Instance != null) &&
            (StageState.Instance.CurrentActionOnFire1 == ITriggers.Action.Flip) &&
            (CrossPlatformInputManager.GetButtonDown("Fire1")== true))
        {
            StageState.Instance.ToggleFlip();
            WorldFlipper.Instance.ExecuteFlip(StageState.Instance.IsFlipped);
        }
        else if (body.isKinematic == true)
        {
            transform.position = Vector3.Lerp(transform.position, StageState.Instance.LastPortal.FinalSpawnPointPosition, (Time.unscaledDeltaTime * lerpSpeed));
        }
    }

    void StartAnimation(WorldFlipper flipper)
    {
        body.isKinematic = true;
        cameraTransform.SetParent(transform);
    }

    void EndAnimation(WorldFlipper flipper)
    {
        if ((StageState.Instance != null) && (StageState.Instance.LastPortal != null))
        {
            transform.position = StageState.Instance.LastPortal.SpawnPoint.position;
        }
        cameraTransform.SetParent(null);
        body.isKinematic = false;
    }
}
