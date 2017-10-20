using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public static class FasterMath
{
    public static Vector3 Uniform(float b)
    {
        Vector3 a;

        a.x = b;
        a.y = b;
        a.z = b;

        return a;
    }

    public static void Uniform(ref Vector3 a, float b)
    {
        a.x = b;
        a.y = b;
        a.z = b;
    }
    
    public static Vector3 Add(Vector3 a, Vector3 b)
    {
        a.x += b.x;
        a.y += b.y;
        a.z += b.z;

        return a;
    }

    public static Vector3 Add(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        a.x += b.x + c.x + d.x;
        a.y += b.y + c.y + d.y;
        a.z += b.z + c.z + d.z;

        return a;
    }

    public static Vector3 Sub(Vector3 a, Vector3 b)
    {
        a.x -= b.x;
        a.y -= b.y;
        a.z -= b.z;

        return a;
    }

    public static Vector3 Mul(Vector3 a, float b)
    {
        a.x *= b;
        a.y *= b;
        a.z *= b;

        return a;
    }

    public static Vector3 Mul(Vector3 a, Vector3 b)
    {
        a.x *= b.x;
        a.y *= b.y;
        a.z *= b.z;

        return a;
    }

    public static Vector3 Lerp(Vector3 a, Vector3 b, float u)
    {
        a.x = a.x * (1 - u) + b.x * u;
        a.y = a.y * (1 - u) + b.y * u;
        a.z = a.z * (1 - u) + b.z * u;

        return a;
    }
}
