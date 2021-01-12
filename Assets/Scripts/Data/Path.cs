using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyNavigation
{
    [Serializable]
    public class Path
    {
        [SerializeField, HideInInspector]
        List<Vector2> points;

        public Path(Vector2 centre)
        {
            points = new List<Vector2>()
            {
                centre,
                centre + (Vector2.down + Vector2.left) * 5f * 0.5f,
                centre + (Vector2.down + Vector2.right) * 5f * 0.5f,
                centre + Vector2.down * 5f
            };
        }

        public void TranslatePath(Vector2 translation)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = points[i] - translation;
            }
        }

        public Vector2 this[int i]
        {
            get
            {
                return points[i];
            }
        }

        public int PointCount
        {
            get
            {
                return points.Count;
            }
        }

        public int SegmentCount
        {
            get
            {
                return (points.Count - 4) / 3 + 1;
            }
        }

        public void AddSegment(Vector2 anchorPos)
        {
            points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
            points.Add((points[points.Count - 1] + anchorPos) * 0.5f);
            points.Add(anchorPos);
        }

        public Vector2[] GetPointsInSegment(int i)
        {
            return new Vector2[] { points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[i * 3 + 3] };
        }

        public Vector2[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1)
        {
            List<Vector2> evenlySpacedPoints = new List<Vector2>();
            evenlySpacedPoints.Add(points[0]);
            Vector2 previousPoint = points[0];
            float dstSinceLastPoint = 0;

            for (int segment = 0; segment < SegmentCount; segment++)
            {
                Vector2[] p = GetPointsInSegment(segment);
                float controlNetLength = Vector2.Distance(p[0], p[1]) + Vector2.Distance(p[1], p[2]) + Vector2.Distance(p[2], p[3]);
                float estimatedCurveLength = Vector2.Distance(p[0], p[3]) + controlNetLength / 2f;
                int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10);
                float t = 0;
                while (t <= 1)
                {
                    t += 1f / divisions;
                    Vector2 pointOnCurve = Bezier.EvaluateCubic(p[0], p[1], p[2], p[3], t);
                    dstSinceLastPoint += Vector2.Distance(previousPoint, pointOnCurve);

                    while (dstSinceLastPoint >= spacing)
                    {
                        float overshootDst = dstSinceLastPoint - spacing;
                        Vector2 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDst;
                        evenlySpacedPoints.Add(newEvenlySpacedPoint);
                        dstSinceLastPoint = overshootDst;
                        previousPoint = newEvenlySpacedPoint;
                    }

                    previousPoint = pointOnCurve;
                }
            }
            return evenlySpacedPoints.ToArray();
        }

        public void MovePoint(int i, Vector2 pos)
        {
            Vector2 deltaMove = pos - points[i];
            points[i] = pos;

            if (i % 3 == 0)
            {
                if( i + 1 < points.Count) points[i + 1] += deltaMove;
                if (i - 1 >= 0) points[i - 1] += deltaMove;
            }
            else
            {
                bool nextPointIsAnchor = (i + 1) % 3 == 0;
                int correspondingControlIndex = nextPointIsAnchor ? i + 2 : i - 2;
                int anchorIndex = nextPointIsAnchor ? i + 1 : i - 1;

                if(correspondingControlIndex >= 0 && correspondingControlIndex < points.Count)
                {
                    float dst = (points[anchorIndex] - points[correspondingControlIndex]).magnitude;
                    Vector2 dir = (points[anchorIndex] - pos).normalized;
                    points[correspondingControlIndex] = points[anchorIndex] + dir * dst;
                }
            }
        }
    }
}