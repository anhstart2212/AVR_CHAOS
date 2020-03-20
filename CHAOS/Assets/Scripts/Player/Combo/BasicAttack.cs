using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicAttack", menuName = "Generic/Basic Attack")]
public class BasicAttack : ActionData
{
    //public override IEnumerator Excecute(Transform t = null, int comboCount = 0)
    //{
    //    Init();

    //    animator.SetBool("IsInCombat",true);

    //    Debug.Log("comboCount  " + comboCount + SubActions[comboCount].triggerName);

    //    animator.SetTrigger(SubActions[comboCount].triggerName);

    //    yield return new WaitForSeconds(.5f);
    //}
}
