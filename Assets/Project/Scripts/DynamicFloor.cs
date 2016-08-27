using UnityEngine;
using Line2D;

[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(Line2DRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class DynamicFloor : MonoBehaviour
{
    Line2DRenderer lineGraphic = null;
    EdgeCollider2D lineCollider = null;

    Vector2[] originalPoints = null;

    public Line2DRenderer Graphic
    {
        get
        {
            if(lineGraphic == null)
            {
                lineGraphic = GetComponent<Line2DRenderer>();
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
        for(int index = 0; index < Collider.pointCount; ++index)
        {
            if(index < Graphic.points.Count)
            {
                Graphic.points[index].pos = Collider.points[index];
            }
            else
            {
                Graphic.points.Add(new Line2DPoint(Collider.points[index], 1f, Color.white));
            }
        }
        while(Graphic.points.Count > Collider.pointCount)
        {
            Graphic.points.RemoveAt(Graphic.points.Count - 1);
        }
    }
}
