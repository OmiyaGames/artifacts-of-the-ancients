using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Block : ITriggers
{
    public enum GrabCondition
    {
        StartGrab,
        ManualLetGo,
        AccidentLetGo
    }

    public enum BlockCondition
    {
        Active = 0,
        InActive,
        Hidden
    }

    Animator animator = null;
    Rigidbody2D body = null;

    [SerializeField]
    Collider2D changeMaterial;
    [SerializeField]
    float massOnGrab = 1f;
    [SerializeField]
    float massOnLetGo = 100f;
    [SerializeField]
    PhysicsMaterial2D materialOnGrab;
    [SerializeField]
    PhysicsMaterial2D materialOnLetGo;
    [SerializeField]
    Transform[] cornersClockwise = new Transform[4];
    [SerializeField]
    Collider2D[] activeColliders;
    [SerializeField]
    Collider2D[] inactiveColliders;

    BlockCondition currentCondition = BlockCondition.Active;

    public override Action ActionOnFire1
    {
        get
        {
            return Action.Grab;
        }
    }

    public override string ActionText
    {
        get
        {
            return "Grab";
        }
    }

    public Rigidbody2D Body
    {
        get
        {
            if(body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }
            return body;
        }
    }

    public Transform[] CornersClockwise
    {
        get
        {
            return cornersClockwise;
        }
    }

    public Animator Animator
    {
        get
        {
            return animator;
        }
    }

    public BlockCondition CurrentCondition
    {
        get
        {
            return currentCondition;
        }
        private set
        {
            if (currentCondition != value)
            {
                currentCondition = value;

                int index = 0;
                for (; index < activeColliders.Length; ++index)
                {
                    activeColliders[index].gameObject.SetActive(currentCondition == BlockCondition.Active);
                }
                for (index = 0; index < inactiveColliders.Length; ++index)
                {
                    inactiveColliders[index].gameObject.SetActive(currentCondition == BlockCondition.InActive);
                }
                Body.isKinematic = (currentCondition != BlockCondition.Active);

                // FIXME: play some sort of animation
                // Animator.SetBool(IsEnabled, Body.isKinematic);
            }
        }
    }

    public bool IsVisible
    {
        get
        {
            return (CurrentCondition != BlockCondition.Hidden);
        }
        set
        {
            if((value == true) && (CurrentCondition == BlockCondition.Hidden))
            {
                MakeBodyVisible(StageState.Instance.IsRightSideUp);
            }
            else if ((value == false) && (CurrentCondition != BlockCondition.Hidden))
            {
                CurrentCondition = BlockCondition.Hidden;
            }
        }
    }

    void Start()
    {
        StageState.Instance.onAfterFlipped += UpdateBody;
        MakeBodyVisible(StageState.Instance.IsRightSideUp);

        Body.mass = massOnLetGo;
        changeMaterial.sharedMaterial = materialOnLetGo;
    }

    void UpdateBody(StageState obj)
    {
        if (currentCondition != BlockCondition.Hidden)
        {
            MakeBodyVisible(obj.IsRightSideUp);
        }
    }

    void MakeBodyVisible(bool rightSideUp)
    {
        if ((rightSideUp == true) && (transform.localScale.y > 0))
        {
            CurrentCondition = BlockCondition.Active;
        }
        else if ((rightSideUp == false) && (transform.localScale.y < 0))
        {
            CurrentCondition = BlockCondition.Active;
        }
        else
        {
            CurrentCondition = BlockCondition.InActive;
        }
    }

    public void SetupBlock(GrabCondition isGrabbing)
    {
        switch(isGrabbing)
        {
            case GrabCondition.StartGrab:
                StageState.Instance.RemoveTrigger(this);
                Body.mass = massOnGrab;
                changeMaterial.sharedMaterial = materialOnGrab;
                break;
            case GrabCondition.ManualLetGo:
            case GrabCondition.AccidentLetGo:
                Body.mass = massOnLetGo;
                changeMaterial.sharedMaterial = materialOnLetGo;
                break;
        }
    }
}
