using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate Vector3 getPtFromBounds(Bounds b);

public class Util {

    public static readonly getPtFromBounds getTopCenterFromBounds = bounds =>
    {
        Vector3 delta = new Vector3(bounds.min.x - bounds.max.x, 0, bounds.min.z - bounds.max.z) / 2.0f;
        Vector3 topCenter = bounds.max + delta;
        return topCenter;
    };
    public static readonly getPtFromBounds getBottomCenterFromBounds = bounds =>
    {
        Vector3 delta = new Vector3(bounds.min.x - bounds.max.x, 0, bounds.min.z - bounds.max.z) / 2.0f;
        Vector3 bottomCenter = bounds.min - delta;
        return bottomCenter;
    };

    public static Vector3 Clamp(Vector3 vec, Vector3 min, Vector3 max)
    {
        return new Vector3(
          Mathf.Clamp(vec.x, min.x, max.x),
          Mathf.Clamp(vec.y, min.y, max.y),
          Mathf.Clamp(vec.z, min.z, max.z)
        );
    }

    public static float getDistanceFromRange(float val, float min, float max)
    {
        if (val > max)
            return val - max;
        if (val < min)
            return val - min;
        return 0.0f;
    }

    public static Vector3 getDistanceFromRange(Vector3 vec, Vector3 min, Vector3 max)
    {
        return new Vector3(
          getDistanceFromRange(vec.x, min.x, max.x),
          getDistanceFromRange(vec.y, min.y, max.y),
          getDistanceFromRange(vec.z, min.z, max.z)
        );
    }

    public static Vector3 getPosition(GameObject obj, Vector3 offset, getPtFromBounds func)
    {
        Bounds bounds;
        Collider2D coll = obj.GetComponent<Collider2D>();
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer)
            bounds = renderer.bounds;
        else if (coll)
            bounds = coll.bounds;
        else
            return obj.transform.position + offset;

        Vector3 point = func(bounds);
        return point + offset;
    }

    public static float GetSqrDistance(Bounds a, Bounds b)
    {
        Vector3 result = Vector3.zero;
        for (int i = 0; i < 3; i++)
        {
            if (a.min[i] > b.max[i])
                result[i] = a.min[i] - b.max[i];
            else if (a.max[i] < b.min[i])
                result[i] = b.min[0] - a.max[0];
        }
        return result.sqrMagnitude;
    }
    public static float GetSqrDistance(Bounds a, Vector3 b)
    {
        float x = Mathf.Max(0, Mathf.Min(b.x - a.max.x, b.x - a.min.x));
        float y = Mathf.Max(0, Mathf.Min(b.y - a.max.y, b.y - a.min.y));
        float z = Mathf.Max(0, Mathf.Min(b.z - a.max.z, b.z - a.min.z));

        return x * x + y * y + z * z;
    }

    public static float GetSqrDistanceTest(Bounds a, Bounds b)
    {
        float x = Mathf.Max(0, Mathf.Min(b.min.x - a.max.x, b.max.x - a.min.x));
        float y = Mathf.Max(0, Mathf.Min(b.min.y - a.max.y, b.max.y - a.min.y));
        float z = Mathf.Max(0, Mathf.Min(b.min.z - a.max.z, b.max.z - a.min.z));
        return x * x + y * y + z * z;
    }

    public static Vector3 getBottomFront(Collider2D coll)
    {
        Bounds bounds = coll.bounds;
        float front;
        if (coll.transform.localScale.x < 0)
            front = bounds.min.x;
        else
            front = bounds.max.x;
        return new Vector3(front, bounds.min.y, bounds.min.z);
    }
    public static Vector3 getBottomBack(Collider2D coll)
    {
        Bounds bounds = coll.bounds;
        float front;
        if (coll.transform.localScale.x < 0)
            front = bounds.max.x;
        else
            front = bounds.min.x;
        return new Vector3(front, bounds.min.y, bounds.min.z);
    }

    /*
     * Classe pour comparer les bounds de deux collider et decider si un est plus proche de l'objet   
     */
    public class GOComparer : IComparer<GameObject>
    {
        private Collider2D coll;
        public GOComparer(Collider2D c)
        {
            coll = c;
        }
        public int Compare(GameObject x, GameObject y)
        {
            Bounds bounds = coll.bounds;
            float distX, distY;

            Collider2D c = x.GetComponent<Collider2D>();
            if (c)
                distX = GetSqrDistance(bounds, c.bounds);
            else
                distX = GetSqrDistance(bounds, x.transform.position);
            c = y.GetComponent<Collider2D>();
            if (c)
                distY = GetSqrDistance(bounds, c.bounds);
            else
                distY = GetSqrDistance(bounds, y.transform.position);

            if (distX < distY)
                return -1;
            if (distX == distY)
                return 0;
            return 1;
        }
    }

}
