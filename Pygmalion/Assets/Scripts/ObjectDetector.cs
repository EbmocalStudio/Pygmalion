using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ObjectDetector : MonoBehaviour {

	public float behind, front, above, below;
    public bool constantDetection = true;
//    public int numberActivated = 1;
    public LayerMask interactiveLayer;

    private int pointer = -1; // -1 means no selection
    public GameObject[] objs;
    private Collider2D coll;

    public Vector3 min { 
        get {
            Vector3 sc = transform.localScale;
            Vector3 boundsMin = transform.position;
            if (coll)
            {
                if (sc.x < 0)
                    boundsMin.x = coll.bounds.max.x;
                else
                    boundsMin.x = coll.bounds.min.x;
                if (sc.y < 0)
                    boundsMin.y = coll.bounds.max.y;
                else
                    boundsMin.y = coll.bounds.min.y;

            }
            return boundsMin + new Vector3(-behind * Mathf.Sign(sc.x), -below * Mathf.Sign(sc.y), 0);
        } 
    }
    public Vector3 max
    {
        get {
            Vector3 sc = transform.localScale;
            Vector3 boundsMax = transform.position;
            if (coll)
            {
                if (sc.x < 0)
                    boundsMax.x = coll.bounds.min.x;
                else
                    boundsMax.x = coll.bounds.max.x;
                if (sc.y < 0)
                    boundsMax.y = coll.bounds.min.y;
                else
                    boundsMax.y = coll.bounds.max.y;

            }
            return boundsMax + new Vector3(front * Mathf.Sign(sc.x), above * Mathf.Sign(sc.y), 0);
        }
    }

    private void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        Collider2D[] newColls = Physics2D.OverlapAreaAll(min, max, interactiveLayer, -1, 1);
        GameObject[] newObjs = new GameObject[newColls.Length];
        for (int i = 0; i < newColls.Length; i++)
            newObjs[i] = newColls[i].gameObject;
        System.Array.Sort(newObjs, new Util.GOComparer(coll));
        if (pointer  >= 0 && pointer < objs.Length){
            GameObject select = objs[pointer];
            int newI = System.Array.IndexOf(newObjs, select);
            if (newI < 0 && pointer >= newObjs.Length && newObjs.Length > 0)
                pointer = 0;
            else if (newI >= 0)
                pointer = newI;
        }
        else if (newObjs.Length > 0)
            pointer = 0;
        objs = newObjs;
    }


    public GameObject GetSelected()
    {
        if (pointer < 0 || objs.Length == 0)
            return null;
        if (pointer >= objs.Length)
            Next();
        return objs[pointer];
    }
    public void Next()
    {
        if (pointer < 0)
            return;
        pointer++;
        if (pointer >= objs.Length)
            pointer = 0;
    }
    public GameObject[] GetSelected(int n)
    {
        if (pointer < 0)
            return new GameObject[0];
        int size = Mathf.Min(n, objs.Length);
        GameObject[] array = new GameObject[size];
        for (int i = 0; i < size; i++){
            array[i] = GetSelected();
            Next();
        }
        return array;
    }

/* 
       public void OnDrawGizmos()
    {
        if (coll)
        {
            Vector3 mn = min, mx = max;
            Vector3 size = (mx - mn) / 2.0f;
            Vector3 center = mn + size;
            Gizmos.DrawWireCube(center, size);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(mx, 0.1f);
            Gizmos.DrawSphere(coll.bounds.max, 0.1f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(mn, 0.1f);
            Gizmos.DrawSphere(coll.bounds.min, 0.1f);
        }

    }
*/   

}
