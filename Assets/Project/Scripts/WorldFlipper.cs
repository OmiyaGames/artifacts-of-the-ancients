using UnityEngine;
using System.Collections;

public class WorldFlipper : MonoBehaviour
{
    [SerializeField]
    [Range(-1f, 1f)]
    float flipScaleFactor = 1f;

    float currentFlipFactor = 1f;
    Vector3 originalScale = Vector3.one;
    Vector3 currentScale = Vector3.one;

    void Start()
    {
        currentFlipFactor = flipScaleFactor;
        originalScale = transform.localScale;
        currentScale = originalScale;
    }

    void SetFlipFactor(float changeTo, Transform scaleAround)
    {
        if (Mathf.Approximately(changeTo, currentFlipFactor) == false)
        {
            currentFlipFactor = changeTo;

            // diff from object pivot to desired pivot/origin
            Vector3 diffPositions = transform.position - scaleAround.position;

            // calc final position post-scale
            //Vector3 finalPosition = (diffPositions * currentFlipFactor) + scaleAround.position;

            // finally, actually perform the scale/translation
            currentScale = originalScale;
            currentScale.y = originalScale.y * currentFlipFactor;
            transform.localScale = currentScale;
            //transform.position = finalPosition;
        }
    }

    void Update()
    {
        if(StageState.Instance.LastPortal != null)
        {
            SetFlipFactor(flipScaleFactor, StageState.Instance.LastPortal.transform);
        }
    }
}
