using UnityEngine;

[CreateAssetMenu(fileName = "CombatData", menuName = "Generic/Combat Data")]
public class CombatData : ScriptableObject
{
  
    public ActionData BasicAttack;
  
    public ActionData HeavyAttack;

    public ActionData Parry;

    public ActionData Shoot;

    public ActionData Dash;

}
