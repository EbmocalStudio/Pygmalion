using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollisionFableCheckpoint : FableCheckpoint {

    public string[] detectedTag;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (detectedTag.Length == 0 || System.Array.IndexOf(detectedTag, other.tag) >= 0)
        {
            Debug.Log("Next line!");
            fable.printNext(this);
        }
    }

}
