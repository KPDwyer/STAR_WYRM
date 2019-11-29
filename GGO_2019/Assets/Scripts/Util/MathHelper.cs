using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BoolEvent : UnityEvent<bool> { }

public static class MathHelper
{


    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }


    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static float Vector2ToDegree(Vector2 vec)
    {
        return Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
    }

    public static bool CheckAngleInRange(float angle, float lowerBound, float upperBound)
    {
        if (lowerBound > upperBound)
        {
            float t = lowerBound;
            lowerBound = upperBound;
            upperBound = t;
        }

        angle = LoopAngle(angle);
        lowerBound = LoopAngle(lowerBound);
        upperBound = LoopAngle(upperBound);

        if (lowerBound <= 0)
        {
            lowerBound += 180;
            upperBound += 180;
            angle += 180;
        }
        else if (upperBound >= 360)
        {
            lowerBound -= 180;
            upperBound -= 180;
            angle -= 180;
        }
        return (angle >= lowerBound && angle <= upperBound);
    }

    public static float LoopAngle(float angle)
    {
        if (angle > 360)
        {
            angle -= 360;
        }
        if (angle < 0)
        {
            angle += 360;
        }

        return angle;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

    }
}
