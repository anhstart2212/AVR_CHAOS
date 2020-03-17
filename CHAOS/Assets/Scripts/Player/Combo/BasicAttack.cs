using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicAttack", menuName = "Generic/Basic Attack")]
public class BasicAttack : ActionData
{
    public override IEnumerator Excecute(IActionCaller caller, Transform t = null)
    {
        GameObject go = Instantiate(fxPrefab,t.position,t.rotation);

        go.transform.SetParent(t);

        yield return new WaitForSeconds(.5f);

        if(caller != null)
        {
            caller.Callback(this);
        }

        if(go != null)
        {
            Destroy(go);
        }
    }
}
