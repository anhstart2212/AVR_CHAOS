using System;
using System.Collections;
using UnityEngine;

public abstract class ActionData : ScriptableObject
{
    // Composite
    [Header("Additional Data")]
    public ActionData[] SubActions = new ActionData[0];

    // How much stamina this action consumes
    public float staminaCost;

    // can this action interrupt other actions?
    public bool interruptable;

    // can this action interrupt other actions?
    public GameObject fxPrefab;

    public virtual IEnumerator Excecute(IActionCaller caller,Transform t = null)
    {
        yield return null;
    }

}
