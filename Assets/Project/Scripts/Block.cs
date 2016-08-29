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

    Animator animator = null;
    Rigidbody2D body = null;

    //[SerializeField]
    //Collider2D changeMaterial;
    [SerializeField]
    float massOnGrab = 1f;
    [SerializeField]
    float massOnLetGo = 100f;
    //[SerializeField]
    //PhysicsMaterial2D materialOnGrab;
    //[SerializeField]
    //PhysicsMaterial2D materialOnLetGo;
    [SerializeField]
    Transform[] cornersClockwise = new Transform[4];

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

    public Animator Animator
    {
        get
        {
            return animator;
        }
    }

    void Start()
    {
        StageState.Instance.onAfterFlipped += UpdateBody;
        UpdateBody(StageState.Instance);

        Body.mass = massOnLetGo;
        //changeMaterial.sharedMaterial = materialOnLetGo;
    }

    void UpdateBody(StageState obj)
    {
        if((obj.IsRightSideUp == true) && (transform.localScale.y > 0))
        {
            Body.isKinematic = false;
        }
        else if ((obj.IsRightSideUp == false) && (transform.localScale.y < 0))
        {
            Body.isKinematic = false;
        }
        else
        {
            Body.isKinematic = true;
        }
        // FIXME: play some sort of animation
        // Animator.SetBool(IsEnabled, Body.isKinematic);
    }

    public void SetupBlock(GrabCondition isGrabbing)
    {
        switch(isGrabbing)
        {
            case GrabCondition.StartGrab:
                StageState.Instance.RemoveTrigger(this);
                Body.mass = massOnGrab;
                //changeMaterial.sharedMaterial = materialOnGrab;
                break;
            case GrabCondition.ManualLetGo:
                //StageState.Instance.AddTrigger(this);
                //goto case GrabCondition.AccidentLetGo;
            case GrabCondition.AccidentLetGo:
                Body.mass = massOnLetGo;
                //changeMaterial.sharedMaterial = materialOnLetGo;
                break;
        }
    }
}
