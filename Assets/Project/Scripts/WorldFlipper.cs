using UnityEngine;
using System.Collections;

public class WorldFlipper : MonoBehaviour
{
    [SerializeField]
    [Range(-1f, 1f)]
    float flipScaleFactor = 1f;

    float currentFlipFactor = 1f;
    Vector3 originalScale = Vector3.one;

    void Start()
    {
        currentFlipFactor = flipScaleFactor;
        originalScale = transform.localScale;
    }

    void SetFlipFactor(float changeTo, Transform scaleAround)
    {
        if (Mathf.Approximately(changeTo, currentFlipFactor) == false)
        {
            currentFlipFactor = changeTo;

            // diff from object pivot to desired pivot/origin
            Vector3 diffPositions = transform.position - scaleAround.position;

            // calc final position post-scale
            Vector3 finalPosition = (diffPositions * currentFlipFactor) + scaleAround.position;

            // finally, actually perform the scale/translation
            transform.localScale = originalScale * currentFlipFactor;
            transform.position = finalPosition;
        }
    }
}
