using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000031 RID: 49
public class PlayerActions : MonoBehaviour
{
    // Token: 0x060001C9 RID: 457 RVA: 0x0000E1C8 File Offset: 0x0000C3C8
    private void Start()
    {
        this.playerScript = base.GetComponent<Player>();
        this.anim = this.playerScript.Animator;
        this.ResetAttack();
        PlayerAnimation.SetAttackState(this.anim, this.attackPrimed, this.attackReleased);
        PlayerAnimation.SetAttackParameters(this.anim, -1f, -1f, 1f, 0f);
        this.SetDamageEffect();
    }

    // Token: 0x060001CA RID: 458 RVA: 0x0000E234 File Offset: 0x0000C434
    private void Update()
    {
        this.anim.SetBool("IsAttackAnim", PlayerAnimation.InAttackAnimation(this.anim));
        this.Attack();
    }

    // Token: 0x060001CB RID: 459 RVA: 0x0000E257 File Offset: 0x0000C457
    public void Disable()
    {
        this.ResetAttack();
        PlayerAnimation.SetAttackState(this.anim, this.attackPrimed, this.attackReleased);
        PlayerAnimation.SetAttackParameters(this.anim, -1f, -1f, 1f, 0f);
    }

    // Token: 0x060001CC RID: 460 RVA: 0x0000E295 File Offset: 0x0000C495
    public void Enable()
    {
    }

    // Token: 0x060001CD RID: 461 RVA: 0x0000E297 File Offset: 0x0000C497
    public void StartGrabEscapeAttempt()
    {
        if (this.grabEscapeRoutine != null && this.grabEscapeIsRunning)
        {
            return;
        }
        this.grabEscapeRoutine = this.GrabEscapeAttempt();
        base.StartCoroutine(this.grabEscapeRoutine);
    }

