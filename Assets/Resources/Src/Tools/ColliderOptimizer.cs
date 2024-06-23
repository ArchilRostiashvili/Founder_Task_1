using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderOptimizer : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D _colllider;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private double _tolerance;

    public void OptimizeCollider()
    {
        _colllider.pathCount = 0;
        _colllider.pathCount = _spriteRenderer.sprite.GetPhysicsShapeCount();

        List<Vector2> pathVector = new List<Vector2>();
        List<Vector2> optimizedPointsList = new List<Vector2>();
        for (int i = 0; i < _colllider.pathCount; i++)
        {
            optimizedPointsList.Clear();
            pathVector.Clear();
            _spriteRenderer.sprite.GetPhysicsShape(i, pathVector);
            optimizedPointsList = DouglasPeuckerReduction(pathVector, _tolerance);
            _colllider.SetPath(i, optimizedPointsList.ToArray());
        }
    }

    public List<Vector2> DouglasPeuckerReduction(List<Vector2> Points, Double Tolerance)
    {
        if (Points == null || Points.Count < 3)
            return Points;
        Int32 firstPoint = 0;
        Int32 lastPoint = Points.Count - 1;
        List<Int32> pointIndexesToKeep = new List<Int32>();
        //Add the first and last index to the keepers
        pointIndexesToKeep.Add(firstPoint);
        pointIndexesToKeep.Add(lastPoint);
        //The first and the last point cannot be the same
        while (Points[firstPoint].Equals(Points[lastPoint]))
        {
            lastPoint--;
        }
        DouglasPeuckerReductionRecursive(Points, firstPoint, lastPoint,
            Tolerance, ref pointIndexesToKeep);
        List<Vector2> returnPoints = new List<Vector2>();
        pointIndexesToKeep.Sort();
        foreach (Int32 index in pointIndexesToKeep)
        {
            returnPoints.Add(Points[index]);
        }
        return returnPoints;
    }
    private void DouglasPeuckerReductionRecursive(List<Vector2>
        points, Int32 firstPoint, Int32 lastPoint, Double tolerance,
        ref List<Int32> pointIndexsToKeep)
    {
        Double maxDistance = 0;
        Int32 indexFarthest = 0;
        for (Int32 index = firstPoint; index < lastPoint; index++)
        {
            Double distance = (Double)PerpendicularDistance
                (points[firstPoint], points[lastPoint], points[index]);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                indexFarthest = index;
            }
        }
        if (maxDistance > tolerance && indexFarthest != 0)
        {
            //Add the largest point that exceeds the tolerance
            pointIndexsToKeep.Add(indexFarthest);
            DouglasPeuckerReductionRecursive(points, firstPoint,
                indexFarthest, tolerance, ref pointIndexsToKeep);
            DouglasPeuckerReductionRecursive(points, indexFarthest,
                lastPoint, tolerance, ref pointIndexsToKeep);
        }
    }

    public double PerpendicularDistance
    (Vector2 Point1, Vector2 Point2, Vector2 Point)
    {
        double area = Math.Abs(.5f * (Point1.x * Point2.y + Point2.x *
            Point.y + Point.x * Point1.y - Point2.x * Point1.y - Point.x *
            Point2.y - Point1.x * Point.y));
        double bottom = Math.Sqrt(Mathf.Pow(Point1.x - Point2.x, 2f) +
            Math.Pow(Point1.y - Point2.y, 2f));
        double height = area / bottom * 2f;
        return height;
    }
}
