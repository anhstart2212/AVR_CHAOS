using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public MeleeWeapon meleeWeapon;
    public float attackDuration = 0.03f;
    public bool canAttack;

    private AnimatorStateInfo m_CurrentStateInfo;    // Information about the base layer of the animator cached.
    private AnimatorStateInfo m_NextStateInfo;
    private AnimatorStateInfo m_PreviousCurrentStateInfo;    // Information about the base layer of the animator from last frame.
    private AnimatorStateInfo m_PreviousNextStateInfo;
    private Coroutine m_AttackWaitCoroutine;
    private bool m_Attack;
    private Player m_Player;
    private Animator m_Animator;
    private bool m_InCombo;




    


    public bool InCombo
    {
        get
        {
            return m_InCombo;
        }
    }

    // Use this for initialization
    void Start()
    {
        m_Player = GetComponent<Player>();
        m_Animator = m_Player.Animator;
        meleeWeapon = GetComponentInChildren<MeleeWeapon>();
        meleeWeapon.SetOwner(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Combat();
        CacheAnimatorState();
        EquipMeleeWeapon(IsWeaponEquiped());
    }

    private void Combat()
    {
        if (m_Animator == null || m_Player == null)
        {
            return;
        }

        m_Animator.SetFloat(PlayerAnimation.PARAMS.hashCombatStateTime, Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
        m_Animator.ResetTrigger(PlayerAnimation.PARAMS.hashMeleeAttack);

        if (m_Player.AttackKeyDown)
        {
            if (m_AttackWaitCoroutine != null)
                StopCoroutine(m_AttackWaitCoroutine);

            m_AttackWaitCoroutine = StartCoroutine(AttackWait());
        }

        if (m_Attack && canAttack)
        {
            PlayerAnimation.SetAttackState(m_Player.Animator);
        }

    }

    IEnumerator AttackWait()
    {
        m_Attack = true;
        yield return new WaitForSeconds(attackDuration);
        m_Attack = false;
    }

    bool IsWeaponEquiped()
    {
        bool equipped = m_NextStateInfo.shortNameHash == PlayerAnimation.PARAMS.hashPlayerCombo1 || m_CurrentStateInfo.shortNameHash == PlayerAnimation.PARAMS.hashPlayerCombo1;
        equipped |= m_NextStateInfo.shortNameHash == PlayerAnimation.PARAMS.hashPlayerCombo2 || m_CurrentStateInfo.shortNameHash == PlayerAnimation.PARAMS.hashPlayerCombo2;
        equipped |= m_NextStateInfo.shortNameHash == PlayerAnimation.PARAMS.hashPlayerCombo3 || m_CurrentStateInfo.shortNameHash == PlayerAnimation.PARAMS.hashPlayerCombo3;

        return equipped;
    }

    void CacheAnimatorState()
    {
        m_PreviousCurrentStateInfo = m_CurrentStateInfo;
        m_PreviousNextStateInfo = m_NextStateInfo;

        m_CurrentStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        m_NextStateInfo = m_Animator.GetNextAnimatorStateInfo(0);
    }

    void EquipMeleeWeapon(bool equip)
    {
        m_InCombo = equip;
    }

    // This is called by an animation event when swings staff.
    public void MeleeAttackStart(int throwing = 0)
    {
        meleeWeapon.BeginAttack(throwing != 0);
        //m_InAttack = true;
    }

    // This is called by an animation event when finishes swinging staff.
    public void MeleeAttackEnd()
    {
        meleeWeapon.EndAttack();
        //m_InAttack = false;
    }
}
