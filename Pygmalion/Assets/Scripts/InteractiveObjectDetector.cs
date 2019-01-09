using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionReceiverSelection { First, Closest, Random };

[RequireComponent(typeof(Collider2D))]
public class InteractiveObjectDetector : MonoBehaviour {

    private List<GameObject> detectedObjs = new List<GameObject>();
    public ActionReceiverSelection selectionType = ActionReceiverSelection.Closest;
    public Collider2D detectionZone;
    public LayerMask interactiveLayer;
    public uint sortingDelay = 2048;
    private uint timePassedSinceSort;
    // -1 veut dire pas de selection
    private int selected = -1;

    IComparer<GameObject> comparer;

    public GameObject SelectedObj
    {
        get
        {
            if (selected < 0)
                return null;
            return detectedObjs[selected];
        }
    }
    public int Count
    {
        get {
            return detectedObjs.Count;
        }
    }

    private void Start()
    {
        if (!detectionZone)
            detectionZone = GetComponent<Collider2D>();
        comparer = new MyComparer(detectionZone);
    }

    private void FixedUpdate()
    {
        /*
         * Parfois trier le tableau pour mettre les objets les plus proche en avant       
         */       
        if (selectionType == ActionReceiverSelection.Closest) {
            if (timePassedSinceSort < sortingDelay)
                timePassedSinceSort++;
            else {
                timePassedSinceSort = 0;
                GameObject selectedObj = detectedObjs[selected];
                detectedObjs.Sort(comparer);

                // pour pas que le triage change l'objet selectionne
                selected = detectedObjs.IndexOf(selectedObj);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;
        AddObject(go);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;
        RemoveObj(go);
    }

    public void NextObj()
    {
        if (selected >= 0)
        {
            selected++;
            if (selected >= detectedObjs.Count)
                selected = 0;
        }
    }

    public GameObject[] getObjects(int n)
    {
        if (n <= 0)
            return new GameObject[0];
        int m = Mathf.Min(n, Count);
        GameObject[] gos = new GameObject[m];
        int cs = Count - selected;
        if (cs > m){
            detectedObjs.CopyTo(selected, gos, 0, m);
        }
        else {
            detectedObjs.CopyTo(selected, gos, 0, cs);
            detectedObjs.CopyTo(0, gos, cs, m-cs);
        }
        selected = (m + selected + 1)%Count;
        return gos;
    }

    public void RemoveObj(GameObject go)
    {
        int i = detectedObjs.IndexOf(go);
        if (i > 0)
        {
            // on change la selection si elle est affecte par le retrait
            if (i < selected)
                selected--;
            detectedObjs.Remove(go);
            // si on enleve le dernier obj et qu'il n'y en a plus
            if (detectedObjs.Count == 0)
                selected = -1;
            // si on a enleve l'obj a la fin, qui etait aussi l'objet selectionne, on loop back au debut
            else if (selected == detectedObjs.Count)
                selected = 0;
            go.SendMessage("OnDetectedExit", gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void AddObject(GameObject go)
    {
        if ((interactiveLayer.value & go.layer) != 0 && !detectedObjs.Contains(go))
        {
            detectedObjs.Add(go);
            if (selected < 0)
                selected = 0;
            go.SendMessage("OnDetectedEnter", gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }

    /*
     * Classe pour comparer les bounds de deux collider et decider si un est plus proche de l'objet   
     */
    class MyComparer : IComparer<GameObject>
    {
        private Collider2D coll;
        public MyComparer(Collider2D c)
        {
            coll = c;
        }
        public int Compare(GameObject x, GameObject y)
        {
            Bounds bounds = coll.bounds;
            float distX, distY;

            Collider2D c = x.GetComponent<Collider2D>();
            if (c)
                distX = Util.GetSqrDistance(bounds, c.bounds);
            else
                distX = Util.GetSqrDistance(bounds, x.transform.position);
            c = y.GetComponent<Collider2D>();
            if (c)
                distY = Util.GetSqrDistance(bounds, c.bounds);
            else
                distY = Util.GetSqrDistance(bounds, y.transform.position);

            if (distX < distY)
                return -1;
            if (distX == distY)
                return 0;
            return 1;
        }
    }

}
