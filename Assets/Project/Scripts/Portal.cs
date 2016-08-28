using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class Portal : ITriggers
{
    // FIXME: remove these member variables
    [Header("Activate")]
    [SerializeField]
    GameObject[] activateOnRightSideUp;
    [SerializeField]
    GameObject[] activateOnUpsideDown;

    // FIXME: remove these member variables
    [Header("Deactivate")]
    [SerializeField]
    GameObject[] deactivateOnRightSideUp;
    [SerializeField]
    GameObject[] deactivateOnUpsideDown;

    [Header("Spawn Point")]
    [SerializeField]
    Transform spawnPointOnRightSideUp;
    [SerializeField]
    Transform spawnPointOnUpsideDown;

    Animator animatorCache = null;
    Vector3 finalSpawnPositionOnRightSideUp, finalSpawnPositionOnUpsideDown;

    public override Action ActionOnFire1
    {
        get
        {
            return Action.Flip;
        }
    }

    public Animator CachedAnimator
    {
        get
        {
            if(animatorCache == null)
            {
                animatorCache = GetComponent<Animator>();
            }
            return animatorCache;
        }
    }

    public Transform SpawnPoint
    {
        get
        {
            Transform returnTransform = spawnPointOnRightSideUp;
            if((StageState.Instance != null) && (StageState.Instance.IsRightSideUp == false))
            {
                returnTransform = spawnPointOnUpsideDown;
            }
            return returnTransform;
        }
    }

    public Vector3 FinalSpawnPointPosition
    {
        get
        {
            if ((StageState.Instance != null) && (StageState.Instance.IsRightSideUp == false))
            {
                return finalSpawnPositionOnUpsideDown;
            }
            else
            {
                return finalSpawnPositionOnRightSideUp;
            }
        }
    }

    void Start()
    {
        StageState.Instance.onAfterFlipped += OnAfterFlipped;
        UpdateActivation(!StageState.Instance.IsRightSideUp);

        finalSpawnPositionOnRightSideUp = spawnPointOnRightSideUp.position;
        finalSpawnPositionOnUpsideDown = transform.position;
        Vector3 diff = transform.position - spawnPointOnUpsideDown.position;
        finalSpawnPositionOnUpsideDown.y *= -1f;
        finalSpawnPositionOnUpsideDown.y += diff.y;
    }

    void OnAfterFlipped(StageState state)
    {
        UpdateActivation(!state.IsRightSideUp);
    }

    void UpdateActivation(bool rightSideUp)
    {
        GameObject[] activate = activateOnRightSideUp;
        GameObject[] deactivate = deactivateOnRightSideUp;
        if(rightSideUp == false)
        {
            activate = activateOnUpsideDown;
            deactivate = deactivateOnUpsideDown;
        }

        int index = 0;
        for (; index < activate.Length; ++index)
        {
            activate[index].SetActive(true);
        }
        for(index = 0; index < deactivate.Length; ++index)
        {
            deactivate[index].SetActive(true);
        }
    }
}
