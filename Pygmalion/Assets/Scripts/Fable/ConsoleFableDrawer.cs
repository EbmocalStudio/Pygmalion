using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleFableDrawer : FableDrawer
{
    public override GameObject draw(string fstr, Transform position)
    {
        Debug.Log(fstr + " @" + position.position);
        return null;
    }
}
