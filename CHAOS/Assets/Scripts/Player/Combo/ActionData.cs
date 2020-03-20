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

    // can this action interrupt other actions?
    public Animator animator;

    public string triggerName;

    public virtual IEnumerator Excecute(Transform t = null, int comboCount = 0)
    {
        yield return null;
    }

    protected void Init()
    {
        if(!animator)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            animator = go.GetComponent<Animator>();
        }
    }

}
