using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSmartTool
{
    public static class ShapeOptimizationHelper
    {
        public static List<Vector2> arrayDouglasPeuckerReduction
        (List<Vector2> arrayPoints, Double Tolerance)
        {
            if(arrayPoints == null || arrayPoints.Count < 3)
            {
                return arrayPoints;
            }


            Int32 firstPoint = 0;
            Int32 lastPoint = arrayPoints.Count - 1;
            List<Int32> arrayPointIndexesToKeep = new List<Int32>
            {
                //Add the first and last index to the keepers
                firstPoint,
                lastPoint
            };

            //The first and the last point cannot be the same
            while(arrayPoints[firstPoint].Equals(arrayPoints[lastPoint]))
            {
                lastPoint--;
            }

            DouglasPeuckerReductionRecursive(arrayPoints, firstPoint, lastPoint,
                Tolerance, ref arrayPointIndexesToKeep);

            List<Vector2> arrayReturnPoints = new List<Vector2>();
            arrayPointIndexesToKeep.Sort();
            foreach(Int32 index in arrayPointIndexesToKeep)
            {
                arrayReturnPoints.Add(arrayPoints[index]);
            }

            return arrayReturnPoints;
        }

        private static void DouglasPeuckerReductionRecursive(List<Vector2>
            arrayPoints, Int32 firstPoint, Int32 lastPoint, Double tolerance,
            ref List<Int32> arrayPointIndexesToKeep)
        {
            Double maxDistance = 0;
            Int32 indexFarthest = 0;

            for(Int32 index = firstPoint; index < lastPoint; index++)
            {
                Double distance = (Double)PerpendicularDistance
                    (arrayPoints[firstPoint], arrayPoints[lastPoint], arrayPoints[index]);
                if(distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if(maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                arrayPointIndexesToKeep.Add(indexFarthest);

                DouglasPeuckerReductionRecursive(arrayPoints, firstPoint,
                    indexFarthest, tolerance, ref arrayPointIndexesToKeep);
                DouglasPeuckerReductionRecursive(arrayPoints, indexFarthest,
                    lastPoint, tolerance, ref arrayPointIndexesToKeep);
            }
        }

        public static double PerpendicularDistance
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
}