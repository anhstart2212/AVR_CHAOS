using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour, IActionCaller
{
    public CombatData combatData;

    public ActionData nextAction;

    public bool actionPending;

    public ActionData[] FalconPunchCombo;

    private KeyCombo falconPunch;

    private List<ActionData> queue = new List <ActionData>();

    public float timeBetweenCombo = 1;

    public float timeBetweenComboTimecount;

    public ComboList moveList;

    void Awake()
    {
        falconPunch = new KeyCombo(FalconPunchCombo);
    }

    void Update()
    {
        if(Input.GetButtonDown(InputAxisNames.attack))
        {
            this.QueueAction(combatData.BasicAttack);
        }

        if (Input.GetButtonDown(InputAxisNames.heavyAttack))
        {
            this.QueueAction(combatData.HeavyAttack);
        }

        if (Input.GetButtonDown(InputAxisNames.parry))
        {
            this.QueueAction(combatData.Parry);
        }

        if(timeBetweenComboTimecount >= 0)
        {
            timeBetweenComboTimecount -= Time.deltaTime;

            if(timeBetweenComboTimecount <= 0)
            {
                queue.Clear();
            }
        }

    }

    private void QueueAction(ActionData actionData)
    {
        if (actionData.interruptable)
        {
            this.UseAction(actionData);
        }
        else
        {
            if(actionPending)
            {
                nextAction = actionData;
            }
            else
            {
                this.UseAction(actionData);
            }
        }
    }

    // Token: 0x06000021 RID: 33 RVA: 0x00006585 File Offset: 0x00004985
    protected void UseAction(ActionData actionData)
    {
        AddCombo(actionData);

        if(CheckCombo())
        {
            print("Done Combo!...clearing old action datas");
            queue.Clear();
        }
        else
        {
            actionPending = true;

            timeBetweenComboTimecount = timeBetweenCombo;

            StartCoroutine(actionData.Excecute(this, transform));
        }
    }

    public void Callback(ActionData finishedData)
    {
        actionPending = false;

        if(nextAction != null)
        {
            this.UseAction(nextAction);
            nextAction = null;
        }
    }

    bool CheckCombo()
    {
        if(moveList.Check(queue))
        {
            queue.Clear();
            return true;
        }
        return false;
    }

    void AddCombo(ActionData action)
    {
        queue.Add(action);
    }
}
