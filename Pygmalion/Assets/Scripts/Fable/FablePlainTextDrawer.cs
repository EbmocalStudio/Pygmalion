using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FablePlainTextDrawer : FableDrawer {

    public override GameObject draw(string fstr, Transform position)
    {
        GameObject go = new GameObject("fablestr", typeof(TextMesh));
        TextMesh text = go.GetComponent<TextMesh>();
        text.text = fstr;
        go.transform.position = position.position;
        return go;
    }
}
