using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class KeyCombo
{
    public ActionData[] actions;

    public float allowedTimeBetweenButtons = 0.5f; //tweak as needed

    public KeyCombo(ActionData[] b)
    {
        actions = b;
    }

    //usage: call this once a frame. when the combo has been completed, it will return true
    public bool Check(List<ActionData> b)
    {
        int count = 0;

        try
        {
            for (int i = 0; i < b.Count; i++)
            {
                //Debug.Log("Debug    " + i +b[i].GetType().ToString() +"       "+  actions[i].GetType().ToString());
                if (b[i].GetType().ToString() == actions[i].GetType().ToString())
                {
                    count++;
                }
                else
                {
                    return false;
                }
            }
        }
        catch (System.Exception e)
        {
            return false;
        }

     
        if(count != actions.Length)
        {
            return false;
        }

        return true;
    }
}