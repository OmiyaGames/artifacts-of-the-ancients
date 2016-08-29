using UnityEngine;
using System.Collections;
using System;

public class Block : ITriggers
{
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

    public void ClosePopUp()
    {
        StageState.Instance.RemoveTrigger(this);
    }
}