    // Token: 0x060001CE RID: 462 RVA: 0x0000E2CC File Offset: 0x0000C4CC
    private IEnumerator GrabEscapeAttempt()
    {
        this.grabEscapeIsRunning = true;
        float r = 1f - Common.GetRatio(this.timeSinceEscape, 30f, 180f);
        float escapeTime = 1.5f + 1.5f * r;
        float t = 0f;
        while (t < escapeTime)
        {
            t += Time.deltaTime;
            yield return null;
        }
        if (!this.AttemptEscapeFromGrab())
        {
            this.grabEscapeIsRunning = false;
            yield break;
        }
        this.playerScript.Animator.SetInteger(PlayerAnimation.PARAMS.grabbedState, 1);
        while (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Grabbed_Struggle"))
        {
            yield return null;
        }
        while (this.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f)
        {
            yield return null;
        }
        this.playerScript.SoundScript.SwordSound("miss");
        this.playerScript.Animator.SetInteger(PlayerAnimation.PARAMS.grabbedState, 2);
        while (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Grabbed_Slash"))
        {
            yield return null;
        }
        //Transform titan = Common.GetRootObject(base.transform);
        //TitanMain main = titan.GetComponent<TitanMain>();
        //main.ContactScript.DisableHand(main.IsLeftEating);
        this.StartGrabbedTimer();
        this.playerScript.ReleasePlayer();
        this.playerScript.EffectsScript.AttackHit("escape");
        while (this.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.33f)
        {
            this.playerScript.RigidBody.velocity = Vector3.zero;
            yield return null;
        }
        this.grabEscapeIsRunning = false;
        yield break;
    }

    // Token: 0x060001CF RID: 463 RVA: 0x0000E2E8 File Offset: 0x0000C4E8
    private bool AttemptEscapeFromGrab()
    {
        float ratio = Common.GetRatio(this.timeSinceEscape, 30f, 180f);
        return UnityEngine.Random.value <= 1f * ratio * ratio;
    }

    // Token: 0x060001D0 RID: 464 RVA: 0x0000E31E File Offset: 0x0000C51E
    private void StartGrabbedTimer()
    {
        if (this.grabTimerRoutine != null && this.timerIsRunning)
        {
            base.StopCoroutine(this.grabTimerRoutine);
        }
        this.grabTimerRoutine = this.GrabTimer();
        base.StartCoroutine(this.grabTimerRoutine);
    }

    // Token: 0x060001D1 RID: 465 RVA: 0x0000E35C File Offset: 0x0000C55C
    private IEnumerator GrabTimer()
    {
        this.timerIsRunning = true;
        this.timeSinceEscape = 0f;
        while (this.timeSinceEscape < 180f)
        {
            this.timeSinceEscape += Time.deltaTime;
            yield return null;
        }
        this.timeSinceEscape = 180f;
        this.timerIsRunning = false;
        yield break;
    }

    // Token: 0x060001D2 RID: 466 RVA: 0x0000E378 File Offset: 0x0000C578
    private void SetDamageEffect()
    {
        int num = 0;
        foreach (Transform transform in this.playerScript.transforms.playerUI.GetComponentsInChildren<Transform>())
        {
            if (num == 1)
            {
                break;
            }
            string name = transform.name;
            if (name != null)
            {
                if (name == "effect_damage_main")
                {
                    this.damageEffect = transform.GetComponent<Image>();
                    num++;
                }
            }
        }
        this.damageEffect.color = this.dmgColorClear;
    }

    // Token: 0x060001D3 RID: 467 RVA: 0x0000E40C File Offset: 0x0000C60C
    public void StartDamageEffect()
    {
        if (this.damageFadeRoutine != null && this.damageEffectActive)
        {
            base.StopCoroutine(this.damageFadeRoutine);
        }
        this.damageFadeRoutine = this.DamageFader(true);
        base.StartCoroutine(this.damageFadeRoutine);
    }

    // Token: 0x060001D4 RID: 468 RVA: 0x0000E44A File Offset: 0x0000C64A
    public void StopDamageEffect()
    {
        if (this.damageFadeRoutine != null && this.damageEffectActive)
        {
            base.StopCoroutine(this.damageFadeRoutine);
        }
        this.damageFadeRoutine = this.DamageFader(false);
        base.StartCoroutine(this.damageFadeRoutine);
    }

    // Token: 0x060001D5 RID: 469 RVA: 0x0000E488 File Offset: 0x0000C688
    private IEnumerator DamageFader(bool isFadeIn)
    {
        this.damageEffectActive = true;
        float duration = 0f;
        float alpha = this.damageEffect.color.a;
        if (isFadeIn)
        {
            duration = 0.5f - Mathf.Clamp(alpha * 0.5f, 0f, 0.5f);
        }
        else
        {
            duration = 4f - Mathf.Clamp((1f - alpha) * 4f, 0f, 4f);
        }
        Color start = this.damageEffect.color;
        Color end = (!isFadeIn) ? this.dmgColorClear : this.dmgColorFull;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            if (t > duration)
            {
                t = duration;
            }
            this.damageEffect.color = Color.Lerp(start, end, this.QuadEaseOut(t / duration));
            yield return null;
        }
        if (isFadeIn)
        {
            this.damageEffect.color = this.dmgColorFull;
        }
        else
        {
            this.damageEffect.color = this.dmgColorClear;
        }
        this.damageEffectActive = false;
        yield break;
    }

    // Token: 0x060001D6 RID: 470 RVA: 0x0000E4AA File Offset: 0x0000C6AA
    private float SineEase(float x)
    {
        return 0.5f * Mathf.Sin(6.28318548f * (x - 0.25f)) + 0.5f;
    }

    // Token: 0x060001D7 RID: 471 RVA: 0x0000E4CA File Offset: 0x0000C6CA
    private float QuadEaseOut(float x)
    {
        return -Mathf.Pow(x - 1f, 2f) + 1f;
    }

    // Token: 0x17000069 RID: 105
    // (get) Token: 0x060001D8 RID: 472 RVA: 0x0000E4E4 File Offset: 0x0000C6E4
    public int FramesSinceAttack
    {
        get
        {
            return this.framesSinceActivated;
        }
    }

    // Token: 0x1700006A RID: 106
    // (get) Token: 0x060001D9 RID: 473 RVA: 0x0000E4EC File Offset: 0x0000C6EC
    public Vector3 VelocityAtPrime
    {
        get
        {
            return this.velocityAtPrime;
        }
    }

    // Token: 0x060001DA RID: 474 RVA: 0x0000E4F4 File Offset: 0x0000C6F4
    private void Attack()
    {
        if (this.attackReleased)
        {
            base.Invoke("ResetAttack", Time.fixedDeltaTime);
        }
        bool flag = false;
        if (this.playerScript.Height > 1.25f && !Common.CurrentOrNextStateTag(this.anim, 0, "AttackRelease"))
        {
            if (playerScript.AttackKeyDown && !this.attackPrimed)
            {
                flag = (this.attackPrimed = true);
            }
        }
        else
        {
            this.attackPrimed = false;
        }
        if (this.attackPrimed)
        {
            bool flag2 = this.FindTitanTarget();
            if (this.CancelAttack(flag, !flag2))
            {
                this.attackPrimed = false;
            }
            else if (flag2 && playerScript.AttackKeyDown && !flag)
            {
                this.attackReleased = true;
                this.attackPrimed = false;
                this.framesSinceActivated = 0;
                if (Common.CurrentStateTag(this.anim, 0, PlayerAnimation.TAGS.attackPrime))
                {
                    this.playerScript.SoundScript.SwordSound("miss");
                }
            }
        }
        PlayerAnimation.SetAttackState(this.anim, this.attackPrimed, this.attackReleased);
        this.playerScript.IsAttacking = Common.CurrentOrNextStateTag(this.anim, 0, PlayerAnimation.TAGS.attackRelease);
        this.playerScript.IsAttackReadied = (!this.playerScript.IsAttacking && Common.CurrentOrNextStateTag(this.anim, 0, PlayerAnimation.TAGS.attackPrime));
        if (this.playerScript.IsAttacking)
        {
            if (this.framesSinceActivated == 0)
            {
                this.velocityAtPrime = this.playerScript.VelocityLastFrame;
            }
            if (this.framesSinceActivated < 50)
            {
                this.framesSinceActivated++;
            }
        }
        else
        {
            this.framesSinceActivated = -1;
        }
    }

    // Token: 0x060001DB RID: 475 RVA: 0x0000E6DC File Offset: 0x0000C8DC
    private bool FindTitanTarget()
    {
        return false;
        //GameObject gameObject = null;
        //RaycastHit raycastHit;
        //if (Physics.SphereCast(base.transform.position, 5f, this.playerScript.transforms.playerCamera.forward, out raycastHit, 30f, Common.layerTitan))
        //{
        //    gameObject = raycastHit.transform.gameObject;
        //}
        //else
        //{
        //    Collider[] array = Physics.OverlapSphere(base.transform.position, 2.5f, Common.layerTitan);
        //    if (array.Length > 0)
        //    {
        //        gameObject = Common.GetRootObject(array[0].transform).gameObject;
        //    }
        //}
        //if (gameObject == null)
        //{
        //    this.SetAttackParametersToDefault();
        //    return false;
        //}
        //bool flag = this.playerScript.GameScript.mode == GameMode.CityRace;
        //UnityEngine.Object component;
        //if (flag)
        //{
        //    component = gameObject.GetComponent<TrainingTitanController>();
        //}
        //else
        //{
        //    component = gameObject.GetComponent<TitanMain>();
        //}
        //if (component == null)
        //{
        //    this.SetAttackParametersToDefault();
        //    return false;
        //}
        //PlayerActions.AttackInfo nearestAttackTargetInfo;
        //if (flag)
        //{
        //    nearestAttackTargetInfo = this.GetNearestAttackTargetInfo((TrainingTitanController)component);
        //}
        //else
        //{
        //    nearestAttackTargetInfo = this.GetNearestAttackTargetInfo((TitanMain)component);
        //}
        //Vector3 to = Common.RemoveYComponent(base.transform.forward);
        //Vector3 lhs = Common.RemoveYComponent(base.transform.right);
        //Vector3 vector = Common.RemoveYComponent(nearestAttackTargetInfo.direction);
        //float value = Vector3.Angle(vector, to);
        //float num = base.transform.position.y + 1f - nearestAttackTargetInfo.position.y;
        //float num2 = Mathf.Clamp(nearestAttackTargetInfo.distance, 0f, 30f);
        //bool flag2 = Vector3.Dot(lhs, vector) < 0f;
        //float newHeight;
        //if (num < 0f)
        //{
        //    newHeight = -1f + Common.GetRatio(num, -6f, 0f);
        //}
        //else
        //{
        //    newHeight = Common.GetRatio(num, 0f, 6f);
        //}
        //float newFacing;
        //if (num < -6f || num > 6f)
        //{
        //    newFacing = 1f - Common.GetRatio(Mathf.Abs(num), 6f, 12f);
        //}
        //else
        //{
        //    newFacing = 1f - Common.GetRatio(value, 30f, 90f);
        //}
        //float newSide;
        //if (flag2)
        //{
        //    newSide = -1f;
        //}
        //else
        //{
        //    newSide = 1f;
        //}
        //this.currentSide = newSide;
        //float newDistance = 1f - num2 / 30f;
        //this.SetAttackParametersToValues(newSide, newHeight, newFacing, newDistance);
        //return nearestAttackTargetInfo.distance < 30f;
    }

    // Token: 0x060001DC RID: 476 RVA: 0x0000E962 File Offset: 0x0000CB62
    private void SetAttackParametersToDefault()
    {
        PlayerAnimation.SetAttackParametersWithLerp(this.anim, this.currentSide, -1f, 1f, 0f);
    }

    // Token: 0x060001DD RID: 477 RVA: 0x0000E984 File Offset: 0x0000CB84
    private void SetAttackParametersToValues(float newSide, float newHeight, float newFacing, float newDistance)
    {
        PlayerAnimation.SetAttackParametersWithLerp(this.anim, newSide, newHeight, newFacing, newDistance);
    }

    // Token: 0x060001DE RID: 478 RVA: 0x0000E996 File Offset: 0x0000CB96
    private void ResetAttack()
    {
        this.attackReleased = false;
        this.attackPrimed = false;
    }

    // Token: 0x060001DF RID: 479 RVA: 0x0000E9A8 File Offset: 0x0000CBA8
    private bool CancelAttack(bool sameFrame, bool releaseValid = true)
    {
        return false;
        //return (playerScript.AttackKeyDown && !sameFrame && releaseValid) || this.playerScript.IsGrounded || (/*Input.GetButtonDown(this.playerScript.axisName.fireLeftHook)*/ playerScript.FireLeftHook || /*Input.GetButtonDown(this.playerScript.axisName.fireRightHook)*/ playerScript.FireRightHook) || (this.playerScript.BurstTurnIsRunning || this.playerScript.BurstForceIsRunning) || this.playerScript.WalledState != 0;
    }


    //private PlayerActions.AttackInfo GetNearestAttackTargetInfo(TrainingTitanController ttc)
    //{
    //    Vector3 position = ttc.napeFull.transform.position;
    //    Vector3 vectorTo = Common.GetVectorTo(base.transform.position, position);
    //    float sqrMagnitude = vectorTo.sqrMagnitude;
    //    return new PlayerActions.AttackInfo(position, vectorTo, Mathf.Sqrt(sqrMagnitude));
    //}


    //private PlayerActions.AttackInfo GetNearestAttackTargetInfo(TitanMain tm)
    //{
    //    Vector3 position = tm.ContactScript.body.napeTrans.position;
    //    Vector3 vectorTo = Common.GetVectorTo(base.transform.position, position);
    //    float sqrMagnitude = vectorTo.sqrMagnitude;
    //    Vector3 position2 = tm.ContactScript.body.lKneeTrans.position;
    //    Vector3 vectorTo2 = Common.GetVectorTo(base.transform.position, position2);
    //    float sqrMagnitude2 = vectorTo2.sqrMagnitude;
    //    Vector3 position3 = tm.ContactScript.body.rKneeTrans.position;
    //    Vector3 vectorTo3 = Common.GetVectorTo(base.transform.position, position3);
    //    float sqrMagnitude3 = vectorTo3.sqrMagnitude;
    //    PlayerActions.AttackInfo result;
    //    if (sqrMagnitude < sqrMagnitude2 && sqrMagnitude < sqrMagnitude3)
    //    {
    //        result = new PlayerActions.AttackInfo(position, vectorTo, Mathf.Sqrt(sqrMagnitude));
    //    }
    //    else if (sqrMagnitude2 < sqrMagnitude3)
    //    {
    //        result = new PlayerActions.AttackInfo(position2, vectorTo2, Mathf.Sqrt(sqrMagnitude2));
    //    }
    //    else
    //    {
    //        result = new PlayerActions.AttackInfo(position3, vectorTo3, Mathf.Sqrt(sqrMagnitude3));
    //    }
    //    return result;
    //}

    // Token: 0x040001E9 RID: 489
    private Player playerScript;

    // Token: 0x040001EA RID: 490
    private Animator anim;

    // Token: 0x040001EB RID: 491
    private IEnumerator grabEscapeRoutine;

    // Token: 0x040001EC RID: 492
    private bool grabEscapeIsRunning;

    // Token: 0x040001ED RID: 493
    private const float min_escape_delay = 1.5f;

    // Token: 0x040001EE RID: 494
    private const float max_escape_delay = 3f;

    // Token: 0x040001EF RID: 495
    private IEnumerator grabTimerRoutine;

    // Token: 0x040001F0 RID: 496
    private const float max_escape_chance = 1f;

    // Token: 0x040001F1 RID: 497
    private const float max_escape_time = 180f;

    // Token: 0x040001F2 RID: 498
    private const float min_escape_time = 30f;

    // Token: 0x040001F3 RID: 499
    private float timeSinceEscape = 180f;

    // Token: 0x040001F4 RID: 500
    private bool timerIsRunning;

    // Token: 0x040001F5 RID: 501
    private Image damageEffect;

    // Token: 0x040001F6 RID: 502
    private readonly Color dmgColorFull = new Color(0.3f, 0f, 0f, 0.75f);

    // Token: 0x040001F7 RID: 503
    private readonly Color dmgColorClear = new Color(0.3f, 0f, 0f, 0f);

    // Token: 0x040001F8 RID: 504
    private IEnumerator damageFadeRoutine;

    // Token: 0x040001F9 RID: 505
    private const float fadein_time = 0.5f;

    // Token: 0x040001FA RID: 506
    private const float fadeout_time = 4f;

    // Token: 0x040001FB RID: 507
    private bool damageEffectActive;

    // Token: 0x040001FC RID: 508
    private bool attackReleased;

    // Token: 0x040001FD RID: 509
    private bool attackPrimed;

    // Token: 0x040001FE RID: 510
    private int framesSinceActivated = -1;

    // Token: 0x040001FF RID: 511
    private Vector3 velocityAtPrime = Vector3.zero;

    // Token: 0x04000200 RID: 512
    private float currentSide = -1f;

    // Token: 0x02000032 RID: 50
    internal struct AttackInfo
    {
        // Token: 0x060001E2 RID: 482 RVA: 0x0000EB9E File Offset: 0x0000CD9E
        public AttackInfo(Vector3 pos, Vector3 dir, float dist)
        {
            this.position = pos;
            this.direction = dir;
            this.distance = dist;
        }

        // Token: 0x04000201 RID: 513
        public Vector3 position;

        // Token: 0x04000202 RID: 514
        public Vector3 direction;

        // Token: 0x04000203 RID: 515
        public float distance;
    }
}
