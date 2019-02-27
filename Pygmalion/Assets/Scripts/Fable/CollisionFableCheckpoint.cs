using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollisionFableCheckpoint : FableCheckpoint {

    public string[] detectedTag;
    bool activated = false;
    GameObject created;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activated && (detectedTag.Length == 0 || System.Array.IndexOf(detectedTag, other.tag) >= 0))
        {
            Debug.Log("Next line!");
            fable.printNext(this);
            activated = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (created)
        {
            Destroy(created, 2.0f);
            created = null;
        }
    }
    public new void assign(GameObject obj)
    {
        created = obj;
    }

}
