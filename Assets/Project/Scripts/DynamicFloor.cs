using UnityEngine;
using Line2D;
using System.Collections.Generic;

[DisallowMultipleComponent]
[ExecuteInEditMode]
[RequireComponent(typeof(Line2DRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class DynamicFloor : MonoBehaviour
{
    const float DistanceCloseEnough = 0.1f;

    Line2DRenderer lineGraphic = null;
    EdgeCollider2D lineCollider = null;

    Vector2[] originalPoints = null;

    readonly List<Block> rightSideUpBlocks = new List<Block>();
    readonly List<Block> upsideDownBlocks = new List<Block>();
    readonly List<Vector2> latestColliderPoints = new List<Vector2>();

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

    public void AddBlock(Block block)
    {
        if(block.transform.localScale.y > 0)
        {
            rightSideUpBlocks.Add(block);
        }
        else
        {
            upsideDownBlocks.Add(block);
        }
    }

    void Start()
    {
        if (Application.isPlaying == true)
        {
            // Populate original points
            originalPoints = new Vector2[Collider.pointCount];
            for (int index = 0; index < Collider.pointCount; ++index)
            {
                originalPoints[index] = Collider.points[index];
            }

            // Sort block list
            if (rightSideUpBlocks.Count > 1)
            {
                rightSideUpBlocks.Sort(BlockSorter);
            }
            if (upsideDownBlocks.Count > 1)
            {
                upsideDownBlocks.Sort(BlockSorter);
            }

            // Update this line
            StageState.Instance.onAfterFlipped += UpdateAll;
            UpdateCollider(StageState.Instance.IsRightSideUp, true);
            UpdateGraphics();
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if(Application.isPlaying == false)
        {
            UpdateGraphics();
        }
    }
#endif

    void UpdateAll(StageState obj)
    {
        UpdateCollider(obj.IsRightSideUp, false);
        UpdateGraphics();
    }

    void UpdateCollider(bool isRightSideUp, bool onStart)
    {
        // Clear cache list
        latestColliderPoints.Clear();

        // Setup index
        int pIndex = 0;
        int bIndex = 0;
        bool addPoint = true;

        // Grab the first block (if there are any)
        Block currentBlock = null;
        NextBlock(bIndex, isRightSideUp, out currentBlock);

        // Go through all the original points
        Vector2 currentPoint, cornerPoint;
        for (; pIndex < originalPoints.Length; ++pIndex)
        {
            // Setup corner
            addPoint = true;
            currentPoint = originalPoints[pIndex];

            // Make sure we have a block
            if (currentBlock != null)
            {
                // Grab the lower left corner from the block
                GetBlockCorner(currentBlock, 0, isRightSideUp, onStart, out cornerPoint);

                // Check if the current point is a lot like a block's lower-left point
                if (IsCloseEnough(ref currentPoint, cornerPoint) == true)
                {
                    // Skip currentPoint
                    addPoint = false;

                    // Skip the current point, in favor of the 2nd corner of the block
                    AppendBlockCornersToList(isRightSideUp, onStart, ref bIndex, 1, ref currentBlock, out cornerPoint);

                    // Add the last corner point
                    latestColliderPoints.Add(cornerPoint);
                }
                else if (cornerPoint.x < currentPoint.x)
                {
                    // If instead, there's a block between the last 2 points
                    AppendBlockCornersToList(isRightSideUp, onStart, ref bIndex, 0, ref currentBlock, out cornerPoint);

                    // Check if we should skip the currentPoint in favor of the last corner (since they're close enough)
                    if (IsCloseEnough(ref currentPoint, cornerPoint) == true)
                    {
                        // Skip currrentPoint
                        addPoint = false;
                    }
                    else
                    {
                        // Add the last corner point
                        latestColliderPoints.Add(cornerPoint);
                    }
                }
            }

            // Add point
            if (addPoint == true)
            {
                latestColliderPoints.Add(currentPoint);
            }
        }

        // Convert list into an array
        Collider.points = latestColliderPoints.ToArray();
    }

    private void AppendBlockCornersToList(bool isRightSideUp, bool onStart, ref int bIndex, int firstCorner, ref Block currentBlock, out Vector2 cornerPoint)
    {
        do
        {
            // Add all the block points
            for (int cIndex = firstCorner; cIndex < (currentBlock.CornersClockwise.Length - 1); ++cIndex)
            {
                GetBlockCorner(currentBlock, cIndex, isRightSideUp, onStart, out cornerPoint);
                latestColliderPoints.Add(cornerPoint);
            }

            // Set the currentPoint to the last corner
            GetBlockCorner(currentBlock, (currentBlock.CornersClockwise.Length - 1), isRightSideUp, onStart, out cornerPoint);

            // Grab the next block (if there are any)
            ++bIndex;
            NextBlock(bIndex, isRightSideUp, out currentBlock);

            // FIXME: check if we should move onto the next block's lower left corner
            // I'm looping only once for now
            break;
        }
        while (currentBlock != null);
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

        if (Application.isPlaying == true)
        {
            Graphic.UpdateLineToNewPoints();
        }
    }

    void NextBlock(int bIndex, bool isRightSideUp, out Block currentBlock)
    {
        currentBlock = null;
        if ((isRightSideUp == false) && (bIndex < rightSideUpBlocks.Count))
        {
            currentBlock = rightSideUpBlocks[bIndex];
        }
        else if ((isRightSideUp == true) && (bIndex < upsideDownBlocks.Count))
        {
            currentBlock = upsideDownBlocks[bIndex];
        }
    }

    void GetBlockCorner(Block currentBlock, int cIndex, bool isRightSideUp, bool onStart, out Vector2 cornerPoint)
    {
        cornerPoint = currentBlock.CornersClockwise[cIndex].position;
        if(isRightSideUp == true)
        {
            if (onStart == false)
            {
                cornerPoint.y *= -1f;
                cornerPoint.y += transform.position.y;
            }
            else
            {
                cornerPoint.y -= transform.position.y;
            }
        }
        else
        {
            cornerPoint.y -= transform.position.y;
        }
        cornerPoint.x -= transform.position.x;
    }

    int BlockSorter(Block left, Block right)
    {
        return (int)Mathf.Sign(left.transform.position.x - right.transform.position.y);
    }

    bool IsCloseEnough(ref Vector2 originalPoint, Vector3 transformPoint)
    {
        float diffX = originalPoint.x - transformPoint.x;
        float diffY = originalPoint.y - transformPoint.y;

        return (((diffX * diffX) + (diffY * diffY)) < (DistanceCloseEnough * DistanceCloseEnough));
    }

    bool IsCloseEnough(Vector3 transformPoint1, Vector3 transformPoint2)
    {
        float diffX = transformPoint1.x - transformPoint2.x;
        float diffY = transformPoint1.y - transformPoint2.y;

        return (((diffX * diffX) + (diffY * diffY)) < (DistanceCloseEnough * DistanceCloseEnough));
    }
}
