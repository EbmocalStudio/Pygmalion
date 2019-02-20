using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FableManager : MonoBehaviour {
    public TextAsset fable;
    public FableDrawer drawer;

    private string[] lines;
    private int line = -1;
    private bool ended
    {
        get
        {
            return line >= lines.Length;
        }
    }

    private void Awake()
    {
        lines = fable.text.Split(new char[]{'\n'});
        line = 0;
    }

    /**
     * retourne la prochaine ligne   
     */   
    public string nextLine()
    {
        if (ended) return "\0";
        return lines[line++];
    }
    public string getLine(int l)
    {
        if (l < 0 || l >= lines.Length) return "\0";
        return lines[l];
    }
    public void printSpecific(int l, FableCheckpoint detector)
    {
        if (l < lines.Length && l >= 0)
            detector.assign(drawer.draw(lines[l], detector.transform));
    }
    public void printNext(FableCheckpoint detector)
    {
        if (!ended)
            detector.assign(drawer.draw(lines[line++], detector.transform));
    }
    /**
     * remet la fable a zero
     */
    public void reset()
    {
        line = 0;
    }
}
