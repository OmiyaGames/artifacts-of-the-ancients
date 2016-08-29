using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using OmiyaGames;

[DisallowMultipleComponent]
public class BlockController : MonoBehaviour
{
    [SerializeField]
    SpringJoint2D boxJoint;

    Block lastblock = null;
    Vector2 blockVelocity = Vector2.zero;

    void Update()
    {
        if (StageState.Instance != null)
        {
            if ((CrossPlatformInputManager.GetButtonDown("Fire1") == true) &&
                (StageState.Instance.IsPopUpVisible == true) &&
                (StageState.Instance.Controller.IsCrouching == false) &&
                (StageState.Instance.CurrentActionOnFire1 == ITriggers.Action.Grab))
            {
                SetCrouching((StageState.Instance.CurrentTriggerOnFire1 as Block), Block.GrabCondition.StartGrab);
            }
            else if (StageState.Instance.Controller.IsCrouching == true)
            {
                blockVelocity.x = 0;
                blockVelocity.y = lastblock.Body.velocity.y;
                if (StageState.Instance.Platformer.IsGrounded == false)
                {
                    lastblock.Body.velocity = blockVelocity;
                    SetCrouching(null, Block.GrabCondition.AccidentLetGo);
                }
                else if (CrossPlatformInputManager.GetButtonUp("Fire1") == true)
                {
                    lastblock.Body.velocity = blockVelocity;
                    SetCrouching(null, Block.GrabCondition.ManualLetGo);
                }
                else if(lastblock != null)
                {
                    blockVelocity.x = StageState.Instance.Platformer.IntendedVelocity.x;
                    lastblock.Body.velocity = blockVelocity;
                }
            }
        }
    }

    void SetCrouching(Block blockTrigger, Block.GrabCondition reason)
    {
        // Check if we wan to crough
        bool flag = (blockTrigger != null);

        // Update flags
        //boxJoint.gameObject.SetActive(flag);
        StageState.Instance.Controller.IsCrouching = flag;

        // make changes specific to flags
        if (flag == true)
        {
            // If crouching, connect the rigidbody
            //boxJoint.connectedBody = blockTrigger.Body;

            // Setup block
            blockTrigger.SetupBlock(reason);
            lastblock = blockTrigger;
        }
        else
        {
            // If not crouching anymore, disconnect
            //boxJoint.connectedBody = null;

            // Setup block
            if (lastblock != null)
            {
                lastblock.SetupBlock(reason);
                lastblock = null;
            }
        }

        // Setup joint
        //boxJoint.autoConfigureDistance = !flag;
    }
}
