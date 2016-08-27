using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class DynamicFloor : MonoBehaviour
{
    LineRenderer lineGraphic = null;
    EdgeCollider2D lineCollider = null;

    Vector2[] originalPoints = null;

    public LineRenderer Graphic
    {
        get
        {
            if(lineGraphic == null)
            {
                lineGraphic = GetComponent<LineRenderer>();
            }
            return lineGraphic;
        }
    }

    public EdgeCollider2D Collider
    {
        get
        {
            if(lineCollider == null)
            {
                lineCollider = GetComponent<EdgeCollider2D>();
            }
            return lineCollider;
        }
    }

    void Start()
    {
        // Populate original points
        originalPoints = new Vector2[Collider.pointCount];
        for (int index = 0; index < Collider.pointCount; ++index)
        {
            originalPoints[index] = Collider.points[index];
        }

        // Update lineGraphics
        UpdateGraphics();
    }

    void Update()
    {
#if UNITY_EDITOR
        if(Application.isEditor && !Application.isPlaying)
        {
            UpdateGraphics();
        }
#else
#endif
    }

    void UpdateGraphics()
    {
        Graphic.SetVertexCount(Collider.pointCount);
        //Graphic.SetVertexCount(Collider.pointCount * 2 - 2);
        int graphicIndex = 0;
        for(int colliderIndex = 0; colliderIndex < Collider.pointCount; ++colliderIndex)
        {
            //if((colliderIndex == 0) || (colliderIndex == (Collider.pointCount - 1)))
            //{
                Graphic.SetPosition(graphicIndex, Collider.points[colliderIndex]);
                ++graphicIndex;
            //}
            //else
            //{
            //    Graphic.SetPosition(graphicIndex, Collider.points[colliderIndex]);
            //    ++graphicIndex;
            //    Graphic.SetPosition(graphicIndex, (Collider.points[colliderIndex] + (Random.insideUnitCircle * 0.01f)));
            //    ++graphicIndex;
            //}
        }
    }
}
