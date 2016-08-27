using UnityEngine;

[DisallowMultipleComponent]
public class Portal : ITriggers
{
    [Header("Activate")]
    [SerializeField]
    GameObject[] activateOnRightSideUp;
    [SerializeField]
    GameObject[] activateOnUpsideDown;

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

    public override Action ActionOnFire1
    {
        get
        {
            return Action.Flip;
        }
    }

    public Transform SpawnPoint
    {
        get
        {
            Transform returnTransform = spawnPointOnRightSideUp;
            if((StageState.Instance != null) && (StageState.Instance.IsFlipped == false))
            {
                returnTransform = spawnPointOnUpsideDown;
            }
            return returnTransform;
        }
    }

    public Vector2 SpawnPointPosition
    {
        get
        {
            return SpawnPoint.position;
        }
    }

    void Start()
    {
        StageState.Instance.onAfterFlipped += OnAfterFlipped;
        UpdateActivation(!StageState.Instance.IsFlipped);
    }

    void OnAfterFlipped(StageState state)
    {
        UpdateActivation(!state.IsFlipped);
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
