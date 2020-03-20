using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeavyAttack", menuName = "Generic/Heavy Attack")]
public class HeavyAttack : ActionData
{
    public override IEnumerator Excecute(Transform t = null, int comboCount = 0)
    {
        Debug.Log("what");
        GameObject go = Instantiate(fxPrefab, t.position, t.rotation);

        go.transform.SetParent(t);

        yield return new WaitForSeconds(.5f);

        if (go != null)
        {
            Destroy(go);
        }
    }
}
