using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt.AdvancedTutorial;

public class MeleeWeapon : Chaos_WeaponBase
{
    public CombatData combatData;

    public ActionData lastAction;

    public ActionData currentAction;

    public ActionData nextAction;

    public bool actionPending;

    public bool isInCombo;

    public bool comboFinished;

    private List<ActionData> queue = new List<ActionData>();

    public float timeBetweenCombo = 1.5f;

    public float timeBetweenComboTimecount;

    public ComboList moveList;

    Animator animator;
    void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        animator = go.GetComponent<Animator>();
    }

    void Update()
    {
        if (timeBetweenComboTimecount >= 0)
        {
            timeBetweenComboTimecount -= Time.deltaTime;

            if (timeBetweenComboTimecount <= 0)
            {
                chainCount = 0;
                lastAction = null;
                queue.Clear();
            }
        }

    }

    public override void BasicAttack()
    {
        this.QueueAction(combatData.BasicAttack);
    }

    public override void HeavyAttack()
    {
        this.QueueAction(combatData.HeavyAttack);
    }

    public override void Parry()
    {
        this.QueueAction(combatData.Parry);
    }

    private void QueueAction(ActionData actionData)
    {
        if (isInCombo)
            return;

        if (actionPending)
        {
            nextAction = actionData;
        }
        else
        {
            this.UseAction(actionData);
        }
    }

    // Token: 0x06000021 RID: 33 RVA: 0x00006585 File Offset: 0x00004985
    protected void UseAction(ActionData actionData)
    {
        AddCombo(actionData);
        Combo combo = CheckCombo();

        // If a combo is found, start animation based on that combo
        if (combo)
        {
            animator.SetTrigger(combo.triggerName);

            isInCombo = true;

            Debug.LogError("Done Combo!...clearing old action datas");
            queue.Clear();
        }
        else
        {
            actionPending = true;

            timeBetweenComboTimecount = timeBetweenCombo;

            animator.SetBool("IsInCombat", true);

            currentAction = actionData;

            if(currentAction is BasicAttack)
            {
                bool beingChained = false;
                // is last action also basic attack ?...
                if (currentAction is BasicAttack && lastAction is BasicAttack)
                {
                    beingChained = true;
                }

                // is next action also basic attack ?...
                // only check if there is no last action to check
                if (!lastAction && (currentAction is BasicAttack && nextAction is BasicAttack) )
                {
                    beingChained = true;
                }

                // If a chain is detected, increase chain count
                if (beingChained)
                {
                    // If chaincount is at its end, reset the chain
                    if (chainCount >= currentAction.SubActions.Length - 1)
                    {
                        chainCount = 0;
                    }
                    else
                    {
                        // Otherwise, continue the chain
                        chainCount++;
                    }
                }

                // Set Animation for this chain
                animator.SetTrigger(currentAction.SubActions[chainCount].triggerName);
            }

            else
            {
                chainCount = 0;
                animator.SetTrigger(currentAction.triggerName);
            }
        }
    }

    public int chainCount = 0;
    public void OnActionAnimationFinished(AnimationEvent animationEvent)
    {
       actionPending = false;

        if (nextAction != null)
        {
            this.UseAction(nextAction);

            lastAction = nextAction;

            nextAction = null;
        }
        else
        {
            lastAction = currentAction;
        }

        currentAction = null;

        if(animationEvent.intParameter == 1)
        {
            lastAction = null;
        }
    }

    Combo CheckCombo()
    {
        return moveList.Check(queue);
    }

    void AddCombo(ActionData action)
    {
        queue.Add(action);
    }

    // Call by UnityEvent Animation
    public void OnDoneCombo(AnimationEvent animationEvent)
    {
        isInCombo = false;
    }

}
