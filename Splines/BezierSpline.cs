using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BezierSpline : MonoBehaviour 
{
    [SerializeField]
    private Vector3[] points;

    [SerializeField]
    private BezierControlPointMode[] modes;

    [SerializeField]
    private bool loop;

    public bool Loop 
    {
        get 
        {
            return loop;
        }

        set
        {
            loop = value;

            if (value == true) 
            {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }

    public int ControlPointCount 
    {
        get 
        {
            return points.Length;
        }
    }

    public float GetLength() 
    {
        float precision = 0.001f;
        float t = precision;

        float length = 0.0f;
        Vector3 lastPoint = GetPoint(0.0f);

        while (t < 1.0f)
        {
            Vector3 nextPoint = GetPoint(t);
            length += (nextPoint - lastPoint).magnitude;

            t += precision;
            lastPoint = nextPoint;
        }

        return length;
    }

    public float GetDistance(float a, float b) 
    {
        float precision = 0.001f;
        float t = a + precision;

        float distance = 0.0f;
        Vector3 lastPoint = GetPoint(a);

        while (t < b)
        {
            Vector3 nextPoint = GetPoint(t);
            distance += (nextPoint - lastPoint).magnitude;

            t += precision;
            lastPoint = nextPoint;
        }

        return distance;
    }

    public Vector3 GetPointAtDistance(float distance) 
    {
        float precision = 0.001f;
        float t = precision;

        float distanceAlong = 0.0f;
        Vector3 lastPoint = GetPoint(0.0f);

        while (distanceAlong < distance)
        {
            Vector3 nextPoint = GetPoint(t);
            distanceAlong += (nextPoint - lastPoint).magnitude;

            if (distanceAlong >= distance)
            {
                break;
            }

            t += precision;
            lastPoint = nextPoint;
        }

        return GetPoint(t);
    }

    public Vector3 GetPointAtDistance(float distance, out float outT)
    {
        float precision = 0.001f;
        float t = precision;

        float distanceAlong = 0.0f;
        Vector3 lastPoint = GetPoint(0.0f);

        while (distanceAlong < distance)
        {
            Vector3 nextPoint = GetPoint(t);
            distanceAlong += (nextPoint - lastPoint).magnitude;

            if (distanceAlong >= distance)
            {
                break;
            }

            t += precision;
            lastPoint = nextPoint;
        }

        outT = t;

        return GetPoint(t);
    }

    public Vector3 GetControlPoint (int index) 
    {
        return points[index];
    }

    public void SetControlPoint (int index, Vector3 point) 
    {
        if (index % 3 == 0) 
        {
            Vector3 delta = point - points[index];

            if (loop) 
            {
                if (index == 0) 
                {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                else if (index == points.Length - 1) 
                {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else 
                {
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            }
            else 
            {
                if (index > 0) 
                {
                    points[index - 1] += delta;
                }

                if (index + 1 < points.Length) 
                {
                    points[index + 1] += delta;
                }
            }
        }

        points[index] = point;
        EnforceMode(index);
    }

    public BezierControlPointMode GetControlPointMode (int index) 
    {
        return modes[(index + 1) / 3];
    }

    public void SetControlPointMode (int index, BezierControlPointMode mode) 
    {
        int modeIndex = (index + 1) / 3;
        modes[modeIndex] = mode;

        if (loop)
        {
            if (modeIndex == 0) 
            {
                modes[modes.Length - 1] = mode;
            }

            else if (modeIndex == modes.Length - 1) 
            {
                modes[0] = mode;
            }
        }
        EnforceMode(index);
    }

    private void EnforceMode (int index) 
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];

        if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1)) 
        {
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;

        if (index <= middleIndex) 
        {
            fixedIndex = middleIndex - 1;

            if (fixedIndex < 0) 
            {
                fixedIndex = points.Length - 2;
            }

            enforcedIndex = middleIndex + 1;

            if (enforcedIndex >= points.Length) 
            {
                enforcedIndex = 1;
            }
        }
        else 
        {
            fixedIndex = middleIndex + 1;

            if (fixedIndex >= points.Length) 
            {
                fixedIndex = 1;
            }

            enforcedIndex = middleIndex - 1;

            if (enforcedIndex < 0) 
            {
                enforcedIndex = points.Length - 2;
            }
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];

        if (mode == BezierControlPointMode.Aligned) 
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }

        points[enforcedIndex] = middle + enforcedTangent;
    }

    public int CurveCount 
    {
        get
        {
            return (points.Length - 1) / 3;
        }
    }

    public Vector3 GetPoint (float t)
    {
        int i;

        if (t >= 1f) 
        {
            t = 1f;
            i = points.Length - 4;
        }

        else 
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }
    
    public Vector3 GetVelocity (float t) 
    {
        int i;

        if (t >= 1f) 
        {
            t = 1f;
            i = points.Length - 4;
        }
        else 
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }
    
    public Vector3 GetDirection (float t) 
    {
        return GetVelocity(t).normalized;
    }

    public void AddCurve () 
    {
        Vector3 point = points[points.Length - 1];
        Vector3 dir = transform.InverseTransformVector(GetDirection(1));

        Array.Resize(ref points, points.Length + 3);
        point += dir;
        points[points.Length - 3] = point;
        point += dir;
        points[points.Length - 2] = point;
        point += dir;
        points[points.Length - 1] = point;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);

        if (loop) 
        {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }
    
    public void Reset () 
    {
        points = new Vector3[] 
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };

        modes = new BezierControlPointMode[] 
        {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

    public void SnapToSurface(Transform handleTransform, int selectedIndex)
    {
        RaycastHit upHitInfo, downHitInfo;

        Vector3 transformedPoint = handleTransform.TransformPoint(GetControlPoint(selectedIndex));
        bool upHit = Physics.Raycast(transformedPoint, Vector3.up, out upHitInfo);
        bool downHit = Physics.Raycast(transformedPoint, -Vector3.up, out downHitInfo);

        if (upHit && downHit)
        {
            if (upHitInfo.distance < downHitInfo.distance)
            {
                SetControlPoint(selectedIndex, handleTransform.InverseTransformPoint(upHitInfo.point));
            }
            else
            {
                SetControlPoint(selectedIndex, handleTransform.InverseTransformPoint(downHitInfo.point));
            }
        }
        else if (upHit)
        {
            SetControlPoint(selectedIndex, handleTransform.InverseTransformPoint(upHitInfo.point));
        }
        else if (downHit)
        {
            SetControlPoint(selectedIndex, handleTransform.InverseTransformPoint(downHitInfo.point));
        }
    }

    public void RemoveLastCurve()
    {
        if (points.Length > 3)
        {
            Array.Resize(ref points, points.Length - 3);

            modes[modes.Length - 2] = modes[modes.Length - 1];
            Array.Resize(ref modes, modes.Length - 1);
            EnforceMode(points.Length - 1);
        }
    }

    public void RemoveFirstCurve()
    {
        if (points.Length > 3)
        {
            Array.Copy(points, 3, points, 0, points.Length - 3);
            Array.Resize(ref points, points.Length - 3);

            modes[1] = modes[0];
            Array.Resize(ref modes, modes.Length - 1);
            EnforceMode(0);
        }
    }

    public void RegenerateSplineEvenly()
    {
        Vector3[] newPoints = new Vector3[points.Length];
        float distance = GetLength();
        float spacing = distance / points.Length;

        for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
        {
            if (pointIndex % 3 == 0)
            {
                Vector3 position = GetPointAtDistance(pointIndex * spacing);
                newPoints[pointIndex] = position;
            }
            else if (pointIndex % 3 == 1)
            {
                float outT = 0.0f;
                Vector3 position = GetPointAtDistance((pointIndex - 1) * spacing, out outT);
                newPoints[pointIndex] = position + GetVelocity(outT) * 0.25f;
            }
            else if (pointIndex % 3 == 2)
            {
                float outT = 0.0f;
                Vector3 position = GetPointAtDistance((pointIndex + 1) * spacing, out outT);
                newPoints[pointIndex] = position - GetVelocity(outT) * 0.25f;
            }
        }

        for (int pointIndex = 1; pointIndex < points.Length - 1; pointIndex++)
        {
            SetControlPoint(pointIndex, transform.InverseTransformPoint(newPoints[pointIndex]));
        }
    }
}
