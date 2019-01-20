using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{



    public Rigidbody2D bigObj;
    private RigidbodyType2D bigObjOldState;
    private LayerMask bigObjOldLayer;
    public Dictionary<string, SmallObject> smallObjs = new Dictionary<string, SmallObject>();
    // from top center
    public Vector3 heldItemOffset;

    public void FixedUpdate()
    {
        if (bigObj)
        {
            Vector3 heldPoint = Util.getPosition(gameObject, heldItemOffset, Util.getTopCenterFromBounds);
            Vector3 bigObjPivot = Util.getPosition(bigObj.gameObject, Vector3.zero, Util.getBottomCenterFromBounds);
            Vector3 delta = bigObj.transform.position-bigObjPivot;
            bigObj.transform.position = heldPoint + delta;
        }
    }

    public void addObject(string id)
    {
        SmallObject obj;
        if (smallObjs.TryGetValue(id, out obj))
        {
            obj.count++;
        }
        else
        {
            smallObjs[id] = new SmallObject(id);
        }
    }

    public bool containsSmallObject(string id)
    {
        return smallObjs.ContainsKey(id);
    }
    public bool containsSmallObject(string id, uint n)
    {
        SmallObject obj;
        if (smallObjs.TryGetValue(id, out obj))
        {
            return obj.count >= n;
        }
        return false;
    }

    public void removeSmallObject(string id)
    {
        smallObjs.Remove(id);
    }
    public void removeSmallObject(string id, uint n)
    {
        SmallObject obj;
        if (smallObjs.TryGetValue(id, out obj))
        {
            if (n >= obj.count)
                smallObjs.Remove(id);
            else
                obj.count -= n;
        }
    }

    /*
	Rammasse l'objet en parametre.
	Si le personnage tient deja un objet, ne fait rien
	-> bool : true si l'objet a ete ramasse, sinon false
	*/
    public bool pickBigObject(Rigidbody2D obj)
    {
        if (bigObj)
        {
            return false;
        }
        bigObj = obj;
        obj.BroadcastMessage("OnPickup", SendMessageOptions.DontRequireReceiver);
        bigObjOldState = bigObj.bodyType;
        bigObj.bodyType = RigidbodyType2D.Kinematic;
        bigObjOldLayer = obj.gameObject.layer;
        obj.gameObject.layer = gameObject.layer;
        return true;
    }

    public bool dropBigObject(out Rigidbody2D obj)
    {
        if (!bigObj)
        {
            obj = null;
            return false;
        }
        obj = bigObj;
        obj.bodyType = bigObjOldState;
        obj.gameObject.layer = bigObjOldLayer;
        obj.BroadcastMessage("OnDrop", SendMessageOptions.DontRequireReceiver);
        bigObj = null;
        return true;
    }

    [System.Serializable]
    public class SmallObject
    {
        public string id;
        public uint count;
        public SmallObject(string id)
        {
            this.id = id;
            count = 1;
        }
    }
}
