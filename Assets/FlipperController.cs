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
    readonly Vector3 targetLocalPosition = Vector3.zero;

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
            //body.MovePosition(Vector2.Lerp(body.position, StageState.Instance.LastPortal.SpawnPointPosition, (Time.unscaledDeltaTime * lerpSpeed)));
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
            Debug.Log(StageState.Instance.LastPortal.SpawnPoint.name);
            transform.position = StageState.Instance.LastPortal.SpawnPoint.position;
        }
        cameraTransform.SetParent(null);
        body.isKinematic = false;
    }
}
