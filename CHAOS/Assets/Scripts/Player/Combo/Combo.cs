using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Combo", menuName = "Generic/Combo")]
public class Combo : ScriptableObject {

    public string triggerName;

    public ActionData[] actions;

    public bool canContinue;

    public bool Check(int comboCount)
    {
        return false;
    }

    //public bool Check(List<ActionData> b)
    //{
    //    int count = 0;

    //    try
    //    {
    //        for (int i = 0; i < b.Count; i++)
    //        {
    //           // Debug.Log("Debug    " + i +b[i].GetType().ToString() +"       "+  actions[i].GetType().ToString());
    //            if (b[i].GetType().ToString() == actions[i].GetType().ToString())
    //            {
              
    //                count++;
    //            }
    //            else
    //            {
    //                return false;
    //            }
    //        }
    //    }
    //    catch (System.Exception e)
    //    {
    //        return false;
    //    }


    //    if (count != actions.Length)
    //    {
    //        return false;
    //    }

    //    Debug.Log("Combo founded! " + this.name);
    //    return true;
    //}
}
