using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Parry", menuName = "Generic/Parry")]
public class Parry : ActionData
{
    public override IEnumerator Excecute(IActionCaller caller, Transform t = null)
    {
        Debug.Log("Parry");

        GameObject go = Instantiate(fxPrefab, t.position + Vector3.down, t.rotation);

        go.transform.SetParent(t);

        go.transform.localPosition = new Vector3(1.33f, -4.87f, -2.91f);

        yield return new WaitForSeconds(.1f);

        if (caller != null)
        {
            caller.Callback(this);
        }

        if (go != null)
        {
            Destroy(go);
        }
    }
}
