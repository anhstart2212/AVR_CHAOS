using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ComboList", menuName = "Generic/Combo List")]
public class ComboList : ScriptableObject
{
    public Combo[] combos;

    public Combo Check(List<ActionData> b)
    {
        foreach(Combo combo in combos)
        {
            if(combo.Check(0))
            {
                return combo;
            }
        }
        return null;
    }
}
